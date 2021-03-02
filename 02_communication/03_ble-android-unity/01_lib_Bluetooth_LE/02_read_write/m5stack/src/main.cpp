#include <BLEDevice.h>
#include <M5Stack.h>

#define SERVICE_UUID "00002220-0000-1000-8000-00805F9B34FB"
#define READ_CHARACTERISTIC_UUID "00002221-0000-1000-8000-00805F9B34FB"
#define WRITE_CHARACTERISTIC_UUID "00002222-0000-1000-8000-00805F9B34FB"

BLECharacteristic* pReadCharacteristic = NULL;
BLEServer* pServer = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;
std::string receivedValue = "0";
uint32_t readValue = 0;

class MyServerCallbacks : public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    // The program stops if you call M5.lcd.print function in the callback.
    Serial.println("connect");
    deviceConnected = true;
  };
  void onDisconnect(BLEServer* pServer) {
    // The program stops if you call M5.lcd.print function in the callback.
    Serial.println("disconnect");
    deviceConnected = false;
  }
};

class MyReadCharacteristicCallbacks : public BLECharacteristicCallbacks {
  void onRead(BLECharacteristic* pCharacteristic) {
    // The program stops if you call M5.lcd.print function in the callback.
    Serial.println("onRead");
  }
};

class MyWriteCharacteristicCallbacks : public BLECharacteristicCallbacks {
  void onWrite(BLECharacteristic* pCharacteristic) {
    // The program stops if you call M5.lcd.print function in the callback.
    Serial.println("onWrite");
    receivedValue = pCharacteristic->getValue();
    Serial.println(receivedValue.c_str());
  }
};

void setup() {
  M5.begin();
  BLEDevice::init("M5Stack");
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  BLEService* pService = pServer->createService(SERVICE_UUID);

  pReadCharacteristic = pService->createCharacteristic(
      READ_CHARACTERISTIC_UUID, BLECharacteristic::PROPERTY_READ);
  pReadCharacteristic->setCallbacks(new MyReadCharacteristicCallbacks());
  pReadCharacteristic->setValue((uint8_t*)&readValue, 4);
  pService
      ->createCharacteristic(WRITE_CHARACTERISTIC_UUID,
                             BLECharacteristic::PROPERTY_WRITE)
      ->setCallbacks(new MyWriteCharacteristicCallbacks());
  pService->start();
  pServer->startAdvertising();
  Serial.println("Start advertising...  ");
  M5.Lcd.setTextSize(2);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.println("BLE Read Write");
  M5.Lcd.setCursor(0, 18);
  M5.Lcd.println("Start advertising...  ");
}

void loop() {
  if (deviceConnected) {
    M5.Lcd.println("                      ");
  }
  if (!deviceConnected && oldDeviceConnected) {
    delay(500);
    pServer->startAdvertising();
    Serial.println("Restart advertising");
    M5.Lcd.setCursor(0, 18);
    M5.Lcd.println("Restart advertising...");
    oldDeviceConnected = deviceConnected;
  }
  if (deviceConnected && !oldDeviceConnected) {
    oldDeviceConnected = deviceConnected;
  }
  static unsigned long prevUpdate = 0;
  if (millis() > prevUpdate + 5000) {
    prevUpdate = millis();
    readValue++;
    pReadCharacteristic->setValue((uint8_t*)&readValue, 4);
  }
  M5.Lcd.setCursor(0, 36);
  M5.Lcd.printf("R %-3d W %s   ", readValue, receivedValue.c_str());
}