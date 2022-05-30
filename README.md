# IoT Eğitimi

Bu proje, eğitmenliğini üstlendiğim, 10 hafta süren eğitim kapsamında yapılan çalışmaları içermektedir.

Eğitim süresince IoT portalı oluşturmak hedeflendi ve bu hedef doğrultusunda 4 farklı disiplin ele alındı :
- Arduino Programlama
- Server Yazılımı
- PCB Tasarımı
- 3D Tasarım

## Proje Yapısı

[Web](https://github.com/ugurportakaldali/IoTEducation/tree/main/Web/IoTEducation/IoTEducation.Web) projesi,
sistemde bulunan cihazların yönetimini ve cihazdan gelen verilerin incelenebildiği temel bir WebAssembly yazılımıdır. 
Verileri [API](https://github.com/ugurportakaldali/IoTEducation/tree/main/Web/IoTEducation/IoTEducation.API) projesinden sağlamaktadır.

API projesinin veri tabanı bağlantısı yapılandırma dosyası üzerinden belirlenmektedir.
```json
  "ConnectionStrings": {
    "DataConnection": "Server=.;Initial Catalog=IoTEducation;User ID=suIoT;Password=IoT2022"
  }
```

Donanım tarafında hızlı prototipleme adına Arduino kartı kullanıldı. 
[Arduino](https://github.com/ugurportakaldali/IoTEducation/tree/main/Arduino), donanım tarafındaki yazılımı içermektedir.

Server tarafı ile yapılacak iletişim yapılandırma dosyası üzerinden belirlenmektedir.
```c
#define WIFI_SSID     "****"
#define WIFI_PASSWORD "****"

#define SERVER_IP    "192.168.0.104"
#define SERVER_PORT  14000
```

Donanım tarafından veriler [TCPServer](https://github.com/ugurportakaldali/IoTEducation/tree/main/Web/IoTEducation/IoTEducation.TCPServer) projesi tarafından karşılanmaktadır.

Bu proje, yapılandırma dosyasında belirtilen adreste port dinlemesi yapmaktadır.
```json
  "IPAddress": "192.168.0.104",
  "Port": "14000"
```
