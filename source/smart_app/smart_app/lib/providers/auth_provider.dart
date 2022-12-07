import 'package:flutter/material.dart';

class AuthProvider extends ChangeNotifier {
  String _username = "";
  String get username => _username;
  setUsername(String value) {
    if (value != _username) {
      _username = value;
      notifyListeners();
    }
  }

  String _password = "";
  String get password => _password;
  setPassword(String value) {
    if (value != _password) {
      _password = value;
      notifyListeners();
    }
  }
}
