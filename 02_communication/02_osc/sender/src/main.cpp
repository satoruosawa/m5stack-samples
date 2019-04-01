#include <Ethernet2.h>
#include <EthernetUdp2.h>
#include <M5Stack.h>
#include <OSCBundle.h>
#include <OSCMessage.h>
#include <SPI.h>

#define CS 26

EthernetUDP Udp;
OSCBundle bundleOut;

// Adjust for each Device.
byte macAddress[] = { 0x84, 0x0D, 0x8E, 0x3D, 0x39, 0xA7 }; // 84:0D:8E:3D:39:A7
IPAddress localIp(169, 254, 148, 27);
IPAddress localDns(169, 254, 148, 1);
IPAddress localGateway(169, 254, 148, 1);
IPAddress localSubnet(255, 255, 0, 0);
unsigned int localPort = 8888;

IPAddress targetIp(169, 254, 148, 24);
const unsigned int targetPort = 8000;

int32_t sendData = 0;

void setup() {
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
}

void loop() {
  bundleOut.add("/testOsc").add(sendData);
  Udp.beginPacket(targetIp, targetPort);
  bundleOut.setTimetag(oscTime());
  bundleOut.send(Udp);
  Udp.endPacket();
  bundleOut.empty();
  sendData++;
  delay(100);
}
