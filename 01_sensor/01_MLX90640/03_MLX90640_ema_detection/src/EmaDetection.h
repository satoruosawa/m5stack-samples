#ifndef EMA_DETECTION_H_
#define EMA_DETECTION_H_

#include "./library/MLX90640_API.h"
#include "./library/MLX90640_I2C_Driver.h"
#include "./Modules.h"

#define TA_SHIFT 8 //Default shift for MLX90640 in open air
#define X_COUNT 32
#define Y_COUNT 24
#define VALUE_COUNT 768 // 32 * 24

class EmaDetection {
public:
  EmaDetection ();
  void Setup ();
  void Update ();
  void Draw ();

private:
  void InitiateValues ();
  void UpdateRaw ();
  void UpdateRef ();
  void UpdateSample ();
  void UpdateDiff ();
  boolean isConnected ();
  const byte mlx90640Address;
  float * rawValues;
  double * refEmaValues;
  double * sampleEmaValues;
  double * diffValues;
  const double refEmaCoef;
  const double sampleEmaCoef;
  paramsMLX90640 mlx90640;
  int diffMinIndex;
  int diffMaxIndex;
};

#endif EMA_DETECTION_H_
