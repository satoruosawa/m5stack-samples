#include "./callback.hpp"

void Callback::Process(int value) {
  value_ = value;
  Serial.print("Process :: value = ");
  Serial.println(value);
}
int Callback::value_ = 0;
int Callback::Value() { return value_; }
