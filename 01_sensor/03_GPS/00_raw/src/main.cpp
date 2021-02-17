#include <M5Stack.h>

HardwareSerial GPS_s(2);

void setup() {
  M5.begin();
  GPS_s.begin(9600);
}

void loop() {
  while (GPS_s.available() > 0) {
    Serial.write(GPS_s.read());
  }
  delay(1000);
}