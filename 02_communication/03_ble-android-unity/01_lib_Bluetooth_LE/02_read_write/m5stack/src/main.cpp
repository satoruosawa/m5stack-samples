#include <BLEDevice.h>
#include <M5Stack.h>

#define SERVICE_UUID "00002220-0000-1000-8000-00805F9B34FB"
#define CHARACTERISTIC_UUID "00002221-0000-1000-8000-00805F9B34FB"

BLEServer* pServer = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;
std::__cxx11::string receivedValue = "";

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

class dataCb : public BLECharacteristicCallbacks {
  void onRead(BLECharacteristic* pCharacteristic) {
    // The program stops if you call M5.lcd.print function in the callback.
    Serial.println("onRead");
  }
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
  pService
      ->createCharacteristic(
          CHARACTERISTIC_UUID,
          BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_WRITE)
      ->setCallbacks(new dataCb());
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
}