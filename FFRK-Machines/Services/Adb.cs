using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SharpAdbClient;
using System.Diagnostics;
using FFRK_Machines;
using FFRK_Machines.Extensions;
using System.Threading;
using System.IO;

namespace FFRK_LabMem.Services
{
    public class Adb
    {

        public class Size {
            public int Width {get; set;}
            public int Height { get; set; }
        }

        protected DeviceData Device { get; set; }
        public double TopOffset { get; set; }
        public double BottomOffset { get; set; }
        public bool Debug { get; set; }
        public bool HasDevice
        {
            get
            {
                return this.Device != null;
            }
        }
        private Size screenSize = null;
        private String host;
        
        public Adb(string path, string host, int topOffset, int bottomOffset)
        {

            AdbServer server = new AdbServer();
            var result = server.StartServer(path, restartServerIfNewer: true);
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
                var deviceName = this.Device.Name;
                if (deviceName.Equals("")) deviceName = "Unknown";
                deviceName += string.Format(" ({0} {1})", this.Device.Product, this.Device.Model);
                ColorConsole.WriteLine("Connected to " + deviceName);
                return true;
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not connect to device via adb.  Check your connection, make sure device drivers are installed, and enable USB debugging in developer options.  If you recently updated LabMem, try killing adb.exe and try again.");
                return false;
            }


        }

