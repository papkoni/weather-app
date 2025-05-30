import 'dart:convert';
import 'dart:ffi';
import 'dart:math';

import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:scrollable_positioned_list/scrollable_positioned_list.dart';
import 'package:client/Model/weatherModel.dart';
import 'package:client/Services/config.dart';
import 'package:client/Services/getUserIdByUsername.dart';
import 'package:client/Utils/staticFile.dart';
import 'package:intl/intl.dart';
import 'package:http/http.dart' as http;
import 'package:client/View/Pages/forecast.dart';

class Home extends StatefulWidget {
  List<WeatherModel> weatherModel = [];
  String username;

  Home(
      {required this.weatherModel,
      required this.username,
      GoogleSignInAccount? user});

  @override
  State<Home> createState() => _HomeState();
}

class _HomeState extends State<Home> {
  List<WeatherModel> weatherModel = [];
  bool isKeyboardVisible = false;
  bool isLoading = true;
  bool isForecastSelected = true;

  String city_name = "Minsk";
  String main_img = "assets/img/04n.png";
  String temp = "10°С";
  String wind = "10 km/h";
  String humidity = "10%";
  List<dynamic> hour_img = List.filled(24, "assets/img/04n.png");
  List<dynamic> hour_temp = List.filled(24, "10°С");
  List<dynamic> hour_time = [];
  String userId = "";

  @override
  void initState() {
    initHourList();
    fetchWeatherData();
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((timeStamp) async {
      await scrollToIndex();
    });
    //find_hour_index();
  }

  Future<void> initHourList() async {
    DateTime now = DateTime.now();
    int currentHour = now.hour;

    hour_time = List.generate(24, (index) {
      int hour = (currentHour + index) % 24;
      return "${hour.toString().padLeft(2, '0')}:00";
    });
  }

