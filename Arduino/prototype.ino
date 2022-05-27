#include "_config.h"
#include "_secret.h"
#include "_includes.h"

void setup() {
  Serial.begin(9600);
  DEBUG_PRINTLN(F("DEBUG -> Setup starting..."));

  pinMode(LED_Red, OUTPUT);
  pinMode(LED_Green, OUTPUT);
  pinMode(LED_Yellow, OUTPUT);
  pinMode(Buzzer_Pin, OUTPUT);

  digitalWrite(LED_Red, LOW);
  digitalWrite(LED_Green, LOW);
  digitalWrite(LED_Yellow, LOW);
  digitalWrite(Buzzer_Pin, LOW);

  dht.begin();
  gpsSerial.begin(9600);
  espSerial.begin(115200);
  espSerial.listen();

  SendCommandTillSuccess(F("AT"), F("OK"));
  String response = GetCommandResponse(F("AT+CWMODE?"));
  if (response.indexOf("CWMODE:1") == -1) {
    SendCommandTillSuccess(F("AT+CWMODE=1"), F("OK"));
  }

  response = GetCommandResponse(F("AT+CWJAP?"));
  if (response.indexOf("No AP") == -1) {
    SendCommandTillSuccess(String("AT+CWJAP=\"") + WIFI_SSID + String("\",\"") + WIFI_PASSWORD + String("\""), F("OK"));
  }

  DEBUG_PRINTLN(F("DEBUG -> Setup completed!"));
  gpsSerial.listen();
}

void loop() {
  while (gpsSerial.available()) {
    gps.encode(gpsSerial.read());
  }

  if (deviceHealth == 3) {
    if (millis() - lastErrorTime > errorTime) {
      digitalWrite(Buzzer_Pin, HIGH);
      delay(100);
      digitalWrite(Buzzer_Pin, LOW);
      lastErrorTime = millis();
    }
  }

  if (millis() - lastLoopTime > loopTime) {
    DEBUG_PRINTLN(F("DEBUG -> Loop time!"));
    if (DHTData() && GPSData()) {
      String payload = PackageData();

      espSerial.listen();

      if (GetCommandResponse(F("AT+CIPSTATUS")).indexOf("STATUS:3") == -1) {
        SendCommandTillSuccess(String("AT+CIPSTART=\"TCP\",\"") + SERVER_IP + String("\",") + SERVER_PORT, "OK");
      }
      SendCommand(String("AT+CIPSEND=") + (payload.length() + 2 ), "OK");
      SendCommandTillSuccess(payload, F("SEND OK"));
      SendCommandTillSuccess(F("AT+CIPCLOSE"), F("OK"));
      gpsSerial.listen();
    }
    lastLoopTime = millis();
  }
}

bool DHTData() {
  h = dht.readHumidity();
  t = dht.readTemperature();

  if (isnan(h) || isnan(t)) {
    DEBUG_PRINTLN(F("DEBUG -> No DHT data !"));
    return false;
  }

  deviceHealth = 1;
  digitalWrite(LED_Red, LOW);
  digitalWrite(LED_Green, LOW);
  digitalWrite(LED_Yellow, LOW);
  digitalWrite(Buzzer_Pin, LOW);

  if (t < TEMPMinError ||
      t > TEMPMaxError ||
      h < HUMMinError ||
      h > HUMMaxError ) {
    DEBUG_PRINTLN(F("DEBUG -> Critical environmental condition !"));
    digitalWrite(LED_Red, HIGH);
    deviceHealth = 3;
  }
  else if (t < TEMPMinWarning ||
           t > TEMPMaxWarning ||
           h < HUMMinWarning ||
           h > HUMMaxWarning) {
    DEBUG_PRINTLN(F("DEBUG -> Warning environmental condition !"));
    digitalWrite(LED_Yellow, HIGH);
    deviceHealth = 2;
  }
  else {
    DEBUG_PRINTLN(F("DEBUG -> Normal environmental condition !"));
    digitalWrite(LED_Green, HIGH);
  }

  DEBUG_PRINT(F("DEBUG -> Humidity: "));
  DEBUG_PRINT(h);
  DEBUG_PRINT(F("%  Temperature: "));
  DEBUG_PRINT(t);
  DEBUG_PRINTLN(F("°C "));

  return true;
}

bool GPSData() {

  if (!gps.location.isValid() ||  !gps.date.isValid() ||  !gps.time.isValid()) {
    DEBUG_PRINTLN(F("DEBUG -> No GPS data !"));
    return false;
  }

  if (gps.location.age() > validGPSAge) {
    DEBUG_PRINTLN(F("DEBUG -> GPS data age violation!"));
    return false;
  }

  DEBUG_PRINT(F("DEBUG -> Location: "));
  DEBUG_PRINT(gps.location.lat());
  DEBUG_PRINT(F(","));
  DEBUG_PRINT(gps.location.lng());
  DEBUG_PRINT(F("  Date/Time: "));
  DEBUG_PRINT(gps.date.month());
  DEBUG_PRINT(F("/"));
  DEBUG_PRINT(gps.date.day());
  DEBUG_PRINT(F("/"));
  DEBUG_PRINT(gps.date.year());
  DEBUG_PRINT(F(" "));
  DEBUG_PRINT(gps.time.hour());
  DEBUG_PRINT(F(":"));
  DEBUG_PRINT(gps.time.minute());
  DEBUG_PRINT(F(":"));
  DEBUG_PRINT(gps.time.second());
  DEBUG_PRINT(F("."));
  DEBUG_PRINTLN(gps.time.centisecond());

  return true;
}

String PackageData() {
  String payload = "#";
  payload += String(DEVICE_CODE) + "~";

  if (gps.date.day() < 10) payload += "0";
  payload += String(gps.date.day()) ;

  if (gps.date.month() < 10) payload += "0";
  payload += String(gps.date.month()) ;

  payload += String(gps.date.year());

  if (gps.time.hour() < 10) payload += "0";
  payload += String(gps.time.hour());

  if (gps.time.minute() < 10) payload += "0";
  payload += String(gps.time.minute());

  if (gps.time.second() < 10) payload += "0";
  payload += String(gps.time.second()) + "~";

  payload += String(gps.location.lat(), 6) + "~";
  payload += String(gps.location.lng(), 6) + "~";

  payload += String(h) + "~";
  payload += String(t) + "~";
  payload += String(deviceHealth) + "~";
  
  payload += String(SECRET_KEY * DEVICE_ID - gps.time.second()) + "#";

  DEBUG_PRINT(F("DEBUG -> Payload : "));
  DEBUG_PRINTLN(payload);

  return payload;
}

void SendCommandTillSuccess(String command, String expectedRespone) {
  while (true) {
    if (SendCommand(command, expectedRespone)) {
      break;
    }
    delay(500);
  }
}

bool SendCommand(String command, String expectedRespone) {
  String espResponse = GetCommandResponse(command);
  bool operationStatus = espResponse.indexOf(expectedRespone) != -1;

  DEBUG_PRINT(F("Status : "));
  DEBUG_PRINT(operationStatus);
  DEBUG_PRINT(F("  Command : "));
  DEBUG_PRINTLN(command);

  DEBUG_PRINTLN(F("Response : "));
  DEBUG_PRINTLN(espResponse);
  if (espResponse.indexOf("ERROR") != -1) {
    DEBUG_PRINT(F("ESP has error"));
    return true;
  }
  DEBUG_PRINTLN(F("____________________________________________"));

  return operationStatus;
}

String GetCommandResponse(String cmd) {
  espSerial.print(cmd);
  espSerial.print("\r\n");
  return espSerial.readString();
}
