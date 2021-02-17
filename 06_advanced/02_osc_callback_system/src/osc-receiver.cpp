#include "./osc-receiver.hpp"

OscReceiver::OscReceiver(OscHandler *osc_handler) : osc_handler_(osc_handler) {}

void OscReceiver::Setup() {
  std::string key("/sensor");
  Serial.println("OscReceiver::Setup");
  osc_handler_->AddCallback(key, ReceiverCallback);
}

void OscReceiver::ReceiverCallback(OSCMessage &msg, int addr_offset) {
  index_ = msg.getInt(0);
  value_ = msg.getFloat(1);
  // Serial.print("Receive osc!! ");
  // Serial.print("index: ");
  // Serial.print(index_);
  // Serial.print(" value: ");
  // Serial.println(value_);
}

int OscReceiver::index_ = 0;
float OscReceiver::value_ = 0;

int OscReceiver::Index() { return index_; }
float OscReceiver::Value() { return value_; }
