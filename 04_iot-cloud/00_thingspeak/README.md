# ThingSpeakの設定
[M5Stackで、においセンサー（TGS2450）を使ってみる。](https://magazine.halake.com/entry/m5stack-smell-sensor-thingspeak?utm_source=feed)
[mathworks/thingspeak-arduino](https://github.com/mathworks/thingspeak-arduino)
- Channelを作成
  - Channelsで New Channelをクリック
  - Nameを入力してsave
  - (設定は後からでも変更可能)
- API Keys タブからAPI Keyを取得



- wifi-info.h
```
#ifndef WIFI_INFO_H_
#define WIFI_INFO_H_

#define WIFI_SSID "xxx"
#define WIFI_PASSWORD "xxx"

#endif
```

- api-info.h
```
#ifndef API_INFO_H_
#define API_INFO_H_

const unsigned long CHANNEL_ID = 000000;
const String API_KEY = "XXXXXXXXXXXXXXX";

#endif
```