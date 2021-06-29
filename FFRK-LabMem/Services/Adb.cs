using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;

namespace FFRK_LabMem.Services
{
    class Adb
    {

        public DeviceData Device { get; set; }

        public Adb()
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(@"adb.exe", restartServerIfNewer: false);
            Console.WriteLine("Adb status: {0}", result);

        }

        public bool Connect()
        {
            //AdbClient.Instance.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 7555));
            AdbClient.Instance.Connect("127.0.0.1:7555");
            this.Device = AdbClient.Instance.GetDevices().FirstOrDefault();
            if (this.Device != null && this.Device.State == DeviceState.Online)
            {
                Console.WriteLine("Connected to " + this.Device.Name);
                return true;
            }
            else
            {
                Console.WriteLine("Could not connect");
                return false;
            }

        }
    }
}
