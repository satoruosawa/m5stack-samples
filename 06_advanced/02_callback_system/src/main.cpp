#include <M5Stack.h>

#include "./callback-handler.hpp"
#include "./callback.hpp"

CallbackHandler CALLBACK_HANDLER;

void setup() {
  M5.begin();
  Callback callback;
  CALLBACK_HANDLER.AddCallback(callback.Process);
}

void loop() {
  CALLBACK_HANDLER.Update();
  delay(1000);
}