using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;

namespace FFRK_LabMem.Services
{
    public class Adb
    {

        public struct Size {
            public int Width;
            public int Height;
        }

        public DeviceData Device { get; set; }
        private Size _screenSize;
        public Size ScreenSize {
            get
            {
                if (_screenSize.Width == 0) _screenSize = GetScreenSize();
                return _screenSize;
            }
        
        }

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

        public async Task TapXY(int X, int Y)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("input tap {0} {1}", X, Y), 
                this.Device, 
                null, 
                System.Threading.CancellationToken.None, 
                1000);
        }

        public async Task TapPct(double X, double Y)
        {
            var size = this.ScreenSize;
            await TapXY((int)(size.Width * X/100), (int)(size.Height * Y/100));
        }

        private Size GetScreenSize()
        {
            // Get screen dimensions
            using (var framebuffer = AdbClient.Instance.GetFrameBufferAsync(this.Device, System.Threading.CancellationToken.None).Result)
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {
                    var size = new Size();
                    size.Width = b.Width;
                    size.Height = b.Height;
                    return size;
                }

            }
        }

    }

}
