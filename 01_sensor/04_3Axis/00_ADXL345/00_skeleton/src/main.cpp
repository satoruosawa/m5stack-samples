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
  Serial.print("values of X , Y , Z: ");
  Serial.print(x);
  Serial.print(" , ");
  Serial.print(y);
  Serial.print(" , ");
  Serial.println(z);
  delay(10);
}