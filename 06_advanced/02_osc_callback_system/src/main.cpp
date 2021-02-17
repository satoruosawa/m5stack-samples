#include <M5Stack.h>

#include "./osc-manager.hpp"

OscManager OSC_MANAGER;

void setup() {
  M5.begin();
  OSC_MANAGER.Setup();
}

void loop() {
  OSC_MANAGER.Update();
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.printf("%d, %d", OSC_MANAGER.Index(), OSC_MANAGER.Value());
  delay(100);
}