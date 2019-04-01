#include <Ethernet2.h>
#include <EthernetUdp2.h>
#include <M5Stack.h>
#include <OSCBundle.h>
#include <OSCMessage.h>
#include <SPI.h>

#define SCK 18
#define MISO 19
#define MOSI 23
#define CS 26

EthernetUDP Udp;

// Adjust for each Device.
byte macAddress[] = { 0x2C, 0xF7, 0xF1, 0x08, 0x00, 0x9A };
IPAddress localIp(169, 254, 148, 24);
IPAddress localDns(169, 254, 148, 1);
IPAddress localGateway(169, 254, 148, 1);
IPAddress localSubnet(255, 255, 0, 0);
const unsigned int localPort = 8000;

void action(OSCMessage &msg, int addrOffset) {
  M5.Lcd.fillRect(0, 20, 100, 20, BLACK);
  M5.Lcd.setTextColor(WHITE);
  M5.Lcd.setTextSize(1);
  M5.Lcd.setCursor(0, 20);
  M5.Lcd.print("Get ");
  M5.Lcd.println(msg.getInt(0));
}

void setup() {
  M5.begin(true, false, true);

  SPI.begin(SCK, MISO, MOSI, -1);
  Ethernet.init(CS);
  Ethernet.begin(macAddress, localIp, localDns, localGateway, localSubnet);
  Udp.begin(localPort);

  Serial.begin(115200);
  delay(1000);

  Serial.print("The local IP address is: ");
  Serial.println(Ethernet.localIP());
  Serial.print("The gateway IP address is: ");
  Serial.println(Ethernet.gatewayIP());
  Serial.print("The DNS server IP address is: ");
  Serial.println(Ethernet.dnsServerIP());
  Serial.print("The subnet mask is: ");
  Serial.println(Ethernet.subnetMask());

  M5.Lcd.println("M5Stack W5500 Test");
}

void loop() {
  OSCBundle bundleIN;
  int size = Udp.parsePacket();

  if (size > 0) {
    while(size--) bundleIN.fill(Udp.read());
    if (!bundleIN.hasError()) bundleIN.route("/testOsc", action);
  }
}
