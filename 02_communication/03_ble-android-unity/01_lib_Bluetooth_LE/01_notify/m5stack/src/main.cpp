#include <BLE2902.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <M5Stack.h>

#define SERVICE_UUID "00002220-0000-1000-8000-00805F9B34FB"
#define CHARACTERISTIC_UUID "00002221-0000-1000-8000-00805F9B34FB"

BLEServer* pServer = NULL;
BLECharacteristic* pCharacteristic = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;
uint32_t value = 0;

class MyServerCallbacks : public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) { deviceConnected = true; };
  void onDisconnect(BLEServer* pServer) { deviceConnected = false; }
};

void setup() {
  M5.begin();
  BLEDevice::init("M5Stack");
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  BLEService* pService = pServer->createService(SERVICE_UUID);
  pCharacteristic = pService->createCharacteristic(
      CHARACTERISTIC_UUID, BLECharacteristic::PROPERTY_READ |
                               BLECharacteristic::PROPERTY_WRITE |
                               BLECharacteristic::PROPERTY_NOTIFY |
                               BLECharacteristic::PROPERTY_INDICATE);
  pCharacteristic->addDescriptor(new BLE2902());
  pService->start();
  BLEAdvertising* pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pAdvertising->setScanResponse(false);
  pAdvertising->setMinPreferred(0x0);
  BLEDevice::startAdvertising();
  Serial.println("Waiting a client connection to notify...");
  M5.Lcd.setTextSize(2);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.println("BLE");
  M5.Lcd.setCursor(0, 18);
  M5.Lcd.println("Connecting...");
}

void loop() {
  if (deviceConnected) {
    pCharacteristic->setValue((uint8_t*)&value, 4);
    pCharacteristic->notify();
    M5.Lcd.setCursor(0, 18);
    M5.Lcd.printf("Count%3d     ", value);
    value++;
    delay(100);
  }
  if (!deviceConnected && oldDeviceConnected) {
    delay(500);
    pServer->startAdvertising();
    Serial.println("start advertising");
    M5.Lcd.setCursor(0, 18);
    M5.Lcd.println("Advertising..");
    oldDeviceConnected = deviceConnected;
  }
  if (deviceConnected && !oldDeviceConnected) {
    oldDeviceConnected = deviceConnected;
  }
}