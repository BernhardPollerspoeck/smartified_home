import 'package:flutter/material.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final TextEditingController usernameController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  void authenticate() {}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Smart Home"),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Padding(
              padding: EdgeInsets.fromLTRB(40, 0, 0, 0),
              child: Text("Benutzername:"),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(30, 8, 30, 8),
              child: TextField(
                controller: usernameController,
                decoration: const InputDecoration(border: OutlineInputBorder()),
              ),
            ),
            const Padding(
              padding: EdgeInsets.fromLTRB(40, 0, 0, 0),
              child: Text("Passwort:"),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(30, 8, 30, 8),
              child: TextField(
                controller: passwordController,
                decoration: const InputDecoration(border: OutlineInputBorder()),
              ),
            ),
            Center(
              child: ElevatedButton(
                onPressed: authenticate,
                child: const Padding(
                  padding: EdgeInsets.fromLTRB(20, 15, 20, 15),
                  child: Text("Anmelden"),
                ),
              ),
            )
          ],
        ),
      ),
    );
  }
}
