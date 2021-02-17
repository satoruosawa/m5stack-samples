#ifndef EMA_H_
#define EMA_H_

#include "./Modules.h"
#include "./library/MLX90640_API.h"
#include "./library/MLX90640_I2C_Driver.h"

#define TA_SHIFT 8  // Default shift for MLX90640 in open air
#define X_COUNT 32
#define Y_COUNT 24
#define VALUE_COUNT 768  // 32 * 24

class Ema {
 public:
  Ema();
  void Setup();
  void Update();
  void Draw();

 private:
  void UpdateRaw();
  void UpdateEma();
  boolean isConnected();
  const double coef;
  const byte mlx90640Address;
  float* rawValues;
  double* emaValues;
  paramsMLX90640 mlx90640;
};

#endif
