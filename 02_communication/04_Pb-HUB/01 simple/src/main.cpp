#include <M5Stack.h>
#include <Wire.h>

#include "./porthub.h"

#define X_OFFSET 10
#define Y_OFFSET 18

PortHub porthub;

void setup() {
  M5.begin(true, false, true);
  M5.Power.begin();
  porthub.begin();
  M5.Lcd.clear(BLACK);
  M5.Lcd.setTextSize(4);
}

void loop() {
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.println("0  ");
  porthub.hub_a_wire_value_B(HUB1_ADDR, 200);
  delay(1000);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.println("1  ");
  porthub.hub_a_wire_value_B(HUB1_ADDR, 0);
  delay(1000);
}