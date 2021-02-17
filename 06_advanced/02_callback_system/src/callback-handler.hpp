#ifndef CALLBACK_HANDLER_HPP_
#define CALLBACK_HANDLER_HPP_

#include <vector>

class CallbackHandler {
 public:
  CallbackHandler() = default;
  void Setup();
  void Update();
  void AddCallback(void (*callback)(int));

 private:
  std::vector<void (*)(int)> callbacks_;
};

#endif  // CALLBACK_HANDLER_HPP_
