#include <M5Stack.h>
#include <TinyGPS++.h>

HardwareSerial GPS_s(2);
TinyGPSPlus gps;

void setup() {
  M5.begin();
  GPS_s.begin(9600);
}

void loop() {
  while (!gps.location.isUpdated()) {
    while (GPS_s.available() > 0) {
      if (gps.encode(GPS_s.read())) {
        break;
      }
    }
  }

  Serial.println("==============================");
  Serial.printf("lat: %f, lng: %f\n", gps.location.lat(), gps.location.lng());

  // Serial.println(gps.location.lat(), 6);  // Latitude in degrees (double)
  // Serial.println(gps.location.lng(), 6);  // Longitude in degrees (double)
  // Serial.print(gps.location.rawLat().negative ? "-" : "+");
  // Serial.println(gps.location.rawLat().deg);  // Raw latitude in whole
  // degrees Serial.println(
  //     gps.location.rawLat().billionths);  // ... and billionths (u16/u32)
  // Serial.print(gps.location.rawLng().negative ? "-" : "+");
  // Serial.println(gps.location.rawLng().deg);  // Raw longitude in whole
  // degrees Serial.println(
  //     gps.location.rawLng().billionths);  // ... and billionths (u16/u32)

  Serial.printf("Raw date in DDMMYY format (u32) :%i\n", gps.date.value());
  // Serial.println(gps.date.value());  // Raw date in DDMMYY format (u32)
  // Serial.println(gps.date.year());         // Year (2000+) (u16)
  // Serial.println(gps.date.month());        // Month (1-12) (u8)
  // Serial.println(gps.date.day());          // Day (1-31) (u8)

  Serial.printf("Raw time in HHMMSSCC format :%i\n", gps.time.value());
  // Serial.println(gps.time.value());  // Raw time in HHMMSSCC format
  // (u32) Serial.println(gps.time.hour());         // Hour (0-23) (u8)
  // Serial.println(gps.time.minute());       // Minute (0-59) (u8)
  // Serial.println(gps.time.second());       // Second (0-59) (u8)
  // Serial.println(gps.time.centisecond());  // 100ths of a second (0-99) (u8)

  Serial.printf("Current ground speed (double) :%lf[m/s]\n", gps.speed.mps());
  // Serial.println(gps.speed.value());  // Raw speed in 100ths of a knot (i32)
  // Serial.println(gps.speed.knots());  // Speed in knots (double)
  // Serial.println(gps.speed.mph());    // Speed in miles per hour (double)
  // Serial.println(gps.speed.mps());   // Speed in meters per second (double)
  // Serial.println(gps.speed.kmph());  // Speed in kilometers per hour (double)

  Serial.printf("Course in degrees (double) :%lf\n", gps.course.deg());
  // Serial.println(
  //   gps.course.value());  // Raw course in 100ths of a degree (i32)
  // Serial.println(gps.course.deg());    // Course in degrees (double)

  Serial.printf("Altitude (double): %f[m]\n", gps.altitude.meters());
  // Serial.println(gps.altitude.value());   // Raw altitude in centimeters
  // (i32) Serial.println(gps.altitude.meters());  // Altitude in meters
  // (double) Serial.println(gps.altitude.miles());   // Altitude in miles
  // (double) Serial.println(gps.altitude.kilometers());  // Altitude in
  // kilometers (double) Serial.println(gps.altitude.feet());        // Altitude
  // in feet (double)

  Serial.printf("The number of visible, participating satellites: %i\n",
                gps.satellites.value());
  // Serial.println(gps.satellites.value());  // Number of satellites in use
  // (u32)

  Serial.printf("Horizontal diminution of precision (100ths-i32): %i\n",
                gps.hdop.value());
  // Serial.println(
  //     gps.hdop.value());  // Horizontal Dim. of Precision (100ths-i32)
}