#ifndef MODULES_H_
#define MODULES_H_

#include <M5Stack.h>

uint16_t GetColor (uint8_t red, uint8_t green, uint8_t blue);

float FloatMap (
  float value, float minFrom, float maxFrom, float minTo, float maxTo);

double DoubleLerp (double x0, double y0, double x1, double y1, double x);

int RotatePixelIndex (int index, int size, bool isRight = true);

int FlipPixelIndex (int index, int size, bool isHorizontal = true);

#endif MODULES_H_
