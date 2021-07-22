using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;
using FFRK_LabMem.Extensions;
using System.Diagnostics;

namespace FFRK_LabMem.Services
{
    public class Adb
    {

        public class Size {
            public int Width {get; set;}
            public int Height { get; set; }
        }

        public DeviceData Device { get; set; }
        public double TopOffset { get; set; }
        public double BottomOffset { get; set; }
        private Size screenSize = null;
        private String host;
        
        public Adb(string path, string host, int topOffset, int bottomOffset)
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(path, restartServerIfNewer: false);
            this.host = host;
            this.TopOffset = topOffset;
            this.BottomOffset = bottomOffset;
            ColorConsole.WriteLine("Adb status: {0}", result);

        }

        public async Task<bool> Connect()
        {

            this.Device = AdbClient.Instance.GetDevices().LastOrDefault();
            if (this.Device == null)
            {
                await RunProcessAsync("cmd.exe", "/c adb connect " + this.host);
            }

            AdbClient.Instance.Connect(this.host);
            this.Device = AdbClient.Instance.GetDevices().LastOrDefault();
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
            double virtX = screenSize.Width * (X / 100);
            double virtY = (screenSize.Height - this.TopOffset - this.BottomOffset) * (Y / 100) + this.TopOffset;
            await TapXY((int)virtX, (int)virtY);
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
                double virtX = screenSize.Width * (item.Item1 / 100);
                double virtY = (screenSize.Height - this.TopOffset - this.BottomOffset) * (item.Item2 / 100) + this.TopOffset;
                coords.Add(new Tuple<int, int>((int)virtX, (int)virtY));
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

        public async Task<Tuple<double, double>> FindButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd)
        {

            // Build input for pixel colors
            var coords = new List<Tuple<double, double>>();
            for (double i = yPctStart; i < yPctEnd; i++)
            {
                coords.Add(new Tuple<double, double>(xPct, i));
            }
            var results = await GetPixelColorPct(coords);

            // Target color
            var target = System.Drawing.ColorTranslator.FromHtml(htmlButtonColor);

            // Hold matches
            Dictionary<int, Tuple<double, double>> matches = new Dictionary<int,Tuple<double,double>>();

            // Iterate color and get distance
            foreach (var item in results)
            {
                // Distance to target
                var d = item.GetDistance(target);

                // If below threshold
                if (d < threshold) {

                    // Add to matches
                    if (!matches.ContainsKey(d))
                        matches.Add(d, coords[results.IndexOf(item)]);

                }

            }

            // Return closest match
            if (matches.Count > 0) return matches[matches.Keys.Min()];

            return null;

        }

        public async Task<Boolean> FindButtonAndTap(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries)
        {

            int tries = 0;
            do
            {
                var b = await FindButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd);
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

        static Task<int> RunProcessAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = { FileName = fileName, Arguments = arguments },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }

    }

}
