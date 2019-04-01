#include "./Modules.h"

uint16_t GetColor (uint8_t red, uint8_t green, uint8_t blue) {
  return ((red >> 3) << 11) | ((green >> 2) << 5) | (blue >> 3);
}

float FloatMap (
  float value, float minFrom, float maxFrom, float minTo, float maxTo) {
  float slope = (maxTo - minTo) / (maxFrom - minFrom);
  return (value - minFrom) * slope + minTo;
}

double DoubleLerp (double x0, double y0, double x1, double y1, double x) {
  return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
}

int RotatePixelIndex (int index, int size, bool isRight) {
  // It must be a square matrix;
  // the "size" should be row or column size.
  if (index >= size * size) return 0;
  int column = index % size;
  int row = index / size;
  if (isRight) {
    return row + (7 - column) * size;
  } else {
    return (7 - row) + column * size;
  }
}

int FlipPixelIndex (int index, int size, bool isHorizontal) {
  // It must be a square matrix;
  // the "size" should be row or column size.
  if (index >= size * size) return 0;
  int column = index % size;
  int row = index / size;
  if (isHorizontal) {
    return column + (7 - row) * size;
  } else {
    return (7 - column) + row * size;
  }
}
