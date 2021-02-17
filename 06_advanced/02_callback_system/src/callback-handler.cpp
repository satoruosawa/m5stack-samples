#include "./callback-handler.hpp"

void CallbackHandler::AddCallback(void (*callback)(int)) {
  callbacks_.push_back(callback);
}

void CallbackHandler::Update() {
  static int count = 0;
  for (auto& callback : callbacks_) {
    callback(count);
  }
  count++;
}
