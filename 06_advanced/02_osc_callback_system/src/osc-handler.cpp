#include "./osc-handler.hpp"

void OscHandler::Setup() {
  SPI.begin(ESP_SCK, ESP_MISO, ESP_MOSI, -1);
  Ethernet.init(ESP_CS);
  byte local_mac_address[6] = {0x2C, 0xF7, 0xF1, 0x08, 0x00, 0x9A};
  IPAddress local_ip = IPAddress(169, 254, 148, 20);
  IPAddress local_dns = IPAddress(169, 254, 148, 20);
  IPAddress local_gateway = IPAddress(169, 254, 148, 20);
  IPAddress local_subnet = IPAddress(169, 254, 148, 20);
  int local_port = 12000;
  Ethernet.begin(local_mac_address, local_ip, local_dns, local_gateway,
                 local_subnet);
  udp_.begin(local_port);
}

void OscHandler::AddCallback(std::string key,
                             void (*callback)(OSCMessage&, int)) {
  callbacks_[key] = callback;
}

void OscHandler::CheckReceiveOsc() {
  OSCBundle osc_bundle;
  int size = udp_.parsePacket();

  if (size > 0) {
    while (size--) {
      osc_bundle.fill(udp_.read());
    }
    if (!osc_bundle.hasError()) {
      for (auto itr = callbacks_.begin(); itr != callbacks_.end(); ++itr) {
        osc_bundle.route(itr->first.c_str(), itr->second);
      }
    }
  }
}
