#include "./osc-manager.hpp"

void OscManager::Setup() {
  osc_receiver_ = new OscReceiver(&osc_handler_);
  osc_handler_.Setup();
  osc_receiver_->Setup();
}

void OscManager::Update() { osc_handler_.CheckReceiveOsc(); }

int OscManager::Index() { return osc_receiver_->Index(); }
float OscManager::Value() { return osc_receiver_->Value(); }
