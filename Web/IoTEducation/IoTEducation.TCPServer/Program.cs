using BeetleX;
using BeetleX.EventArgs;
using IoTEducation.DataLayer;
using IoTEducation.DataLayer.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IoTEducation.TCPServer
{
    class Program : ServerHandlerBase
    {
        #region PROPERTIES   
        private int secretKey = 972635092;
        private static DalDevice dalDevice;
        private static DalDeviceData dalDeviceData;
        private static List<IotDevice> RegisteredDevices = new List<IotDevice>();

        private static IServer server;
        private static IConfiguration APPCONFIG;
        private static int PORT
        {
            get
            {
                return Convert.ToInt32(APPCONFIG["Port"]);
            }
        }
        private static int BUFFERSIZE
        {
            get
            {
                return Convert.ToInt32(APPCONFIG["BufferSize"]);
            }
        }
        private static string IPADDRESS
        {
            get
            {
                return APPCONFIG["IPAddress"];
            }
        }
        private static int MAXCONNECTIONS
        {
            get
            {
                return Convert.ToInt32(APPCONFIG["MaxConnections"]);
            }
        }
        private static int MAXACCEPTQUEUE
        {
            get
            {
                return Convert.ToInt32(APPCONFIG["MaxAcceptQueue"]);
            }
        }
        public static bool SHOWLOGCONSOLE
        {
            get
            {
                return APPCONFIG["ShowLogConsole"].ToString() == "1";
            }
        }
        private static int BUFFERPOOLMAXMEMORY
        {
            get
            {
                return Convert.ToInt32(APPCONFIG["BufferPoolMaxMemory"]);
            }
        }
        private enum DataStructure
        {
            DeviceCode = 0,
            DateTime = 1,
            Latitude = 2,
            Longitude = 3,
            Humidity = 4,
            Temperature = 5,
            Health = 6,
            Checksum = 7
        }
        #endregion

        static void Main(string[] args)
        {
            APPCONFIG = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            var context = new IoTEducationContext();
            dalDevice = new DalDevice(context);
            dalDeviceData = new DalDeviceData(context);

            var task = dalDevice.ListAsync();
            task.Wait();
            RegisteredDevices = task.Result;

            try
            {
                Console.WriteLine("=> " + IPADDRESS + ":" + PORT);

                ServerOptions ops = new ServerOptions();
                ops.BufferSize = BUFFERSIZE;
                ops.MaxConnections = MAXCONNECTIONS;
                ops.MaxAcceptQueue = MAXACCEPTQUEUE;
                ops.BufferPoolMaxMemory = BUFFERPOOLMAXMEMORY;
                ops.Listens.RemoveAt(0);
                ops.AddListen(IPADDRESS, PORT);
                server = SocketFactory.CreateTcpServer<Program>(ops);
                server.Open();

                Console.WriteLine("Press key 'q' to stop BeetleX!");
                while (Console.ReadKey().KeyChar != 'q')
                {
                    Console.WriteLine();
                    continue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #region TCPServer
        public override void SessionReceive(IServer server, SessionReceiveEventArgs e)
        {
            System.Net.IPEndPoint client = e.Session.RemoteEndPoint as System.Net.IPEndPoint;
            var pipeStream = e.Stream.ToPipeStream();
            bool isRead = false;
            string responseData = string.Empty;
            try
            {
                while (pipeStream.TryReadLine(out string receiveData))
                {
                    isRead = true;
                    Console.WriteLine(DateTime.Now + " => New Message  => (Client => " + client.Address.ToString() + ":" + client.Port.ToString() + ") : " + receiveData);
                    e.Session.Stream.ToPipeStream().WriteLine(ValidateRecivedData(receiveData));
                    e.Session.Stream.Flush();
                }
                if (!isRead)
                {
                    string receiveData = pipeStream.ReadToEnd();
                    if (!string.IsNullOrEmpty(receiveData))
                    {
                        Console.WriteLine(DateTime.Now + " => New Message  => (Client => " + client.Address.ToString() + ":" + client.Port.ToString() + ") : " + receiveData);
                        e.Session.Stream.ToPipeStream().WriteLine(ValidateRecivedData(receiveData));
                        e.Session.Stream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("New Message - " + client.Address.ToString() + " " + client.Port.ToString() + " : " + ex.ToString());
            }
            base.SessionReceive(server, e);
        }
        public override void Connected(IServer server, ConnectedEventArgs e)
        {
            if (SHOWLOGCONSOLE)
            {
                System.Net.IPEndPoint client = e.Session.RemoteEndPoint as System.Net.IPEndPoint;
                Console.WriteLine(DateTime.Now + " => Connected    => (Client => " + client.Address.ToString() + ":" + client.Port.ToString() + ")");
            }
            base.Connected(server, e);
        }
        public override void Disconnect(IServer server, SessionEventArgs e)
        {
            if (SHOWLOGCONSOLE)
            {
                System.Net.IPEndPoint client = e.Session.RemoteEndPoint as System.Net.IPEndPoint;
                Console.WriteLine(DateTime.Now + " => Disconnected => (Client => " + client.Address.ToString() + ":" + client.Port.ToString() + ")");
            }
            base.Disconnect(server, e);
        }
        public override void Error(IServer server, ServerErrorEventArgs e)
        {
            System.Net.IPEndPoint client = e.Session.RemoteEndPoint as System.Net.IPEndPoint;
            Console.WriteLine("New Message - " + client.Address.ToString() + " " + client.Port.ToString() + " : " + e.Error.ToString());
            base.Error(server, e);
        }
        private string ValidateRecivedData(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return "Invalid Payload !";
            }
            else if (!payload.StartsWith("#") || !payload.EndsWith("#"))
            {
                return "Invalid Payload !";
            }

            var datas = payload.Replace("#", "").Split('~');
            if (datas.Length != 8)
            {
                return "Invalid Payload !";
            }

            var deviceCode = datas[(int)DataStructure.DeviceCode];
            var client = RegisteredDevices.FirstOrDefault(x => x.Code == deviceCode);
            if (client is null)
            {
                return "Unauthorized Client !";
            }
            client.LastAccessDate = DateTime.Now;
            dalDevice.UpdateAsync(client).Wait();

            var date = datas[(int)DataStructure.DateTime];
            if (date.Length != 14)
            {
                return "Invalid Device Date !";
            }

            var deviceDate = new DateTime(
                Convert.ToInt32(date.Substring(4, 4)),
                Convert.ToInt32(date.Substring(2, 2)),
                Convert.ToInt32(date.Substring(0, 2)),
                Convert.ToInt32(date.Substring(8, 2)),
                Convert.ToInt32(date.Substring(10, 2)),
                Convert.ToInt32(date.Substring(12, 2)));
            var checksum = Convert.ToInt32(datas[(int)DataStructure.Checksum]);
            var calculatedChecksum = secretKey * client.Id - deviceDate.Second;
            if (checksum != calculatedChecksum)
            {
                return "Invalid Checksum !";
            }

            IotDeviceDatum deviceData = new IotDeviceDatum();
            deviceData.DeviceId = client.Id;
            deviceData.CreateDate = DateTime.Now;
            deviceData.DeviceDate = deviceDate;
            deviceData.Latitude = Convert.ToDecimal(datas[(int)DataStructure.Latitude]);
            deviceData.Longitude = Convert.ToDecimal(datas[(int)DataStructure.Longitude]);
            deviceData.Humidity = Convert.ToDecimal(datas[(int)DataStructure.Humidity]);
            deviceData.Temperature = Convert.ToDecimal(datas[(int)DataStructure.Temperature]); ;
            deviceData.DeviceHealth = Convert.ToByte(datas[(int)DataStructure.Health]);
            deviceData.RawData = payload;
            dalDeviceData.AddAsync(deviceData).Wait();

            return "Ok";
        }
        #endregion
    }
}
