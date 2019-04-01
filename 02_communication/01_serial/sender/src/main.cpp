#include <Arduino.h>
#include <M5Stack.h>

HardwareSerial hardwareSerial(2); // RX, TX
const uint8_t buttonA_GPIO = 39;
const uint8_t buttonB_GPIO = 38;

void setup() {
  M5.begin();
  Serial.begin(115200);
  Serial2.begin(115200, SERIAL_8N1, 16, 17);
  pinMode(buttonA_GPIO, INPUT);
  pinMode(buttonB_GPIO, INPUT);
  hardwareSerial.begin(4800);
  // hardwareSerial.println("0");
}

void loop() {
  if (digitalRead(buttonA_GPIO) == LOW) {
    Serial.write("Send 0");
    hardwareSerial.write("Send 0");
  }
  if (digitalRead(buttonB_GPIO) == LOW) {
    Serial2.write("Send 2");
  }
}
