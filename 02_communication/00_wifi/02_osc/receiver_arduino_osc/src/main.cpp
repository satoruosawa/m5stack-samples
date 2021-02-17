#include <ArduinoOSC.h>
#include <M5Stack.h>
#include <WiFi.h>
#include "./wifi-info.h"

OscWiFi osc;
const String SSID = WIFI_SSID;          // "xxxx"
const String PASSWORD = WIFI_PASSWORD;  // "xxxx"
const int PORT = 10000;

void onOscReceived(OscMessage& m) {
  M5.Lcd.setCursor(0, 100);
  M5.Lcd.print("received : ");
  M5.Lcd.print(m.ip());
  M5.Lcd.print(" ");
  M5.Lcd.print(m.port());
  M5.Lcd.print(" ");
  M5.Lcd.print(m.size());
  M5.Lcd.print(" ");
  M5.Lcd.print(m.address());
  M5.Lcd.print(" ");
  M5.Lcd.print(m.arg<int>(0));
  M5.Lcd.print(" ");
  M5.Lcd.print(m.arg<float>(1));
  M5.Lcd.print(" ");
  M5.Lcd.print(m.arg<double>(2));
  M5.Lcd.print(" ");
  M5.Lcd.print(m.arg<String>(3));
  M5.Lcd.print(" ");
  M5.Lcd.print(m.arg<bool>(4));
  M5.Lcd.println();
}

void setup() {
  M5.begin();
  WiFi.begin(SSID.c_str(), PASSWORD.c_str());
  M5.Lcd.printf("Connecting to the WiFi AP: %s ", SSID.c_str());
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    M5.Lcd.print(".");
  }
  M5.Lcd.println(" connected.");
  M5.Lcd.print("IP address: ");
  M5.Lcd.print(WiFi.localIP());
  M5.Lcd.print(" Port: ");
  M5.Lcd.println(PORT);
  M5.Lcd.println("Start receiver_arduino_osc.");
  osc.begin(PORT);
  osc.subscribe("/test", onOscReceived);
}

void loop() {
  osc.parse();  // should be called
}