  Future<void> fetchWeatherData() async {
    try {
      userId = await getUserIdByUsername(widget.username);
      final response =
          await http.get(Uri.parse('${Config.url}/weather?userId=$userId'));

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        setState(() {
          weatherModel = (data as List)
              .map((item) => WeatherModel.fromJson(item))
              .toList();
          isLoading = false;
          city_name = data[0]["name"];
          main_img = data[0]["weeklyWeather"][0]["mainImg"];
          temp = data[0]["weeklyWeather"][0]["mainTemp"];
          wind = data[0]["weeklyWeather"][0]["mainWind"];
          humidity = data[0]["weeklyWeather"][0]["mainHumidity"];
          hour_img = data[0]["weeklyWeather"][0]["allTime"]["img"];
          hour_temp = data[0]["weeklyWeather"][0]["allTime"]["temps"];
          hour_time = data[0]["weeklyWeather"][0]["allTime"]["hour"];
          find_hour_index();
        });
      } else {
        print('Ошибка при запросе данных: ${response.statusCode}');
        print('${response.body}');

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

  void _showWeatherChart(BuildContext context, String userId) async {
    // Initialize empty lists for FlSpot data and labels for x-axis
    List<FlSpot> _temperatureData = [];
    List<FlSpot> _humidityData = [];
    List<FlSpot> _windSpeedData = [];
    List<FlSpot> _cloudinessData = [];
    List<FlSpot> _pressureData = [];
    List<String> _xLabels = [];

    try {
      // Fetch the weather data from the endpoint
      final response = await http.get(
        Uri.parse('${Config.url}/weatherChart?userId=$userId'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        // Parse the JSON response
        final data = json.decode(response.body);
        final List<dynamic> dailyData = data['data']['daily'];

        for (var i = 0; i < dailyData.length; i++) {
          final dayData = dailyData[i];

          final date = DateTime.parse(dayData['date']);
          final dayLabel = DateFormat.E().format(date); // Day of the week (e.g., Mon, Tue)

          // Avoid duplicate days
          if (!_xLabels.contains(dayLabel)) {
            _xLabels.add(dayLabel);
          }

          _temperatureData.add(FlSpot(
            i.toDouble(),
            (dayData['temperature']['day'] as num).toDouble(),
          ));
          _humidityData.add(FlSpot(
            i.toDouble(),
            (dayData['humidity'] as num).toDouble(),
          ));
          _windSpeedData.add(FlSpot(
            i.toDouble(),
            (dayData['windSpeed'] as num).toDouble(),
          ));
        }

        // Show the modal bottom sheet with the charts
        showModalBottomSheet(
          context: context,
          backgroundColor: Colors.grey[900],
          isScrollControlled: true,
          builder: (BuildContext bc) {
            return FractionallySizedBox(
              heightFactor: 0.9,
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  children: [
                    Text(
                      'Weather Chart',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    SizedBox(height: 20),
                    Expanded(
                      child: ListView(
                        children: [
                          _buildChart('Temperature', _temperatureData, _xLabels),
                          _buildChart('Humidity', _humidityData, _xLabels),
                          _buildChart('Wind Speed', _windSpeedData, _xLabels),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      } else {
        throw Exception('Failed to load weather data');
      }
    } catch (error) {
      print('Error fetching weather data: $error');
      // Handle the error, e.g., show a dialog or a snack bar
    }
  }

// Helper method to build the charts
  Widget _buildChart(String title, List<FlSpot> data, List<String> labels) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.only(left: 16.0, top: 8.0),
          child: Text(
            title,
            style: TextStyle(
              color: Colors.white,
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        SizedBox(
          height: 250, // Увеличиваем высоту графика
          child: LineChart(
            LineChartData(
              gridData: FlGridData(show: true),
              titlesData: FlTitlesData(
                show: true,
                bottomTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    getTitlesWidget: (value, meta) {
                      final index = value.toInt();
                      return (index >= 0 && index < labels.length)
                          ? Padding(
                        padding: const EdgeInsets.only(top: 8.0),
                        child: Text(
                          labels[index],
                          style: TextStyle(
                            color: Colors.white70,
                            fontSize: 12,
                          ),
                        ),
                      )
                          : const Text('');
                    },
                    reservedSize: 32, // Место для подписей
                  ),
                ),
                leftTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    getTitlesWidget: (value, meta) {
                      // Форматируем числа до 1 знака после запятой
                      return Text(
                        '${value.toStringAsFixed(1)}',
                        style: TextStyle(
                          color: Colors.white70,
                          fontSize: 12,
                        ),
                      );
                    },
                    reservedSize: 40, // Увеличиваем место для числовых значений
                    interval: _calculateInterval(data), // Автоподбор интервала
                  ),
                ),
                rightTitles: AxisTitles(axisNameSize: 0), // Скрываем правую ось
                topTitles: AxisTitles(axisNameSize: 0), // Скрываем верхнюю ось
              ),
              lineBarsData: [
                LineChartBarData(
                  spots: data,
                  isCurved: true,
                  color: Colors.blue,
                  dotData: FlDotData(show: false),
                  belowBarData: BarAreaData(show: false),
                ),
              ],
              minX: 0,
              maxX: data.isNotEmpty ? data.last.x : 0,
              minY: data.isNotEmpty ? _findMinY(data) : 0,
              maxY: data.isNotEmpty ? _findMaxY(data) : 0,
            ),
          ),
        ),
      ],
    );
  }

// Вспомогательные функции для расчета интервалов
  double _calculateInterval(List<FlSpot> data) {
    if (data.isEmpty) return 10;
    final maxY = _findMaxY(data);
    final minY = _findMinY(data);
    final range = maxY - minY;
    return range > 20 ? 5 : (range > 10 ? 2 : 1);
  }

  double _findMaxY(List<FlSpot> data) {
    return data.map((spot) => spot.y).reduce(max) * 1;
  }

  double _findMinY(List<FlSpot> data) {
    return data.map((spot) => spot.y).reduce(min) * 0.7;
  }


  @override
  Widget build(BuildContext context) {
    isKeyboardVisible = MediaQuery.of(context).viewInsets.bottom != 0;
    double myHeight = MediaQuery.of(context).size.height;
    double myWidth = MediaQuery.of(context).size.width;
    return SafeArea(
      child: Scaffold(
        resizeToAvoidBottomInset: true,
        backgroundColor: Color(0xff778D45),
        body: Container(
            height: myHeight,
            width: myWidth,
            child: Column(
              children: [
                SizedBox(
                  height: myHeight * 0.03,
                ),
                Text(
                  city_name,
                  style: TextStyle(fontSize: 40, color: Colors.white),
                ),
                SizedBox(
                  height: myHeight * 0.01,
                ),
                Text(
                  DateFormat('dd MMMM yyyy, EEEEE').format(time),
                  style: TextStyle(
                      fontSize: 20, color: Colors.white.withOpacity(0.5)),
                ),
                SizedBox(
                  height: myHeight * 0.03,
                ),
                Container(
                  child: Text(
                    'Forecast today',
                    style: TextStyle(
                        color: Colors.white,
                        fontSize: 35),
                  ),
                ),
                SizedBox(
                  height: myHeight * 0.05,
                ),
                Container(
                  //color: Colors.amber,
                  child: buildIcon(
                    main_img
                  ),
                ),
                SizedBox(
                  height: myHeight * 0.07,
                ),
                Container(
                  height: myHeight * 0.09,
                  child: Row(
                    children: [
                      Expanded(
                          flex: 2,
                          child: Column(
                            children: [
                              Text(
                                'Temp',
                                style: TextStyle(
                                    color: Colors.white.withOpacity(0.5),
                                    fontSize: 20),
                              ),
                              Text(
                                temp + " °C",
                                style: TextStyle(
                                    color: Colors.white, fontSize: 18),
                              ),
                            ],
                          )),
                      Expanded(
                          flex: 2,
                          child: Column(
                            children: [
                              Text(
                                'Wind',
                                style: TextStyle(
                                    color: Colors.white.withOpacity(0.5),
                                    fontSize: 20),
                              ),
                              Text(
                                wind + " km/h",
                                style: TextStyle(
                                    color: Colors.white, fontSize: 18),
                              ),
                            ],
                          )),
                      Expanded(
                          flex: 2,
                          child: Column(
                            children: [
                              Text(
                                'Humidity',
                                style: TextStyle(
                                    color: Colors.white.withOpacity(0.5),
                                    fontSize: 20),
                              ),
                              Text(
                                humidity + "%",
                                style: TextStyle(
                                    color: Colors.white, fontSize: 18),
                              ),
                            ],
                          )),
                    ],
                  ),
                ),
                SizedBox(
                  height: myHeight * 0.04 - 30,
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: myWidth * 0.06),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Today',
                        style: TextStyle(color: Colors.white, fontSize: 28),
                      ),
                      InkWell(
                        onTap: () => _showWeatherChart(context, userId),
                        child: Text(
                          'View full report',
                          style: TextStyle(color: Colors.lightBlue, fontSize: 18),
                        ),
                      ),
                    ],
                  ),
                ),
                SizedBox(
                  height: myHeight * 0.02,
                ),
                Expanded(
                    child: Padding(
                  padding: EdgeInsets.only(
                      left: myWidth * 0.03, bottom: myHeight * 0.03),
                  child: ScrollablePositionedList.builder(
                    itemScrollController: itemScrollController,
                    itemPositionsListener: itemPositionsListener,
                    shrinkWrap: true,
                    scrollDirection: Axis.horizontal,
                    itemCount: 24,
                    itemBuilder: (context, index) {
                      return Padding(
                        padding: EdgeInsets.symmetric(
                            horizontal: myWidth * 0.02,
                            vertical: myHeight * 0.03),
                        child: Container(
                          width: myWidth * 0.4,
                          decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(15),
                              color: hour_index == index
                                  ? null
                                  : Colors.white.withOpacity(0.05),
                              gradient: hour_index == index
                                  ? LinearGradient(colors: [
                                Color(0xFF2D3419),
                                Color(0xFF2D3419),
                                    ])
                                  : null),
                          child: Center(
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.start,
                              children: [
                                SizedBox(
                                  width: myWidth * 0.01,
                                ),
                                Container(
                                  width: 65,
                                  height: 65,
                                  child:
                                    buildIcon(hour_img[index]),
                                ),
                                SizedBox(
                                  width: myWidth * 0.01,
                                ),
                                Column(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  children: [
                                    Text(
                                      hour_time[index],
                                      style: TextStyle(
                                          fontSize: 20, color: Colors.white),
                                    ),
                                    Text(
                                      hour_temp[index] ,
                                      style: TextStyle(
                                          fontSize: 25, color: Colors.white),
                                    ),
                                  ],
                                )
                              ],
                            ),
                          ),
                        ),
                      );
                    },
                  ),
                ))
              ],
            )),
      ),
    );
  }
}

