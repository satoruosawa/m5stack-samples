#include <M5Stack.h>
#include <SparkFun_I2C_Mux_Arduino_Library.h>
#include <Wire.h>

const int pin = 21;
int freq = 10000;
int channel = 0;
int resolution = 10;
QWIICMUX myMux;

void setup() {
  M5.begin();
  dacWrite(25, 0);
  Wire.begin();
  if (myMux.begin() == false) {
    Serial.println("Mux not detected. Freezing...");
    while (1)
      ;
  }
  myMux.setPort(0);
}

void loop() {
  myMux.setPort(0);
  ledcSetup(channel, freq, resolution);
  ledcAttachPin(pin, channel);
  delay(1000);
  ledcWrite(channel, 512);
  delay(1000);
  ledcWrite(channel, 0);
  delay(1000);
  ledcWrite(channel, 400);
  delay(1000);
  ledcWrite(channel, 0);
  delay(1000);
}