        public async Task NavigateHome(CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("input keyevent {0}", "KEYCODE_HOME"),
                this.Device,
                null,
                cancellationToken,
                1000);
        }

        public async Task StopPackage(String packageName, CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("am force-stop {0}", packageName),
                this.Device,
                null,
                cancellationToken,
                2000);
        }

        public async Task StartActivity(String packageName, String activityName, CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("am start -n {0}/{1}", packageName, activityName),
                this.Device,
                null,
                cancellationToken,
                2000);
        }

        public async Task<int> GetAPILevel(CancellationToken cancellationToken)
        {

            var receiver = new ConsoleOutputReceiver();
            await AdbClient.Instance.ExecuteRemoteCommandAsync("getprop ro.build.version.sdk",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            return int.Parse(receiver.ToString());

        }

        public async Task<String> GetPackageVersion(CancellationToken cancellationToken)
        {

            var receiver = new ConsoleOutputReceiver();
            await AdbClient.Instance.ExecuteRemoteCommandAsync("dumpsys package com.dena.west.FFRK | grep versionName",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            return receiver.ToString();

        }

        public async Task InstallRootCert(String certPath, CancellationToken cancellationToken)
        {

            // Get package version
            var packageVersion = await GetPackageVersion(cancellationToken);
            if (int.Parse(packageVersion.Substring(packageVersion.IndexOf("=")+1,1)) < 8)
            {
                return;
            }

            // Get API level
            int apiLevel = await GetAPILevel(cancellationToken);

            // Lollipop - Marshmallow
            if (apiLevel >= 21 && apiLevel <= 23)
            {

                if (File.Exists(certPath))
                {

                    var cert = "/storage/emulated/0/TWP_Root_Cert.pfx";
                    bool needsInstall = false;
                    //var certInstalled = "/data/misc/user/0/cacerts-added/3dcac768.0";

                    // Check if copied root cert present
                    // TODO: check if actually installed
                    using (SyncService service = new SyncService(this.Device))
                    {
                        var files = service.GetDirectoryListing("/storage/emulated/0/");
                        needsInstall = !files.Any(f => f.Path.EndsWith("TWP_Root_Cert.pfx"));
                    }

                    // If needs install
                    if (needsInstall)
                    {

                        // Copy root cert over
                        using (SyncService service = new SyncService(this.Device))
                        {
                            if (needsInstall)
                            {
                                using (Stream stream = File.OpenRead(certPath))
                                {
                                    service.Push(stream, cert, 999, DateTime.Now, null, cancellationToken);
                                }
                            }
                        }

                        // Prompt to install
                        await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("am start -a android.settings.SECURITY_SETTINGS"),
                            this.Device,
                            null,
                            cancellationToken,
                            2000);
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Install Certificate: Navigate to Credential Storage > Install from SD card");
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Browse to {0}", cert);
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Use blank password and default certificate name");
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "(You may need to set a device lockscreen)");
                    }
                    
                }

            } else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "This version of android currently not supported (7+).  Maybe root in the future.");
            }

        }

        public async Task TapXY(int X, int Y, CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("input tap {0} {1}", X, Y), 
                this.Device, 
                null, 
                cancellationToken, 
                1000);
        }

        public async Task TapPct(double X, double Y, CancellationToken cancellationToken)
        {
            Tuple<int, int> target = await ConvertPctToXY(X, Y);
            await TapXY(target.Item1, target.Item2, cancellationToken);
        }

        public async Task<List<Color>> GetPixelColorXY(List<Tuple<int, int>> coords, CancellationToken cancellationToken)
        {

            var ret = new List<Color>();

            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, cancellationToken))
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

        public async Task<List<Color>> GetPixelColorPct(List<Tuple<double, double>> coordsPct, CancellationToken cancellationToken)
        {

            // Convert to XY
            var coords = new List<Tuple<int, int>>();
            foreach (var item in coordsPct)
            {
                coords.Add(await ConvertPctToXY(item));
            }

            return await GetPixelColorXY(coords, cancellationToken);

        }

        public async Task<Color> GetPixelColorXY(int X, int Y, CancellationToken cancellationToken)
        {
            var color = await GetPixelColorXY(new List<Tuple<int, int>>() { 
                new Tuple<int, int>(X, Y) 
            }, cancellationToken);
            return color.First();
        }

        public async Task<Color> GetPixelColorPct(double X, double Y, CancellationToken cancellationToken)
        {
            var color = await GetPixelColorPct(new List<Tuple<double, double>>() { 
                new Tuple<double, double>(X, Y) 
            }, cancellationToken);
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

        public async Task<Tuple<double, double>> FindButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, CancellationToken cancellationToken)
        {

            if (this.Debug)
            {
                var dTargetStart = await ConvertPctToXY(xPct, yPctStart);
                var dTargetEnd = await ConvertPctToXY(xPct, yPctEnd);
                ColorConsole.Write(ConsoleColor.DarkGray, "Finding button [{0},{1}-{2}] ({3}): ", dTargetStart.Item1, dTargetStart.Item2, dTargetEnd.Item2, htmlButtonColor);
            }
            // Build input for pixel colors
            var coords = new List<Tuple<double, double>>();
            for (double i = yPctStart; i < yPctEnd; i+=0.5)
            {
                coords.Add(new Tuple<double, double>(xPct, i));
            }
            var results = await GetPixelColorPct(coords, cancellationToken);

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
            if (matches.Count > 0)
            {
                var min = matches.Keys.Min();
                System.Diagnostics.Debug.Print("matches: {0}, closest: {1}", matches.Count, min);
                if (this.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "matches: {0}, closest: {1}", matches.Count, min);
                return matches[min];
            }
            System.Diagnostics.Debug.Print("matches: {0}", matches.Count);
            if (this.Debug) ColorConsole.WriteLine(ConsoleColor.DarkGray, "matches: {0}", matches.Count);
            return null;

        }

        public async Task<Boolean> FindButtonAndTap(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries, CancellationToken cancellationToken)
        {

            int tries = 0;
            do
            {
                var b = await FindButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd, cancellationToken);
                if (b != null)
                {
                    await TapPct(b.Item1, b.Item2, cancellationToken);
                    return true;
                }
                tries++;
                await Task.Delay(1000, cancellationToken);
            } while (tries < retries);

            return false;

        }

        private async Task<Tuple<int, int>> ConvertPctToXY(Tuple<double, double> coords)
        {
            return await ConvertPctToXY(coords.Item1, coords.Item2);
        }

        private async Task<Tuple<int, int>> ConvertPctToXY(double xPct, double yPct)
        {

            if (screenSize == null) screenSize = await GetScreenSize();
            double virtX = screenSize.Width * (xPct / 100);
            double virtY = (screenSize.Height - this.TopOffset - this.BottomOffset) * (yPct / 100) + this.TopOffset;
            return new Tuple<int, int>((int)virtX, (int)virtY);

        }

        static Task<int> RunProcessAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = { FileName = fileName, Arguments = arguments, CreateNoWindow = true},
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
