#include <M5Stack.h>
#include <Wire.h>
#include "./library/MLX90640_API.h"
#include "./library/MLX90640_I2C_Driver.h"

#define TA_SHIFT 8  // Default shift for MLX90640 in open air

boolean isConnected();
uint16_t getColor(uint8_t red, uint8_t green, uint8_t blue);

const byte MLX90640_ADDRESS =
    0x33;  // Default 7-bit unshifted address of the MLX90640
static float MLX90640_TO[768];  // 32 * 24
paramsMLX90640 MLX90640;

void setup() {
  Wire.begin();
  Wire.setClock(400000);  // Increase I2C clock speed to 400kHz

  Serial.begin(115200);
  while (!Serial)
    ;  // Wait for user to open terminal
  Serial.println("MLX90640 IR Array Example");

  if (isConnected() == false) {
    Serial.println(
        "MLX90640 not detected at default I2C address. Please check wiring. "
        "Freezing.");
    while (1)
      ;
  }
  Serial.println("MLX90640 online!");

  // Get device parameters - We only have to do this once
  int status;
  uint16_t eeMLX90640[832];
  status = MLX90640_DumpEE(MLX90640_ADDRESS, eeMLX90640);
  if (status != 0) Serial.println("Failed to load system parameters");

  status = MLX90640_ExtractParameters(eeMLX90640, &MLX90640);
  if (status != 0) Serial.println("Parameter extraction failed");

  // Set refresh rate
  // A rate of 0.5Hz takes 4Sec per reading because we have to read two frames
  // to get complete picture MLX90640_SetRefreshRate(MLX90640_ADDRESS, 0x00);
  // //Set rate to 0.25Hz effective - Works
  // MLX90640_SetRefreshRate(MLX90640_ADDRESS, 0x01); //Set rate to 0.5Hz
  // effective - Works MLX90640_SetRefreshRate(MLX90640_ADDRESS, 0x02); //Set
  // rate to 1Hz effective - Works MLX90640_SetRefreshRate(MLX90640_ADDRESS,
  // 0x03); //Set rate to 2Hz effective - Works
  // MLX90640_SetRefreshRate(MLX90640_ADDRESS, 0x04); //Set rate to 4Hz
  // effective - Works
  MLX90640_SetRefreshRate(MLX90640_ADDRESS,
                          0x05);  // Set rate to 8Hz effective - Works at 800kHz
  // MLX90640_SetRefreshRate(MLX90640_ADDRESS, 0x06); //Set rate to 16Hz
  // effective - Works at 800kHz MLX90640_SetRefreshRate(MLX90640_ADDRESS,
  // 0x07); //Set rate to 32Hz effective - fails

  // Once EEPROM has been read at 400kHz we can increase to 1MHz
  Wire.setClock(1000000);  // Teensy will now run I2C at 800kHz (because of
                           // clock division)

  // Once params are extracted, we can release eeMLX90640 array
  M5.begin();
}

void loop() {
  long startTime = millis();
  for (byte x = 0; x < 2; x++) {
    uint16_t mlx90640Frame[834];
    int status = MLX90640_GetFrameData(MLX90640_ADDRESS, mlx90640Frame);

    float vdd = MLX90640_GetVdd(mlx90640Frame, &MLX90640);
    float Ta = MLX90640_GetTa(mlx90640Frame, &MLX90640);

    float tr = Ta - TA_SHIFT;  // Reflected temperature based on the sensor
                               // ambient temperature
    float emissivity = 0.95;

    MLX90640_CalculateTo(mlx90640Frame, &MLX90640, emissivity, tr, MLX90640_TO);
  }
  long stopReadTime = millis();

  for (int y = 0; y < 24; y++) {
    for (int x = 0; x < 32; x++) {
      int index = x + y * 32;
      float value = map(MLX90640_TO[index], 20, 30, 0, 255);
      value = constrain(value, 0, 255);
      M5.Lcd.fillRect(x * 10, y * 10, 10, 10, getColor(value, value, value));
    }
  }
  long stopPrintTime = millis();

  Serial.print("Read rate: ");
  Serial.print(1000.0 / (stopReadTime - startTime), 2);
  Serial.println(" Hz");
  Serial.print("Read plus print rate: ");
  Serial.print(1000.0 / (stopPrintTime - startTime), 2);
  Serial.println(" Hz");
}

// Returns true if the MLX90640 is detected on the I2C bus
boolean isConnected() {
  Wire.beginTransmission((uint8_t)MLX90640_ADDRESS);
  if (Wire.endTransmission() != 0) return false;  // Sensor did not ACK
  return true;
}

uint16_t getColor(uint8_t red, uint8_t green, uint8_t blue) {
  return ((red >> 3) << 11) | ((green >> 2) << 5) | (blue >> 3);
}
