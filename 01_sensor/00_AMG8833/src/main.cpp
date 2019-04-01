#include <Adafruit_AMG88xx.h>
#include <M5Stack.h>

uint16_t getColor(uint8_t red, uint8_t green, uint8_t blue);

Adafruit_AMG88xx SENSOR;
float PIXELS[AMG88xx_PIXEL_ARRAY_SIZE]; // size = 64

void setup() {
  Serial.begin(115200);
  Serial.println("AMG88xx pixels");

  // default settings
  bool status = SENSOR.begin();
  if (!status) {
    Serial.println("Could not find a valid AMG88xx sensor, check wiring!");
    while (1);
  }

  Serial.println("-- Pixels Test --");
  delay(100);
  M5.begin();
}

void loop() {
  //read all the pixels
  SENSOR.readPixels(PIXELS);

  Serial.println("--------------------------------------------------------");
  for (int i = 0; i < AMG88xx_PIXEL_ARRAY_SIZE; i++) {
    Serial.print(PIXELS[i]);
    Serial.print(", ");
    if(i % 8 == 7) Serial.println();
  }
  Serial.println("--------------------------------------------------------");
  Serial.println();

  for (int i = 0; i < 8; i++) {
    for (int j = 0; j < 8; j++) {
      int index = i + j * 8;
      float value = map(PIXELS[index], 20, 30, 0, 255);
      value = constrain(value, 0, 255);
      M5.Lcd.fillRect(j * 40, i * 30, 40, 30, getColor(value, value, value));
    }
  }

  //delay a second
  delay(100);
}

uint16_t getColor(uint8_t red, uint8_t green, uint8_t blue) {
  return ((red>>3)<<11) | ((green>>2)<<5) | (blue>>3);
}
