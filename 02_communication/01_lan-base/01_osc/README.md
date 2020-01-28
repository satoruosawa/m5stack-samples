## How to
- Check sample
https://github.com/m5stack/M5Stack/blob/master/examples/Modules/W5500/WebServer/WebServer.ino
Web Server
Need to install Ethernet2 arduino library
If new arduino esp32 or make error, need go to c:\Program Files (x86)\Arduino\hardware\espressif\arduino-esp32\cores\esp32\Server.h
Change virtual void begin(uint16_t port = 0) = 0; to virtual void begin() = 0;
Other example can see https://github.com/adafruit/Ethernet2

- Get Mac address
from tet04

## References
- https://qiita.com/kazzy/items/8b7cadd4303b34f6321f
- https://www.switch-science.com/catalog/3994/
- https://github.com/m5stack/M5Stack/blob/master/examples/Modules/W5500/WebServer/WebServer.ino
- https://www.arduino.cc/en/Reference/EthernetBegin
- https://www.mgo-tec.com/blog-entry-chip-info-esp-wroom-32-esp32.html
