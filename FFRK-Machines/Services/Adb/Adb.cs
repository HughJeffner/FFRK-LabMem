using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SharpAdbClient;
using System.Diagnostics;
using FFRK_Machines.Extensions;
using System.Threading;
using System.Net;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FFRK_Machines.Services.Adb
{
    public class Adb
    {

        public const String FFRK_PACKAGE_NAME = "com.dena.west.FFRK";
        public const String FFRK_ACTIVITY_NAME = "jp.dena.dot.Dot";
        private int cachedApiLevel = 0;
        private string cachedAbi = string.Empty;
        private Size cachedScreenSize = null;
        private Random rng = new Random();
        private DeviceMonitor deviceMonitor = null;
        private InputManager inputManager;
        private CaptureManager captureManager;
        private Stopwatch tappingStopwatch = new Stopwatch();

        public event EventHandler<DeviceDataEventArgs> DeviceAvailable;
        public event EventHandler<DeviceDataEventArgs> DeviceUnavailable;

        public enum CaptureType
        {
            ADB = 0,
            Minicap = 1
        }

        public enum InputType
        {
            ADB = 0,
            Minitouch = 1
        }

        public class Size {
            public int Width { get; set; }
            public int Height { get; set; }
            public override string ToString()
            {
                return $"{Width}x{Height}";
            }
        }
        public class ImageDef
        {
            public Bitmap Image { get; set; }
            public float Simalarity { get; set; }
            public Tuple<double, double> Location { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: {1} {2}", Image, Simalarity, Location);
            }
        }
        public string Host { get; private set; }
        public DeviceData Device { get; set; }
        public double TopOffset { get; set; }
        public double BottomOffset { get; set; }
        public int CaptureRate { get; set; } = 200;
        public CaptureType Capture { get; set; } = CaptureType.ADB;
        public double FindPrecision { get; set; } = 0.5;
        public int FindAccuracy { get; set; } = 0;
        public InputType Input { get; set; } = InputType.Minitouch;
        public int TapDelay { get; set; } = 30;
        public int TapDuration { get; set; } = 0;
        public int TapPressure { get; set; } = 50;
        public String ScreenshotFolder { get; set; } = "";
        public bool HasDevice
        {
            get
            {
                return this.Device != null;
            }
        }
        
        public Adb(string path, string host, int topOffset, int bottomOffset)
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Starting server");
            AdbServer server = new AdbServer();
            var result = server.StartServer(path, restartServerIfNewer: true);
            this.Host = host;
            this.TopOffset = topOffset;
            this.BottomOffset = bottomOffset;

            // Device monitor
            this.deviceMonitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            this.deviceMonitor.DeviceConnected += this.OnDeviceConnected;
            this.deviceMonitor.DeviceDisconnected += this.OnDeviceDisconnected;
            this.deviceMonitor.DeviceChanged += this.OnDeviceChanged;
            this.deviceMonitor.Start();

        }

        private void OnDeviceChanged(object sender, DeviceDataEventArgs e)
        {
            ColorConsole.WriteLine("Device changed: {1}:{0}", e.Device, e.Device.State);
            if (e.Device.Serial.Equals(this.Host))
            {
                if (e.Device.State == DeviceState.Online) DeviceAvailable?.Invoke(sender, e);
                if (e.Device.State == DeviceState.Offline) DeviceUnavailable?.Invoke(sender, e);

            }
        }

        private void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            ColorConsole.WriteLine("Device unavailable: {0}", e.Device);
            if (e.Device.Serial.Equals(this.Host)) DeviceUnavailable?.Invoke(sender, e);
        }

        private void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            ColorConsole.WriteLine("Device available: {0}", e.Device);
            if (e.Device.Serial.Equals(this.Host)) DeviceAvailable?.Invoke(sender, e);
        }

        public async Task<bool> Connect()
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Connecting to device");

            // Choose from multiple connected devices
            this.Device = AdbClient.Instance.GetDevices().Where(d => d.Serial.Equals(this.Host)).FirstOrDefault();
            if (this.Device == null)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "First time connect, using cmd.exe");
                await RunProcessAsync("cmd.exe", "/c adb connect " + this.Host);
            }

            AdbClient.Instance.Connect(this.Host);
            this.Device = AdbClient.Instance.GetDevices().Where(d => d.Serial.Equals(this.Host)).FirstOrDefault();
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
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not connect to device via adb.  Check the documentation.");
                return false;
            }


        }

        public Task<List<String>> GetDevices()
        {
            return Task.FromResult(AdbClient.Instance.GetDevices().Select(d => d.Serial).ToList());
        }

        public async Task InstallCertificate(String pfxPath, CancellationToken cancellationToken)
        {
            var certificateManager = new CertificateManager(this);
            await certificateManager.InstallRootCert(pfxPath, cancellationToken);
        }

        public async Task<bool> SetProxySettings(int proxyPort, CancellationToken cancellationToken)
        {
            return await ProxyManager.SetProxySettings(AdbClient.Instance, this.Device, proxyPort, cancellationToken);

        }

        public async Task InputSetup(CancellationToken cancellationToken)
        {
            inputManager = new InputManager(this);
            await inputManager.Setup(cancellationToken);
        }

        public async Task CaptureSetup(CancellationToken cancellationToken)
        {
            captureManager = new CaptureManager(this);
            await captureManager.Setup(cancellationToken);
        }

        public async Task NavigateHome(CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("input keyevent {0}", "KEYCODE_HOME"),
                this.Device,
                null,
                cancellationToken,
                1000);
        }

        public async Task NavigateBack(CancellationToken cancellationToken)
        {
            await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("input keyevent {0}", "KEYCODE_BACK"),
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

        public async Task<bool> IsPackageRunning(string packageName, CancellationToken cancellationToken)
        {
            var receiver = new ConsoleOutputReceiver();
            await AdbClient.Instance.ExecuteRemoteCommandAsync(string.Format("ps | grep {0}", packageName),
            this.Device,
            receiver,
            cancellationToken,
            2000);
            return receiver.ToString().Contains(packageName);
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

            if (cachedApiLevel != 0) return cachedApiLevel;

            var receiver = new ConsoleOutputReceiver();
            await AdbClient.Instance.ExecuteRemoteCommandAsync("getprop ro.build.version.sdk",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            cachedApiLevel = int.Parse(receiver.ToString());
            return cachedApiLevel;

        }

        public async Task<string> GetABI(CancellationToken cancellationToken)
        {

            if (!String.IsNullOrEmpty(cachedAbi)) return cachedAbi;

            // Get ABI
            var receiver = new ConsoleOutputReceiver();
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Getting device ABI");
            await AdbClient.Instance.ExecuteRemoteCommandAsync("getprop ro.product.cpu.abi",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            cachedAbi = receiver.ToString().TrimEnd();
            return cachedAbi;
        }
        
        public async Task<Image> GetFrame(CancellationToken cancellationToken)
        {
            return await captureManager.Capture(cancellationToken);
        }

        public async Task StopTaps()
        {
            tappingStopwatch.Stop();
            await Task.CompletedTask;
        }

        public async Task TapXY(int X, int Y, CancellationToken cancellationToken)
        {
            await inputManager.Tap(X, Y, cancellationToken);
        }

        public async Task TapPct(double X, double Y, CancellationToken cancellationToken)
        {
            Tuple<int, int> target = await ConvertPctToXY(X, Y);
            await TapXY(target.Item1, target.Item2, cancellationToken);
        }

        public async Task TapPctSpam(double X, double Y, TimeSpan duration, CancellationToken cancellationToken)
        {
            Tuple<int, int> target = await ConvertPctToXY(X, Y);
            Tuple<int, int> variance = await ConvertPctToXY(0.5, 0.5);

            // Tap for duration or stopped
            tappingStopwatch.Restart();
            do
            {
                var tX = rng.Next(target.Item1 - variance.Item1, target.Item1 + variance.Item1);
                var tY = rng.Next(target.Item2 - variance.Item2, target.Item2 + variance.Item2);
                await TapXY(tX, tY, cancellationToken);
                await Task.Delay(TapDelay * 3);
            } while (tappingStopwatch.ElapsedMilliseconds < duration.TotalMilliseconds && tappingStopwatch.IsRunning);

        }

        public async Task<ImageDef> FindImages(List<ImageDef> images, int scaleFactor, CancellationToken cancellationToken)
        {
            ImageDef ret = null;

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Finding images: {0} ", images.Count);

            using (var framebuffer = await GetFrame(cancellationToken))
            {
                double ratio = (double)framebuffer.Height / (double)framebuffer.Width;
                int width = (720 / scaleFactor);
                int height = (int)(width * ratio);
                using (Bitmap b = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (Graphics gr = Graphics.FromImage(b))
                    {
                        gr.CompositingQuality = CompositingQuality.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.DrawImage(framebuffer, new Rectangle(0, 0, b.Width, b.Height));
                    }
                    var templateMatcher = new AForge.Imaging.ExhaustiveTemplateMatching();
                    foreach (var item in images)
                    {
                        templateMatcher.SimilarityThreshold = item.Simalarity;
                        var matches = templateMatcher.ProcessImage(b, item.Image);
                        if (matches.Length > 0)
                        {
                            // Return the center of the found image as a pct
                            var match = matches[0].Rectangle;

                            item.Location = new Tuple<double, double>(
                                ((match.X + (match.Width/2)) / (double)width) * 100, 
                                ((match.Y + (match.Height/2)) / (double)height) * 100
                            );
                            ret = item;
                            if (ColorConsole.CheckCategory(ColorConsole.DebugCategory.Adb))
                            {
                                var pixelLoc = await ConvertPctToXY(item.Location);
                                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}, closest: {1} [{2},{3}]", matches.Length, matches[0].Similarity, pixelLoc.Item1, pixelLoc.Item2);
                            }
                            break;
                        }
                    }
                   
                }

            }
            if (ret == null) ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: 0");
            return ret;

        }
        
        public async Task<List<Color>> GetPixelColorXY(List<Tuple<int, int>> coords, CancellationToken cancellationToken)
        {

            var ret = new List<Color>();
            using (var framebuffer = await GetFrame(cancellationToken))
            {
                using (Bitmap b = new Bitmap(framebuffer))
                {
                    foreach (var item in coords)
                    {
                        // Sanity checks
                        if (item.Item1 < b.Width && item.Item2 < b.Height)
                        {
                            ret.Add(b.GetPixel(item.Item1, item.Item2));
                        } else
                        {
                            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Attempt to read OOB pixel: {0},{1} in image sized: {2}x{3}", item.Item1, item.Item2, b.Width, b.Height);
                        }
                            
                    }

                }

            }

            return ret;

        }

        public async Task SaveScreenshot(String fileName, CancellationToken cancellationToken)
        {
            using (var framebuffer = await GetFrame(cancellationToken))
            {
                framebuffer.Save(System.IO.Path.Combine(ScreenshotFolder, fileName), ImageFormat.Png);
            }
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
            if (cachedScreenSize == null)
            {
                // Get screen dimensions
                using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, CancellationToken.None))
                {
                    using (Bitmap b = new Bitmap(framebuffer))
                    {
                        var size = new Size()
                        {
                            Width = b.Width,
                            Height = b.Height
                        };
                        // Rotation check
                        if (size.Width > size.Height)
                        {
                            var w = size.Width;
                            size.Width = size.Height;
                            size.Height = w;
                        }
                        cachedScreenSize = size;
                        ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Screen size set to: {cachedScreenSize}");
                    }

                }
            }

            return cachedScreenSize;
        }

        public async Task<Tuple<double, double>> GetButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, CancellationToken cancellationToken, double precision = -1)
        {

            // Defaults
            if (precision == -1) precision = FindPrecision;

            if (ColorConsole.CheckCategory(ColorConsole.DebugCategory.Adb))
            {
                var dTargetStart = await ConvertPctToXY(xPct, yPctStart);
                var dTargetEnd = await ConvertPctToXY(xPct, yPctEnd);
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Finding button {htmlButtonColor} [{dTargetStart.Item1},{dTargetStart.Item2}-{dTargetEnd.Item2}] t:{threshold} p:{precision}");
            }
            // Build input for pixel colors
            var coords = new List<Tuple<double, double>>();
            for (double i = yPctStart; i < yPctEnd; i+= precision)
            {
                coords.Add(new Tuple<double, double>(xPct, i));
            }
            var results = await GetPixelColorPct(coords, cancellationToken);

            // Target color
            var target = ColorTranslator.FromHtml(htmlButtonColor);

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
                var match = matches[min];
                if (ColorConsole.CheckCategory(ColorConsole.DebugCategory.Adb))
                {
                    var pixelLoc = await ConvertPctToXY(match);
                    ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}, closest: {1} [{2},{3}]", matches.Count, min, pixelLoc.Item1, pixelLoc.Item2);
                }
                return match;
            }
            Debug.Print("matches: {0}", matches.Count);
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}", matches.Count);
            return null;

        }

        public async Task<FindButtonResult> FindButtonAndTap(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries, CancellationToken cancellationToken, double precision = -1, int accuracy = -1)
        {

            var button = await FindButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd, retries, cancellationToken, precision, accuracy);
            if (button == null)
            {
                return new FindButtonResult();
            } else
            {
                await TapPct(button.button.Item1, button.button.Item2, cancellationToken);
                button.tapped = true;
                return button;
            }

        }

        public class FindButtonResult
        {
            public Tuple<double, double> button = null;
            public int retries = 0;
            public bool tapped = false;
        }

        public async Task<FindButtonResult> FindButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int timeout, CancellationToken cancellationToken, double precision = -1, int accuracy = -1)
        {

            // Defaults
            if (accuracy == -1) accuracy = FindAccuracy;

            var time = new Stopwatch();
            time.Start();
            int tries = 0;
            List<Tuple<double, double>> prevButtons = new List<Tuple<double, double>>();
            do
            {
                var b = await GetButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd, cancellationToken, precision);
                if (b != null)
                {
                    if (accuracy <= 0 || prevButtons.Where(i => i.Equals(b)).Count() >= accuracy)
                        return new FindButtonResult() { button = b, retries = tries };
                    prevButtons.Add(b);
                }
                tries++;
                if (timeout > 0) await Task.Delay(CaptureRate, cancellationToken);
            } while (time.ElapsedMilliseconds < timeout * 1000);

            return null;

        }

        public async Task<ImageDef> WaitForImage(Adb.ImageDef image, int scaleFactor, int timeout, CancellationToken cancellationToken)
        {
            List<Adb.ImageDef> items = new List<Adb.ImageDef>() { image };

            // Find
            var time = new Stopwatch();
            do
            {
                var img = await FindImages(items, scaleFactor,cancellationToken);
                if (img != null)
                {
                    return img;
                }
                if (timeout > 0) await Task.Delay(CaptureRate, cancellationToken);
            } while (time.ElapsedMilliseconds < timeout * 1000);
           
            return null;
        }

        public async Task<Tuple<int, int>> GetOffsets(string htmlColor, int threshold, CancellationToken cancellationToken)
        {

            int topOffset = 0;
            int bottomOffset = 0;

            // Screen size
            var size = await GetScreenSize();

            // Coordinates from top of screen to bottom
            var coords = new List<Tuple<int, int>>();
            for (int i = 0; i < size.Height; i++)
            {
                coords.Add(new Tuple<int, int>(size.Width / 2, i));
            }

            // Get color values
            var results = await GetPixelColorXY(coords, cancellationToken);

            // Target color gray
            var target = ColorTranslator.FromHtml(htmlColor);

            // Hold matches
            var matches = new List<int>();
            int itemIndex = 0;

            // Inspect each item
            foreach (var item in results)
            {
                // Distance to target
                var d = item.GetDistance(target);

                // If below threshold add to matches
                if (d < threshold) matches.Add(itemIndex);
                itemIndex++;

            }

            // Inspect matches starting from 0, if a jump over 1 occurs then top offset
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i] != i)
                {
                    topOffset = i - 1;
                    break;
                }
            }

            // Inspect matches starting from last match, if a jump over 1 occurs then bottom offset
            for (int i = matches.Count - 1; i > 0; i--)
            {
                if (matches[i] != (size.Height - 1) - (matches.Count - i - 1))
                {
                    bottomOffset = matches.Count - 1 - i;
                    break;
                }
            }

            // Sanity check
            if (topOffset < 0) topOffset = 0;
            if (bottomOffset < 0) bottomOffset = 0;

            return new Tuple<int, int>(topOffset, bottomOffset);

        }

        private async Task<Tuple<int, int>> ConvertPctToXY(Tuple<double, double> coords)
        {
            return await ConvertPctToXY(coords.Item1, coords.Item2);
        }

        private async Task<Tuple<int, int>> ConvertPctToXY(double xPct, double yPct)
        {

            var size = await GetScreenSize();
            double virtX = size.Width * (xPct / 100);
            double virtY = (size.Height - this.TopOffset - this.BottomOffset) * (yPct / 100) + this.TopOffset;
            return new Tuple<int, int>((int)virtX, (int)virtY);

        }

        static Task<int> RunProcessAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = { 
                    FileName = fileName, 
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle= ProcessWindowStyle.Hidden
                },
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

        public static void KillAdb()
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Killing");
            AdbClient.Instance.KillAdb();
        }

    }

}
