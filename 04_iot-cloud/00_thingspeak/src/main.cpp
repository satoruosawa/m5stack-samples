#include <HTTPClient.h>
#include <M5Stack.h>
#include <ThingSpeak.h>
#include <WiFi.h>
#include "./api-info.h"
#include "./wifi-info.h"

WiFiClient WIFI_CLIENT;
const String SSID = WIFI_SSID;          // "xxxx"
const String PASSWORD = WIFI_PASSWORD;  // "xxxx"

void connectWifi();

void setup() {
  M5.begin();
  connectWifi();
  ThingSpeak.begin(WIFI_CLIENT);
}

int cnt = 0;

void loop() {
  if (WiFi.status() != WL_CONNECTED) {
    connectWifi();
  }

  ThingSpeak.setField(1, cnt);
  int return_code = ThingSpeak.writeFields(CHANNEL_ID, API_KEY.c_str());

  if (return_code == 200) {
    Serial.println("Channel update successful.");
  } else {
    Serial.println("Problem updating channel. HTTP error code " +
                   String(return_code));
  }

  cnt++;
  delay(60000);
}

void connectWifi() {
  WiFi.begin(SSID.c_str(), PASSWORD.c_str());
  M5.Lcd.printf("Connecting to the WiFi AP: %s ", SSID.c_str());
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    M5.Lcd.print(".");
  }
  M5.Lcd.println(" connected.");
  M5.Lcd.print("IP address: ");
  M5.Lcd.println(WiFi.localIP());
}
