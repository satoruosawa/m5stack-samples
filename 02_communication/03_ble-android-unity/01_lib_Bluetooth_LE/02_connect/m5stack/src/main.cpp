#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <M5Stack.h>
#include <Wire.h>
#include <esp_gatts_api.h>
#include <esp_wifi.h>

#define SERVICE_UUID "00002220-0000-1000-8000-00805F9B34FB"
#define CHARACTERISTIC_UUID "00002221-0000-1000-8000-00805F9B34FB"

BLEServer* pServer = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;

class MyServerCallbacks : public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    Serial.println("connect");
    deviceConnected = true;
  };
  void onDisconnect(BLEServer* pServer) {
    Serial.println("disconnect");
    deviceConnected = false;
  }
};

class dataCb : public BLECharacteristicCallbacks {
  void onRead(BLECharacteristic* pCharacteristic) { Serial.println("onRead"); }
  void onWrite(BLECharacteristic* pCharacteristic) {
    Serial.println("onWrite");
    Serial.println(pCharacteristic->getValue().c_str());
  }
};

void setup() {
  M5.begin();
  dacWrite(25, 0);  // Speaker OFF

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
  pServer->getAdvertising()->start();
  Serial.println("Characteristic defined! Now you can read it in your phone!");
}

void loop() {
  if (deviceConnected) {
    //
  }
  if (!deviceConnected && oldDeviceConnected) {
    delay(500);
    pServer->getAdvertising()->start();
    Serial.println("start advertising");
    M5.Lcd.setCursor(0, 18);
    M5.Lcd.println("Advertising..");
    oldDeviceConnected = deviceConnected;
  }
  if (deviceConnected && !oldDeviceConnected) {
    oldDeviceConnected = deviceConnected;
  }
}