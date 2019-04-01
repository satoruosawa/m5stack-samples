#include "./EmaDetection.h"

EmaDetection::EmaDetection ()
: refEmaCoef(0.01), sampleEmaCoef(0.1), mlx90640Address(0x33) {
  // Coefs should be below.
  // refEmaCoef < sampleEmaCoef
  rawValues = new float[VALUE_COUNT];
  refEmaValues = new double[VALUE_COUNT];
  sampleEmaValues = new double[VALUE_COUNT];
  diffValues = new double[VALUE_COUNT];
  for (int i = 0; i < VALUE_COUNT; i++) {
    rawValues[i] = 0;
    refEmaValues[i] = 0;
    sampleEmaValues[i] = 0;
    diffValues[i] = 0;
  }
}

void EmaDetection::Setup () {
  Wire.begin();
  Wire.setClock(400000); //Increase I2C clock speed to 400kHz
  if (isConnected() == false) {
    Serial.println("MLX90640 not detected at default I2C address. Please check wiring. Freezing.");
    while (1);
  }
  Serial.println("MLX90640 online!");

  int status;
  uint16_t eeMLX90640[832];
  status = MLX90640_DumpEE(mlx90640Address, eeMLX90640);
  if (status != 0)
    Serial.println("Failed to load system parameters");

  status = MLX90640_ExtractParameters(eeMLX90640, &mlx90640);
  if (status != 0)
    Serial.println("Parameter extraction failed");

  //Set refresh rate
  //A rate of 0.5Hz takes 4Sec per reading because we have to read two frames to get complete picture
  //MLX90640_SetRefreshRate(mlx90640Address, 0x00); //Set rate to 0.25Hz effective - Works
  //MLX90640_SetRefreshRate(mlx90640Address, 0x01); //Set rate to 0.5Hz effective - Works
  //MLX90640_SetRefreshRate(mlx90640Address, 0x02); //Set rate to 1Hz effective - Works
  //MLX90640_SetRefreshRate(mlx90640Address, 0x03); //Set rate to 2Hz effective - Works
  // MLX90640_SetRefreshRate(mlx90640Address, 0x04); //Set rate to 4Hz effective - Works
  MLX90640_SetRefreshRate(mlx90640Address, 0x05); //Set rate to 8Hz effective - Works at 800kHz
  // MLX90640_SetRefreshRate(mlx90640Address, 0x06); //Set rate to 16Hz effective - Works at 800kHz
  // MLX90640_SetRefreshRate(mlx90640Address, 0x07); //Set rate to 32Hz effective - fails

  //Once EEPROM has been read at 400kHz we can increase to 1MHz
  Wire.setClock(1000000); //Teensy will now run I2C at 800kHz (because of clock division)

}

void EmaDetection::Update () {
  UpdateRaw();
  UpdateRef();
  UpdateSample();
  UpdateDiff();
}

void EmaDetection::UpdateRaw () {
  for (byte x = 0 ; x < 2 ; x++) {
    uint16_t mlx90640Frame[834];
    int status = MLX90640_GetFrameData(mlx90640Address, mlx90640Frame);

    float vdd = MLX90640_GetVdd(mlx90640Frame, &mlx90640);
    float Ta = MLX90640_GetTa(mlx90640Frame, &mlx90640);

    float tr = Ta - TA_SHIFT; //Reflected temperature based on the sensor ambient temperature
    float emissivity = 0.95;

    MLX90640_CalculateTo(mlx90640Frame, &mlx90640, emissivity, tr, rawValues);
  }
}

void EmaDetection::UpdateRef () {
  for (int i = 0; i < VALUE_COUNT; i++) {
    double value = static_cast<double>(rawValues[i]);
    if (value < -100 || value > 100) return;
    refEmaValues[i] = DoubleLerp(0, refEmaValues[i], 1, value, refEmaCoef);
  }
}

void EmaDetection::UpdateSample () {
  for (int i = 0; i < VALUE_COUNT; i++) {
    double value = static_cast<double>(rawValues[i]);
    if (value < -100 || value > 100) return;
    sampleEmaValues[i] = DoubleLerp(0, sampleEmaValues[i], 1, value, sampleEmaCoef);
  }
}

void EmaDetection::UpdateDiff () {
  float diffMin = 100;
  float diffMax = -100;
  for (int i = 0; i < VALUE_COUNT; i++) {
    double ref = refEmaValues[i];
    double sample = sampleEmaValues[i];
    double diff = sample - ref;
    diffValues[i] = diff;
    if (diff < diffMin) {
      diffMinIndex = i;
      diffMin = diff;
    }
    if (diff > diffMax) {
      diffMaxIndex = i;
      diffMax = diff;
    }
  }
}

void EmaDetection::Draw () {
  for (int y = 0; y < 24; y++) {
    for (int x = 0; x < 32; x++) {
      int index = x + y * 32;
      float min = static_cast<float>(diffValues[diffMinIndex]);
      float max = static_cast<float>(diffValues[diffMaxIndex]);
      float diff = static_cast<float>(diffValues[index]);
      float value = FloatMap(diff, min, max, 0, 255);
      value = constrain(value, 0, 255);
      M5.Lcd.fillRect(x * 10, y * 10, 10, 10, GetColor(value, value, value));
    }
  }
}

boolean EmaDetection::isConnected () {
  Wire.beginTransmission((uint8_t)mlx90640Address);
  if (Wire.endTransmission() != 0) return false; //Sensor did not ACK
  return true;
}
