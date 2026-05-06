/*
 * Temperature sensor module for external NTC on ADC0 channel 0
 */

#include <stdio.h>
#include <math.h>

#include "board.h"
#include "pin_mux.h"
#include "clock_config.h"
#include "fsl_lpadc.h"
#include "fsl_spc.h"
#include "fsl_port.h"

#include "temperature_sensor.h"

#define ADC_CHANNEL_NUMBER   0U
#define ADC_COMMAND_ID       1U
#define ADC_TRIGGER_ID       0U
#define ADC_VREF_MV          3300U
#define ADC_12BIT_MAX        4095U

#define NTC_R0               10000.0f
#define NTC_BETA             3950.0f
#define NTC_T0_K             298.15f
#define SERIES_RESISTOR      100.0f

static void ADC_PinInit(void)
{
    /* A0 / ADC0_A0 - seteaza pinul ca analogic */
    CLOCK_EnableClock(kCLOCK_Port1);

    const port_pin_config_t portConfig = {
        kPORT_PullDisable,
        kPORT_FastSlewRate,
        kPORT_PassiveFilterDisable,
        kPORT_OpenDrainDisable,
        kPORT_LowDriveStrength,
        kPORT_MuxAlt0,
        kPORT_UnlockRegister
    };

    PORT_SetPinConfig(PORT1, 8U, &portConfig);
}

static void ADC_Init(void)
{
    lpadc_config_t adcConfig;
    lpadc_conv_command_config_t cmdConfig;
    lpadc_conv_trigger_config_t trigConfig;

    SPC_EnableActiveModeAnalogModules(SPC0, kSPC_controlVref);

    CLOCK_AttachClk(kFRO_HF_to_ADC0);
    CLOCK_SetClkDiv(kCLOCK_DivAdc0Clk, 1U);
    CLOCK_EnableClock(kCLOCK_Adc0);

    LPADC_GetDefaultConfig(&adcConfig);
    adcConfig.referenceVoltageSource = kLPADC_ReferenceVoltageAlt2;
    LPADC_Init(ADC0, &adcConfig);
    LPADC_DoAutoCalibration(ADC0);

    LPADC_GetDefaultConvCommandConfig(&cmdConfig);
    cmdConfig.channelNumber = ADC_CHANNEL_NUMBER;
    cmdConfig.sampleChannelMode = kLPADC_SampleChannelSingleEndSideA;
    cmdConfig.conversionResolutionMode = kLPADC_ConversionResolutionStandard;
    LPADC_SetConvCommandConfig(ADC0, ADC_COMMAND_ID, &cmdConfig);

    LPADC_GetDefaultConvTriggerConfig(&trigConfig);
    trigConfig.targetCommandId = ADC_COMMAND_ID;
    trigConfig.enableHardwareTrigger = false;
    LPADC_SetConvTriggerConfig(ADC0, ADC_TRIGGER_ID, &trigConfig);
}

static uint32_t ADC_ReadRaw(void)
{
    lpadc_conv_result_t result;

    LPADC_DoSoftwareTrigger(ADC0, 1U);

    while (!LPADC_GetConvResult(ADC0, &result, 0U))
    {
    }

    return (result.convValue >> 3U);
}

static uint32_t ADC_ReadAverage(uint8_t samples)
{
    uint32_t sum = 0;
    uint8_t i;

    for (i = 0; i < samples; i++)
    {
        sum += ADC_ReadRaw();
    }

    return sum / samples;
}

static float ADC_ToVoltage(uint32_t rawValue)
{
    return ((float)rawValue * 3.3f) / 4095.0f;
}

static float NTC_ToTemperatureC(uint32_t rawValue)
{
    float vOut;
    float rNtc;
    float tempK;
    float tempC;

    if (rawValue == 0U || rawValue >= ADC_12BIT_MAX)
    {
        return -999.0f;
    }

    vOut = ADC_ToVoltage(rawValue);

    if (vOut <= 0.0001f || vOut >= 3.299f)
    {
        return -999.0f;
    }

    rNtc = SERIES_RESISTOR * ((3.3f - vOut) / vOut);

    tempK = 1.0f / ((1.0f / NTC_T0_K) + (logf(rNtc / NTC_R0) / NTC_BETA));
    tempC = tempK - 273.15f;

    return tempC;
}

void TempSensor_Init(void)
{
    ADC_PinInit();
    ADC_Init();
}

float TempSensor_ReadCelsius(void)
{
    uint32_t rawValue = ADC_ReadAverage(16);
    return NTC_ToTemperatureC(rawValue);
}

uint32_t TempSensor_ReadMilliVolts(void)
{
    uint32_t rawValue = ADC_ReadAverage(16);
    return (rawValue * ADC_VREF_MV) / ADC_12BIT_MAX;
}
