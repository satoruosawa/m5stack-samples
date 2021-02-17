#ifndef OSC_RECEIVER_HPP_
#define OSC_RECEIVER_HPP_

#include "./osc-handler.hpp"

class OscReceiver {
 public:
  OscReceiver(OscHandler *osc_handler);
  void Setup();
  int Index();
  float Value();

 private:
  static void ReceiverCallback(OSCMessage &msg, int addr_offset);
  OscHandler *osc_handler_;
  static int index_;
  static float value_;
};

#endif  // OSC_RECEIVER_HPP_
