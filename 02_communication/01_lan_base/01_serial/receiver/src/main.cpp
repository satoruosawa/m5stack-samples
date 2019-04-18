#include <Arduino.h>
#include <SoftwareSerial.h>

SoftwareSerial softwareSerial(10, 11); // RX, TX
int id = 1;

void setup() {
  Serial.begin(115200);
  Serial.println("Start" + String(id));

  // softwareSerial.begin(4800);
  // softwareSerial.println("0");
}

void loop() {
  if (softwareSerial.available()) {
    Serial.write(softwareSerial.read());
  }
  // if (Serial.available()) {
  //   softwareSerial.write(Serial.read());
  // }
}
