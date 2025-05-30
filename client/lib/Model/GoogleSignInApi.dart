import 'package:google_sign_in/google_sign_in.dart';

class GoogleSignInApi {
  static final _googleSignIn = GoogleSignIn(
    // // Указываем client ID из JSON файла
    // clientId: '27264248163-tg4vg1fjuqr45qde0p00op7b81rll8n1.apps.googleusercontent.com',
    // scopes: [
    //   'email',
    //   'profile',
    // ],
  );

  static Future<GoogleSignInAccount?> login() async {
    await _googleSignIn.signOut();
    return _googleSignIn.signIn();
  }
}
