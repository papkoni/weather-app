import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:scrollable_positioned_list/scrollable_positioned_list.dart';
import 'package:client/Services/config.dart';
import 'package:client/Services/getUserIdByUsername.dart';
import 'package:client/View/Components/item.dart';
import 'package:http/http.dart' as http;

import '../../Model/weatherModel.dart';
import '../../Utils/staticFile.dart';

class Forecast extends StatefulWidget {
  List<WeatherModel> weatherModel = [];

  String username;

  Forecast({required this.weatherModel, required this.username});

  @override
  State<Forecast> createState() => _ForecastState();
}

class _ForecastState extends State<Forecast> {
  bool isLoading = true;
  double _opacity = 1.0;

  List<dynamic> day_img = List.filled(7, "assets/img/04n.png");
  List<dynamic> day_temp = List.filled(7, "10°C");
  List<dynamic> hour_img = List.filled(24, "assets/img/04n.png");
  List<dynamic> hour_temp = List.filled(24, "10°С");
  List<dynamic> hour_time = [];
  List<Map<String, String>> dayDateList = [];

  @override
  void initState() {
    Future.delayed(Duration(milliseconds: 300), () {
      setState(() {
        _opacity = 0.0;
      });
    });
    initDays();
    find_hour_index();
    fetchWeatherData();
    WidgetsBinding.instance.addPostFrameCallback((timeStamp) async {
      await scrollToIndex();
    });
    super.initState();
  }

  Future<void> initDays() async {
    DateTime today = DateTime.now();

    DateFormat dayFormat = DateFormat('EEEE');
    DateFormat dateFormat = DateFormat('d MMMM');

    for (int i = 0; i < 7; i++) {
      DateTime currentDay = today.add(Duration(days: i));
      dayDateList.add({
        'day': dayFormat.format(currentDay),
        'date': dateFormat.format(currentDay),
      });
    }
  }

  Future<void> fetchWeatherData() async {
    try {
      String userId = await getUserIdByUsername(widget.username);
      final response = await http
          .get(Uri.parse('${Config.url}/dailyWeather?userId=$userId'));

      if (response.statusCode == 200) {
        List<dynamic> data = json.decode(response.body);
        setState(() {
          day_img = List<String>.from(data[0]["week_weather"].map((item) => item["main_img"].toString()));
          day_temp = List<String>.from(data[0]["week_weather"].map((item) => item["main_temp"].toString()));
          hour_img = data[0]["day_weather"][0]["all_time"]["img"];
          hour_temp = data[0]["day_weather"][0]["all_time"]["temps"];
          hour_time = data[0]["day_weather"][0]["all_time"]["hour"];
        });
      } else {
        print('Ошибка при запросе данных: ${response.statusCode}');
        setState(() {
          isLoading = false;
        });
      }
    } catch (e) {
      print('Ошибка при запросе данных: $e');
      setState(() {
        isLoading = false;
      });
    }
  }

  DateTime time = DateTime.now();
  int hour_index = 0;
  bool complete1 = false;
  bool complete2 = false;

  find_hour_index() {
    String my_time;
    my_time = time.hour.toString();
    if (my_time.length == 1) {
      my_time = '0$my_time';
    }
    for (var i = 0; i < hour_time.length; i++) {
      if (hour_time[i].substring(0, 2).toString() == my_time) {
        setState(() {
          hour_index = i;
          complete2 = true;
        });
        break;
      }
    }
  }

  final ItemScrollController itemScrollController = ItemScrollController();
  final ItemPositionsListener itemPositionsListener =
      ItemPositionsListener.create();

  scrollToIndex() async {
    itemScrollController.scrollTo(
        index: hour_index,
        duration: Duration(seconds: 1),
        curve: Curves.easeInOutCubic);
  }

  @override
  Widget build(BuildContext context) {
    double myHeight = MediaQuery.of(context).size.height;
    double myWidth = MediaQuery.of(context).size.width;
    return SafeArea(
      child: Scaffold(
        backgroundColor: Color(0xff778D45),
        body: Opacity(
    opacity: 1.0, // от 0.0 (полностью прозрачный) до 1.0 (полностью непрозрачный)
    child: Container(
    height: myHeight,
    width: myWidth,
    child: Column(
              children: [
                SizedBox(
                  height: myHeight * 0.03,
                ),
                Text(
                  'Forecast report',
                  style: TextStyle(fontSize: 30, color: Colors.white),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: myWidth * 0.06),
                ),
                SizedBox(
                  height: myHeight * 0.02,
                ),
                Expanded(
                    child: ListView.builder(
                  itemCount: 7,
                  itemBuilder: (context, index) {
                    return Item(
                        date: dayDateList[index],
                        dayTemp: day_temp[index],
                        dayImg: day_img[index]);
                  },
                ))
              ],
            )),
      ),
      ),
    );
  }
}
