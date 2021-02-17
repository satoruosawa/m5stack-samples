#include <M5Stack.h>

int x = 0;
int y = 100;

void setup() {
  M5.begin();
  M5.Lcd.clear(BLACK);
}

void loop() {
  M5.Lcd.clear();
  M5.Lcd.fillRect(100, y, 50, 50, RED);
  M5.Lcd.fillRect(x, 100, 50, 50, BLUE);
  M5.Lcd.fillRect(x, 100, 25, 25, GREEN);
  x++;
  if (x > 1000) x = 0;
  y++;
  if (y > 777) y = 0;
  delay(15);
}