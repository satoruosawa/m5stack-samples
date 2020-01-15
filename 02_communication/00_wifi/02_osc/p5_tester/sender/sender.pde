import netP5.*;
import oscP5.*;

OscP5 oscP5;

NetAddress myRemoteLocation;

void setup() {
  oscP5 = new OscP5(this,12000);
  myRemoteLocation = new NetAddress("192.168.8.126", 10000);
}

void draw() {}

int count = 0;

void mousePressed() {
  println("send.");
  count++;
  float f = 34.56F;
  double d = 78.987;
  String s = "hello from processing";
  boolean b = true;
  OscMessage msg = new OscMessage("/test");
  msg.add(count);
  msg.add(f);
  msg.add(d);
  msg.add(s);
  msg.add(b);
  oscP5.send(msg, myRemoteLocation);
}
