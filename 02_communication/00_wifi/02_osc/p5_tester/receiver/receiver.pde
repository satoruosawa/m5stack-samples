import netP5.*;
import oscP5.*;

OscP5 oscP5;

void setup() {
  oscP5 = new OscP5(this, 10000);
  println("start.");
}

void draw() {}

void oscEvent(OscMessage msg) {
  println("received.");
  if (msg.checkAddrPattern("/test")) {
    println("address /test");
    print("msg.typetag(): ");
    println(msg.typetag());
    print("msg.get(0).intValue(): ");
    println(msg.get(0).intValue());
    print("msg.get(1).floatValue(): ");
    println(msg.get(1).floatValue());
    print("msg.get(2).doubleValue(): ");
    println(msg.get(2).doubleValue());
    print("msg.get(3).stringValue(): ");
    println(msg.get(3).stringValue());
    // print("msg.get(4).booleanValue(): ");
    // println(msg.get(4).booleanValue());
  }
  println();
}
