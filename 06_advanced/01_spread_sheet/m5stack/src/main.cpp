#include <ArduinoJson.h>
#include <HTTPClient.h>
#include <M5Stack.h>
#include <WiFi.h>

#include "./secret-config.h"

// Json設定
StaticJsonDocument<255> json_request;
char buffer[255];

// カウント初期化
int count = 0;

void setup() {
  M5.begin();

  // Wi-Fi接続
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  M5.Lcd.printf("Connecting to the WiFi AP: %s ", WIFI_SSID);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    M5.Lcd.print(".");
  }
  M5.Lcd.println(" connected.");
  M5.Lcd.print("IP address: ");
  M5.Lcd.println(WiFi.localIP());
}

// カウント値送信
void sendCount() {
  json_request["count"] = count;
  serializeJson(json_request, buffer, sizeof(buffer));

  HTTPClient http;
  http.begin(WEB_APP_URL);
  http.addHeader("Content-Type", "application/json");
  int status_code = http.POST((uint8_t *)buffer, strlen(buffer));
  Serial.println(status_code);
  if (status_code > 0) {
    if (status_code == HTTP_CODE_FOUND) {
      String payload = http.getString();
      Serial.println(payload);

      M5.Lcd.setCursor(10, 100);
      M5.Lcd.fillScreen(BLACK);
      M5.Lcd.setTextColor(WHITE);
      M5.Lcd.setTextSize(3);
      M5.Lcd.println("Send Done!");
    }
  } else {
    Serial.printf("[HTTP] GET... failed, error: %s\n",
                  http.errorToString(status_code).c_str());
  }
  http.end();
}

void loop() {
  M5.update();

  if (M5.BtnA.wasReleased()) {
    // カウントアップ
    count++;

    // ディスプレイ表示
    M5.Lcd.setCursor(10, 100);
    M5.Lcd.fillScreen(RED);
    M5.Lcd.setTextColor(YELLOW);
    M5.Lcd.setTextSize(3);
    M5.Lcd.printf("Count Up: %d", count);
  }

  if (M5.BtnC.wasReleased()) {
    // カウントダウン
    count--;

    // ゼロ以下にはしない
    if (count <= 0) count = 0;

    // ディスプレイ表示
    M5.Lcd.setCursor(10, 100);
    M5.Lcd.fillScreen(GREEN);
    M5.Lcd.setTextColor(BLACK);
    M5.Lcd.setTextSize(3);
    M5.Lcd.printf("Count Down: %d", count);
  }

  if (M5.BtnB.wasReleased()) {
    // ディスプレイ表示
    M5.Lcd.setCursor(10, 100);
    M5.Lcd.fillScreen(BLUE);
    M5.Lcd.setTextColor(WHITE);
    M5.Lcd.setTextSize(3);
    M5.Lcd.printf("Count Send: %d", count);

    // カウント送信
    sendCount();
  }
}