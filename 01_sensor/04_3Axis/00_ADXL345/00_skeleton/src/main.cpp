#include <ADXL345.h>
#include <M5Stack.h>

ADXL345 adxl;

void setup() {
  M5.begin();
  adxl.powerOn();

  // Tap
  adxl.setTapDetectionOnX(0);
  adxl.setTapDetectionOnY(0);
  adxl.setTapDetectionOnZ(1);
  adxl.setTapThreshold(50);
  adxl.setTapDuration(15);
  adxl.setInterruptMapping(ADXL345_INT_SINGLE_TAP_BIT, ADXL345_INT1_PIN);
  adxl.setInterrupt(ADXL345_INT_SINGLE_TAP_BIT, 1);

  dacWrite(25, 0);
}

void loop() {
  int x, y, z;
  adxl.readXYZ(&x, &y, &z);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.print("Values of X , Y , Z: ");
  M5.Lcd.print(x);
  M5.Lcd.print(" , ");
  M5.Lcd.print(y);
  M5.Lcd.print(" , ");
  M5.Lcd.println(z);
  M5.Lcd.print("Tap detection: ");
  M5.Lcd.println(adxl.triggered(adxl.getInterruptSource(), ADXL345_SINGLE_TAP));
  delay(100);
}