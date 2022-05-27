#include "DHT.h"
#include "_config.h"
#include <TinyGPSPlus.h>
#include <SoftwareSerial.h>

SoftwareSerial gpsSerial(GPS_RX, GPS_TX);
SoftwareSerial espSerial(ESP_RX, ESP_TX);
DHT dht(DHT_PIN, DHT_TYPE);
TinyGPSPlus gps;


#ifdef DEBUG_MODE
#define DEBUG_PRINT(x)    Serial.print(x)
#define DEBUG_PRINTLN(x)  Serial.println (x)
#else
#define DEBUG_PRINT(x)
#define DEBUG_PRINTLN(x)
#endif
