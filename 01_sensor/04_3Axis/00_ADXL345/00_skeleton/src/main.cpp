#include <ADXL345.h>
#include <M5Stack.h>

ADXL345 adxl;

void setup() {
  M5.begin();
  adxl.powerOn();
  dacWrite(25, 0);
}

void loop() {
  int x, y, z;
  adxl.readXYZ(&x, &y, &z);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.print("values of X , Y , Z: ");
  M5.Lcd.print(x);
  M5.Lcd.print(" , ");
  M5.Lcd.print(y);
  M5.Lcd.print(" , ");
  M5.Lcd.println(z);
  delay(10);
}