using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;
using FFRK_LabMem.Extensions;

namespace FFRK_LabMem.Services
{
    public class Adb
    {

        public class Size {
            public int Width {get; set;}
            public int Height { get; set; }
        }

        public DeviceData Device { get; set; }
        private Size screenSize = null;
        private String host;
        
        public Adb(string path, string host)
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(path, restartServerIfNewer: false);
            this.host = host;
            ColorConsole.WriteLine("Adb status: {0}", result);

        }

        public bool Connect()
        {
            AdbClient.Instance.Connect(this.host);
            //AdbClient.Instance.Connect("127.0.0.1:62001");
            this.Device = AdbClient.Instance.GetDevices().FirstOrDefault();
            if (this.Device != null && this.Device.State == DeviceState.Online)
            {
                ColorConsole.WriteLine("Connected to " + this.Device.Name);
                return true;
            }
            else
            {
                ColorConsole.WriteLine("Could not connect");
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
            if (screenSize == null) screenSize = await GetScreenSize();
            await TapXY((int)(screenSize.Width * X / 100), (int)(screenSize.Height * Y / 100));
        }

        public async Task<List<Color>> GetPixelColorXY(List<Tuple<int, int>> coords)
        {

            var ret = new List<Color>();

            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, System.Threading.CancellationToken.None))
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {

                    foreach (var item in coords)
                    {
                        ret.Add(b.GetPixel(item.Item1, item.Item2));
                    }

                }

            }

            return ret;

        }

        public async Task<List<Color>> GetPixelColorPct(List<Tuple<double, double>> coordsPct)
        {
            if (screenSize == null) screenSize = await GetScreenSize();

            // Convert to XY
            var coords = new List<Tuple<int, int>>();
            foreach (var item in coordsPct)
            {
                coords.Add(new Tuple<int, int>((int)(screenSize.Width * item.Item1 / 100), (int)(screenSize.Height * item.Item2 / 100)));
            }

            return await GetPixelColorXY(coords);

        }

        public async Task<Color> GetPixelColorXY(int X, int Y)
        {
            var color = await GetPixelColorXY(new List<Tuple<int, int>>() { 
                new Tuple<int, int>(X, Y) 
            });
            return color.First();
        }

        public async Task<Color> GetPixelColorPct(double X, double Y)
        {
            var color = await GetPixelColorPct(new List<Tuple<double, double>>() { 
                new Tuple<double, double>(X, Y) 
            });
            return color.First();
        }

        public async Task<Size> GetScreenSize()
        {
            // Get screen dimensions
            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, System.Threading.CancellationToken.None))
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {
                    var size = new Size()
                    {
                        Width = b.Width,
                        Height = b.Height
                    };
                    return size;
                }

            }
        }

        public async Task<Tuple<double, double>> FindButton(int argbButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd)
        {

            // Hold result
            Tuple<double, double> ret = null;

            // Build input for pixel colors
            var coords = new List<Tuple<double, double>>();
            for (double i = yPctStart; i < yPctEnd; i++)
            {
                coords.Add(new Tuple<double, double>(xPct, i));
            }
            var results = await GetPixelColorPct(coords);

            // Iterate color and get distance
            foreach (var item in results)
            {
                var d = item.GetDistance(Color.FromArgb(argbButtonColor));
                if (d < threshold) { 
                    ret = coords[results.IndexOf(item)];
                    break;
                }
            }

            return ret;

        }

        public async Task<Boolean> FindButtonAndTap(int argbButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries)
        {

            int tries = 0;
            do
            {
                var b = await FindButton(argbButtonColor, threshold, xPct, yPctStart, yPctEnd);
                if (b != null)
                {
                    await TapPct(b.Item1, b.Item2);
                    return true;
                }
                tries++;
                await Task.Delay(1000);
            } while (tries < retries);

            return false;

        }

    }

}
