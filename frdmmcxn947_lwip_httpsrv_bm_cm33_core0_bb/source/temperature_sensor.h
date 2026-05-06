#ifndef TEMPERATURE_SENSOR_H
#define TEMPERATURE_SENSOR_H

#include <stdint.h>

void TempSensor_Init(void);
float TempSensor_ReadCelsius(void);
uint32_t TempSensor_ReadMilliVolts(void);

#endif
