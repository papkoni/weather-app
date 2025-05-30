import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:client/Services/config.dart';

Future<String> getUserIdByUsername(String username) async {
  try {
    final response = await http.get(
      Uri.parse('${Config.url}/getUserId?username=$username'),
      headers: <String, String>{
        'Content-Type': 'application/json',
      },
    );

    if (response.statusCode == 200) {
      String userId = json.decode(response.body)['userId'];
      print("I find user by name with: $userId");
      return userId;
    } else {
      throw Exception('Failed to find user with username');
    }
  } catch (e) {
    print('Error: $e');
    return ''; // В случае ошибки возвращаем 0
  }
}
