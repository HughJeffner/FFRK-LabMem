using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;

namespace FFRK_LabMem
{
    class Program
    {
        static void Main(string[] args)
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(@"C:\Users\JLingis\SDK\android-sdk\platform-tools\adb.exe", restartServerIfNewer: false);
            Console.WriteLine(result);
            AdbClient.Instance.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 7555));
            var device = AdbClient.Instance.GetDevices().First();
            if (device != null)
            {
                Console.WriteLine("Connected to " + device.Name);
                var lab = new Lab(device);
               
            }
            else
            {
                Console.WriteLine("Could not connect");
            }
             
            Console.ReadKey();
            
        }

    }
}
