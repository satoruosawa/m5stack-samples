#include <BLEDevice.h>
#include <M5Stack.h>

void setAdvData(BLEAdvertising *pAdvertising) {
  BLEAdvertisementData oAdvertisementData = BLEAdvertisementData();
  oAdvertisementData.setFlags(
      0x06);  // BR_EDR_NOT_SUPPORTED | LE General Discoverable Mode
  std::string strServiceData = "";
  strServiceData += (char)0x07;  // Length = 7
  strServiceData += (char)0xff;  // AD Type 0xFF: Manufacturer specific data
  strServiceData += (char)0xff;  // Test manufacture ID low byte
  strServiceData += (char)0xff;  // Test manufacture ID high byte
  strServiceData += (char)0x01;
  strServiceData += (char)0x02;
  strServiceData += (char)0x03;
  strServiceData += (char)0x04;
  oAdvertisementData.addData(strServiceData);
  pAdvertising->setAdvertisementData(oAdvertisementData);
}

void setup() {
  M5.begin();
  BLEDevice::init("M5Stack");
  M5.Lcd.setTextSize(2);
  M5.Lcd.setCursor(0, 0);
  M5.Lcd.println("BLE Broadcast");
  BLEServer *pServer = BLEDevice::createServer();
  BLEAdvertising *pAdvertising = pServer->getAdvertising();
  setAdvData(pAdvertising);
  pAdvertising->start();
  Serial.println("Start advertising...");
  M5.Lcd.setCursor(0, 18);
  M5.Lcd.println("Start advertising...");
  delay(600000);
  pAdvertising->stop();
  Serial.println("End advertising...  ");
  M5.Lcd.setCursor(0, 18);
  M5.Lcd.println("End advertising...  ");
}

void loop() {}