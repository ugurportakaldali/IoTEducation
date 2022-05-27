#ifndef _config_h
#define _config_h

#define DEBUG_MODE

#define DHT_TYPE    DHT22
#define DHT_PIN     10
#define LED_Green   9
#define LED_Yellow  8
#define LED_Red     7
#define Buzzer_Pin  6
#define GPS_RX      5
#define GPS_TX      4
#define ESP_RX      3
#define ESP_TX      2

unsigned long TEMPMaxError   = 35;
unsigned long TEMPMaxWarning = 30;
unsigned long TEMPMinWarning = 15;
unsigned long TEMPMinError   = 10;

unsigned long HUMMaxError    = 80;
unsigned long HUMMaxWarning  = 60;
unsigned long HUMMinWarning  = 40;
unsigned long HUMMinError    = 30;

unsigned long validGPSAge    = 60000;
unsigned long lastLoopTime   = 2000;
unsigned long loopTime       = 2000;
unsigned long lastErrorTime  = 500;
unsigned long errorTime      = 500;
int deviceHealth = 1;
float t, h;

#endif