Widget buildIcon(String icon) {
  final fullUrl = icon.startsWith('//') ? 'https:$icon' : icon;
  final isNetwork = icon.startsWith('http') || icon.startsWith('//');

  return _DelayedFadeInImage(
    imageUrl: isNetwork ? fullUrl : null,
    assetPath: isNetwork ? null : 'assets/img/$icon.png',
  );
}


class _DelayedFadeInImage extends StatefulWidget {
  final String? imageUrl;
  final String? assetPath;

  const _DelayedFadeInImage({
    this.imageUrl,
    this.assetPath,
  });

  @override
  State<_DelayedFadeInImage> createState() => _DelayedFadeInImageState();
}

class _DelayedFadeInImageState extends State<_DelayedFadeInImage>
    with SingleTickerProviderStateMixin {
  double _opacity = 0.0;

  @override
  void initState() {
    super.initState();
    Future.delayed(Duration(seconds: 1), () {
      if (mounted) {
        setState(() {
          _opacity = 1.0;
        });
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    Widget imageWidget;

    if (widget.imageUrl != null) {
      imageWidget = Image.network(
        widget.imageUrl!,
        fit: BoxFit.contain,
        errorBuilder: (context, error, stackTrace) {
          return Image.asset('assets/img/default_icon.png');
        },
      );
    } else {
      imageWidget = Image.asset(widget.assetPath!);
    }

    return AnimatedOpacity(
      opacity: _opacity,
      duration: Duration(milliseconds: 600),
      child: imageWidget,
    );
  }
}
