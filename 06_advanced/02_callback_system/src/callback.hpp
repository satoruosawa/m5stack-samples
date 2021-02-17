#ifndef CALLBACK_HPP_
#define CALLBACK_HPP_

#include <M5Stack.h>

#include "./callback-handler.hpp"

class Callback {
 public:
  Callback() = default;
  int Value();
  static void Process(int value);

 private:
  static int value_;
};

#endif  // CALLBACK_HPP_
