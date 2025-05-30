import 'package:flutter/material.dart';
import 'package:client/Model/weatherModel.dart';

// ignore: must_be_immutable
class Item extends StatelessWidget {
  Map<String, String> date;
  String? dayTemp;
  String? dayImg;
  Item({required this.date, required this.dayTemp, required this.dayImg});

  @override
  Widget build(BuildContext context) {
    double myHeight = MediaQuery.of(context).size.height;
    double myWidth = MediaQuery.of(context).size.width;
    return Padding(
      padding: EdgeInsets.symmetric(
          vertical: myHeight * 0.015, horizontal: myWidth * 0.07),
      child: Container(
        height: myHeight * 0.11,
        decoration: BoxDecoration(
            color: Colors.white.withOpacity(0.05),
            borderRadius: BorderRadius.circular(18)),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          children: [
            Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  date["day"].toString(),
                  style: const TextStyle(color: Colors.white, fontSize: 20),
                ),
                Text(
                  date["date"].toString(),
                  style: TextStyle(
                      color: Colors.white.withOpacity(0.5), fontSize: 17),
                ),
              ],
            ),
            Row(
              children: [
                Text(
                  dayTemp!.replaceAll(" °C", "").toString(),
                  style: const TextStyle(color: Colors.white, fontSize: 55),
                ),
                const Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      ' °C',
                      style: TextStyle(color: Colors.white, fontSize: 40),
                    ),
                    Text('')
                  ],
                ),
              ],
            ),
            Container(

              height: myHeight * 0.1,
              width: myWidth * 0.15,
                child: buildIcon(dayImg.toString())
            )
          ],
        ),
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

