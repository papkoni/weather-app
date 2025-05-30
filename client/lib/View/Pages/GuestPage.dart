import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';
import 'package:client/Services/config.dart';
import 'package:client/View/Pages/login_page.dart';

class GuestPage extends StatefulWidget {
  @override
  _GuestPageState createState() => _GuestPageState();
}

class _GuestPageState extends State<GuestPage> {
  bool _isCityAdded = false;
  List<dynamic>? _weatherData;
  String? _selectedCityName;
  TextEditingController _searchController = TextEditingController();
  List<dynamic> _searchResults = [];
  List<dynamic> _cities = [];

  Future<void> _addCity(String cityName) async {
    final response = await http.get(
      Uri.parse('${Config.url}/guestWeather?cityName=$cityName'),
      headers: <String, String>{'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      setState(() {
        _weatherData = json.decode(response.body) as List;
        _selectedCityName = cityName;
        _isCityAdded = true;
      });
    } else {
      print('Failed to fetch weather data: ${response.body}');
    }
  }

  void _navigateToRegister(BuildContext context) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => LoginPage(
          weatherModel: [],
        ),
      ),
    );
  }

  void _openSearchModal(BuildContext context) async {
    await showModalBottomSheet(
      context: context,
      backgroundColor: Colors.grey[900],
      isScrollControlled: true,
      builder: (BuildContext bc) {
        return FractionallySizedBox(
          heightFactor: 0.9,
          child: Padding(
            padding: const EdgeInsets.all(8.0),
            child: Column(
              children: [
                Padding(
                  padding: const EdgeInsets.all(18.0),
                  child: TextFormField(
                    controller: _searchController,
                    decoration: InputDecoration(
                      labelText: 'Search for a city',
                      suffixIcon: Icon(Icons.search),
                      border: OutlineInputBorder(),
                    ),
                    style: TextStyle(color: Colors.white),
                    onChanged: _filterCities,
                  ),
                ),
                Expanded(
                  child: ListView.builder(
                    itemCount: _searchResults.length,
                    itemBuilder: (context, index) {
                      final city = _searchResults[index];
                      return Container(
                        margin: EdgeInsets.symmetric(vertical: 4),
                        decoration: BoxDecoration(
                          color: Colors.grey[850],
                          border: Border.all(color: Colors.grey[700]!),
                          borderRadius: BorderRadius.circular(5),
                        ),
                        child: ListTile(
                          title: Text(
                            city['name'],
                            style: TextStyle(color: Colors.white),
                          ),
                          subtitle: Text(
                            'Lat: ${city['lat']}, Lon: ${city['lon']}',
                            style:
                                TextStyle(color: Colors.white.withOpacity(0.7)),
                          ),
                          trailing: Icon(Icons.location_pin),
                          onTap: () {
                            _addCity(city['name']);
                            Navigator.pop(context);
                          },
                        ),
                      );
                    },
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
    _onModalClosed();
  }

  void _onModalClosed() {
    _loadInitialCities();
  }

  void _loadInitialCities() async {
    final jsonString = await rootBundle.loadString('assets/myJson/cities.json');
    final List<dynamic> cities = json.decode(jsonString);
    setState(() {
      _cities = cities;
      _searchResults = cities;
    });
  }

  void _filterCities(String query) {
    final filteredCities = _cities.where((city) {
      return city['name'].toLowerCase().contains(query.toLowerCase());
    }).toList();
    setState(() {
      _searchResults = filteredCities;
    });
  }

  String _formatDate(DateTime date) {
    return DateFormat('dd MMMM yyyy, EEEEE').format(date);
  }

  @override
  void initState() {
    super.initState();
    _loadInitialCities();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () => Navigator.pop(context),
        ),
        title: Text('Guest Page', style: TextStyle(color: Colors.white)),
        backgroundColor: const Color(0xff778D45),
      ),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: <Widget>[
            if (_isCityAdded)
              Container()
            else
              ElevatedButton(
                onPressed: () => _openSearchModal(context),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Color(0xff97bf53),
                  minimumSize: Size(double.infinity, 50),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(5.0),
                  ),
                ),
                child: Text(
                  'Select city',
                  style: TextStyle(color: Colors.white),
                ),
              ),
            SizedBox(height: 20),
            if (_weatherData != null) ...[
              Text(
                _selectedCityName!,
                style: TextStyle(
                    fontSize: 48,
                    color: Colors.white,
                    fontWeight: FontWeight.bold),
              ),
              SizedBox(height: 10),
              Text(
                _formatDate(DateTime.now()),
                style: TextStyle(
                    fontSize: 20, color: Colors.white.withOpacity(0.7)),
              ),
              SizedBox(height: 20),
              SizedBox(
                height: 120,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: _weatherData![0]['week_weather'].length,
                  itemBuilder: (context, index) {
                    final day = _weatherData![0]['week_weather'][index];
                    return Container(
                      width: 100,
                      margin: EdgeInsets.symmetric(horizontal: 8),
                      decoration: BoxDecoration(
                        gradient: const LinearGradient(
                          colors: [
                            Color(0xFF778D45),
                            Color(0xFF9CAF6B),
                          ],
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                        ),
                        borderRadius: BorderRadius.circular(5.0),
                      ),
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Image.network(
                            'https:${day['main_img']}',
                            width: 50,
                            height: 50,
                          ),
                          SizedBox(height: 8),
                          Text(
                            '${day['main_temp']}°C',
                            style: TextStyle(color: Colors.white),
                          ),
                        ],
                      ),
                    );
                  },
                ),
              ),
              SizedBox(height: 30),
              Expanded(
                child: ListView(
                  shrinkWrap: true,
                  children: [
                    Text(
                      'Hourly Forecast',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    SizedBox(height: 10),
                    SizedBox(
                      height: 150,
                      child: ListView.builder(
                        scrollDirection: Axis.horizontal,
                        itemCount: _weatherData![0]['day_weather'][0]['all_time']['hour'].length,
                        itemBuilder: (context, index) {
                          final hourData = _weatherData![0]['day_weather'][0]['all_time'];
                          return Container(
                            width: 80,
                            margin: EdgeInsets.symmetric(horizontal: 8),
                            decoration: BoxDecoration(
                              gradient: const LinearGradient(
                                colors: [
                                  Color(0xFF778D45),
                                  Color(0xFF9CAF6B),
                                ],
                                begin: Alignment.topLeft,
                                end: Alignment.bottomRight,
                              ),
                              borderRadius: BorderRadius.circular(5.0),
                            ),
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Text(
                                  hourData['hour'][index],
                                  style: TextStyle(color: Colors.white),
                                ),
                                SizedBox(height: 8),
                                Image.network(
                                  'https:${hourData['img'][index]}',
                                  width: 40,
                                  height: 40,
                                ),
                                SizedBox(height: 8),
                                Text(
                                  '${hourData['temps'][index]}°C',
                                  style: TextStyle(color: Colors.white),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                ),
              ),
              SizedBox(height: 20),
              Text(
                "Want to see more information and add more cities?",
                style: TextStyle(color: Colors.white.withOpacity(0.5)),
              ),
              Container(
                width: double.infinity,
                margin: EdgeInsets.symmetric(vertical: 20.0),
                child: TextButton(
                  onPressed: () => _navigateToRegister(context),
                  style: TextButton.styleFrom(
                    backgroundColor: Color(0xff97bf53),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(5.0),
                    ),
                    padding: EdgeInsets.symmetric(vertical: 12.0, horizontal: 24.0),
                  ),
                  child: Text(
                    'Login',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 16,
                    ),
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
      backgroundColor: Color(0xff778D45),
    );
  }
}