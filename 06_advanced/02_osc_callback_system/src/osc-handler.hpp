// MIT License
//
// Copyright (c) 2019 Satoru Osawa
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#ifndef OSC_HANDLER_HPP_
#define OSC_HANDLER_HPP_

#include <Ethernet2.h>
#include <EthernetUdp2.h>
#include <M5Stack.h>
#include <OSCBundle.h>
#include <OSCMessage.h>
#include <SPI.h>

#include <map>

#define ESP_SCK 18
#define ESP_MISO 19
#define ESP_MOSI 23
#define ESP_CS 26

class OscHandler {
 public:
  OscHandler() = default;
  void Setup();
  void CheckReceiveOsc();
  void AddCallback(std::string key, void (*callback)(OSCMessage&, int));

 private:
  EthernetUDP udp_;
  std::map<std::string, void (*)(OSCMessage&, int)> callbacks_;
};

#endif  // OSC_HANDLER_HPP_
