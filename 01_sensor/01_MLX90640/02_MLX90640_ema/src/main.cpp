#include "./Ema.h"
#include <M5Stack.h>
#include <Wire.h>

Ema * EMA;

void setup () {
  Serial.begin(115200);
  while (!Serial); //Wait for user to open terminal
  Serial.println("MLX90640 IR Array Example");
  EMA = new Ema();
  EMA->Setup();
  M5.begin();
}

void loop () {
  long startTime = millis();
  EMA->Update();
  long stopReadTime = millis();
  EMA->Draw();
  long stopPrintTime = millis();

  Serial.print("Read rate: ");
  Serial.print( 1000.0 / (stopReadTime - startTime), 2);
  Serial.println(" Hz");
  Serial.print("Read plus print rate: ");
  Serial.print( 1000.0 / (stopPrintTime - startTime), 2);
  Serial.println(" Hz");
}
