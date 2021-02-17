#ifndef OSC_MANAGER_HPP_
#define OSC_MANAGER_HPP_

#include "./osc-handler.hpp"
#include "./osc-receiver.hpp"

class OscManager {
 public:
  OscManager() = default;
  void Setup();
  void Update();
  int Index();
  float Value();

 private:
  OscHandler osc_handler_;
  OscReceiver* osc_receiver_;
};

#endif  // OSC_MANAGER_HPP_
