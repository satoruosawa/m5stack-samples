http://opensoundcontrol.org/spec-1_0
OSC Type Tag	Type of corresponding argument
- i:	int32  
- f:	float32  
- s:	OSC-string  
- b:	OSC-blob  

OSC Type Tags that must be used for certain nonstandard argument types  
OSC Type Tag	Type of corresponding argument  
- h:	64 bit big-endian two's complement integer  
- t:	OSC-timetag  
- d:	64 bit ("double") IEEE 754 floating point number  
- S:	Alternate type represented as an OSC-string (for example, for systems that differentiate "symbols" from "strings")  
- c:	an ascii character, sent as 32 bits  
- r:	32 bit RGBA color  
- m:	4 byte MIDI message. Bytes from MSB to LSB are: port id, status byte, data1, data2  
- T:	True. No bytes are allocated in the argument data.  
- F:	False. No bytes are allocated in the argument data.  
- N:	Nil. No bytes are allocated in the argument data.  
- I:	Infinitum. No bytes are allocated in the argument data.  
- [:	Indicates the beginning of an array. The tags following are for data in the Array until a close brace tag is reached.  
- ]:	Indicates the end of an array.

https://github.com/sojamo/oscp5/blob/master/src/main/java/oscP5/OscMessage.java
