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
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FFRK_LabMem.Services
{
    public class Adb
    {

        public const String FFRK_PACKAGE_NAME = "com.dena.west.FFRK";
        public const String FFRK_ACTIVITY_NAME = "jp.dena.dot.Dot";
        private const String CERTIFICATE_USER_PATH = "/data/misc/user/0/cacerts-added/3dcac768.0";
        private const String CERTIFICATE_SYSTEM_PATH = "/system/etc/security/cacerts/3dcac768.0";
        private const String CERTIFICATE_CRT_PATH = "/sdcard/LabMem_Root_Cert.crt";
        private int cachedApiLevel = 0;

        public event EventHandler<DeviceDataEventArgs> DeviceAvailable;
        public event EventHandler<DeviceDataEventArgs> DeviceUnavailable;

        public class Size {
            public int Width {get; set;}
            public int Height { get; set; }
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
        protected DeviceData Device { get; set; }
        public double TopOffset { get; set; }
        public double BottomOffset { get; set; }
        public bool HasDevice
        {
            get
            {
                return this.Device != null;
            }
        }
        private Size screenSize = null;
        private String host;
        private DeviceMonitor deviceMonitor = null;
        
        public Adb(string path, string host, int topOffset, int bottomOffset)
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Starting server");
            AdbServer server = new AdbServer();
            var result = server.StartServer(path, restartServerIfNewer: true);
            this.host = host;
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
            if (e.Device.State == DeviceState.Online) DeviceAvailable?.Invoke(sender, e);
            if (e.Device.State == DeviceState.Offline) DeviceUnavailable?.Invoke(sender, e);
        }

        private void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            ColorConsole.WriteLine("Device unavailable: {0}", e.Device);
            DeviceUnavailable?.Invoke(sender, e);
        }

        private void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            ColorConsole.WriteLine("Device available: {0}", e.Device);
            DeviceAvailable?.Invoke(sender, e);
        }

        public async Task<bool> Connect()
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Connecting to device");
            this.Device = AdbClient.Instance.GetDevices().LastOrDefault();
            if (this.Device == null)
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "First time connect, using cmd.exe");
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
                ColorConsole.WriteLine(ConsoleColor.Red, "Could not connect to device via adb.  Check the documentation.");
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

        public async Task<bool> SetProxySettings(int proxyPort, CancellationToken cancellationToken)
        {
            var receiver = new ConsoleOutputReceiver();

            // Remove or set
            if (proxyPort == 0)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Removing proxy settings...");
                await AdbClient.Instance.ExecuteRemoteCommandAsync("settings put global http_proxy :0",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                await AdbClient.Instance.ExecuteRemoteCommandAsync("settings delete global global_http_proxy_exclusion_list",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Please restart your device/emulator to finish applying proxy settings");
            } 
            else
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Detecting proxy settings...");
                await AdbClient.Instance.ExecuteRemoteCommandAsync("ip route list match 0 table all scope global",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                string defaultRoute = "10.0.2.2";
                if (receiver.ToString().ToLower().StartsWith("default via"))
                {
                    defaultRoute = receiver.ToString().Split(' ')[2];
                } else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Could not determine default route, using default: {0}", defaultRoute);
                }
                receiver = new ConsoleOutputReceiver();
                await AdbClient.Instance.ExecuteRemoteCommandAsync("settings get global global_http_proxy_host",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyHostMatch = receiver.ToString().TrimEnd().ToLower().Equals(defaultRoute);
                receiver = new ConsoleOutputReceiver();
                await AdbClient.Instance.ExecuteRemoteCommandAsync("settings get global global_http_proxy_port",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyPortMatch = receiver.ToString().TrimEnd().Equals(proxyPort.ToString());
                receiver = new ConsoleOutputReceiver();
                await AdbClient.Instance.ExecuteRemoteCommandAsync("settings get global global_http_proxy_exclusion_list",
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                bool proxyExclusionMatch = receiver.ToString().TrimEnd().ToLower().Equals(Proxy.PROXY_BYPASS.ToLower());
                if (!proxyHostMatch || !proxyPortMatch || !proxyExclusionMatch)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Auto-setting system proxy settings...");
                    await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("settings put global http_proxy {0}:{1}", defaultRoute, proxyPort),
                    this.Device,
                    receiver,
                    cancellationToken,
                    2000);
                    await AdbClient.Instance.ExecuteRemoteCommandAsync("settings put global global_http_proxy_exclusion_list " + Proxy.PROXY_BYPASS,
                        this.Device,
                        receiver,
                        cancellationToken,
                        2000);
                    await AdbClient.Instance.ExecuteRemoteCommandAsync("settings delete global http_proxy",
                        this.Device,
                        receiver,
                        cancellationToken,
                        2000);
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Please restart your device/emulator to apply proxy settings");
                    return await Task.FromResult(false);
                } else
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Proxy settings OK (trouble loading?, try changing port)");
                }
                
            }

            return await Task.FromResult(true);

        }

        public async Task<bool> CopyUserCertsToSystem(CancellationToken cancellationToken)
        {

            var receiver = new ConsoleOutputReceiver();
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Remount system partition as writeable");
            await AdbClient.Instance.ExecuteRemoteCommandAsync("mount -o rw,remount /system",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            if (receiver.ToString().Contains("denied")) return false;
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy user certs to system certs");
            await AdbClient.Instance.ExecuteRemoteCommandAsync("cp /data/misc/user/0/cacerts-added/* /system/etc/security/cacerts/",
                this.Device,
                receiver,
                cancellationToken,
                2000);
            ColorConsole.WriteLine(ConsoleColor.Yellow, "Remount system partition as read only");
            await AdbClient.Instance.ExecuteRemoteCommandAsync("mount -o ro,remount /system",
               this.Device,
               receiver,
               cancellationToken,
               2000);
            return true;
        }

        public async Task<RootCertInstalledStatus> CheckIfRootCertInstalled(int apiLevel)
        {
            using (var service = Factories.SyncServiceFactory(this.Device))
            {
                var ret = new RootCertInstalledStatus();
                ret.UserExists = service.Stat(CERTIFICATE_USER_PATH).FileMode != 0;
                ret.SystemExists = service.Stat(CERTIFICATE_SYSTEM_PATH).FileMode != 0;
                ret.Installed = ((ret.UserExists && apiLevel <= 23) || ret.SystemExists);
                return await Task.FromResult(ret);
            }

        }

        public async Task<bool> CopyRootCertToStorage(String certPath, X509Certificate2 rootCert, CancellationToken cancellationToken)
        {

            try
            {
                using (var service = Factories.SyncServiceFactory(this.Device))
                {
                    using (MemoryStream stream = new MemoryStream(rootCert.Export(X509ContentType.Cert)))
                    {
                        service.Push(stream, certPath, 999, DateTime.Now, null, cancellationToken);
                    }
                }
            } catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                return await Task.FromResult(false);
            }
     
            return await Task.FromResult(true);

        }

        public async Task<X509Certificate2> GetInstalledRootCert(bool isSystemCert, CancellationToken cancellationToken)
        {

            var pathToCert = (isSystemCert) ? CERTIFICATE_SYSTEM_PATH : CERTIFICATE_USER_PATH;
            X509Certificate2 ret;

            using (var service = Factories.SyncServiceFactory(this.Device))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    service.Pull(pathToCert, stream, null, cancellationToken);
                    ret = new X509Certificate2(stream.ToArray());
                }
            }

            return await Task.FromResult(ret);
        }

        public async Task PromptToInstallRootCert(String cert, X509Certificate2 rootCert, int apiLevel, CancellationToken cancellationToken)
        {

            // First copy it over
            if (await CopyRootCertToStorage(cert, rootCert, cancellationToken))
            {

                // Start security settings activity
                await AdbClient.Instance.ExecuteRemoteCommandAsync(String.Format("am start -a android.settings.SECURITY_SETTINGS"),
                                this.Device,
                                null,
                                cancellationToken,
                                2000);

                ColorConsole.WriteLine(ConsoleColor.Yellow, "***************** Install Certificate *****************");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Scroll to Credential Storage > Install from SD card");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Browse to {0}", cert);
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Use FFRK for certificate name");
                ColorConsole.WriteLine(ConsoleColor.Yellow, "(You may need to set a device lockscreen)");

                // Need root
                if (apiLevel >= 24)
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "*******************ROOT REQUIRED***********************");
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Please press <Enter> once certificate installed to copy");
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "it to the system store");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    if (await CopyUserCertsToSystem(cancellationToken))
                    {
                        if ((await CheckIfRootCertInstalled(apiLevel)).Installed)
                        {
                            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy complete.  You may now delete the user certificate");
                        }
                        else
                        {
                            ColorConsole.WriteLine(ConsoleColor.Yellow, "Copy failed.  Check if the certificate is under the user tab of Trusted Credentials and try again");
                        }

                    }
                    else
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "This version of android currently not supported (7+ no root).  Please root your device.");
                    }
                }
                ColorConsole.WriteLine(ConsoleColor.Yellow, "*******************************************************");

            }

        }

        public async Task ValidateInstalledRootCert(String certPath, bool isSystemCert, X509Certificate2 rootCert, int apiLevel, CancellationToken cancellationToken)
        {

            var installedRootCert = await GetInstalledRootCert(isSystemCert, cancellationToken);

            // Thumbprint check
            if (!installedRootCert.Thumbprint.Equals(rootCert.Thumbprint))
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "** CERTIFICATE MISMATCH **");
                ColorConsole.WriteLine(ConsoleColor.Red, "** DELETE THE CURRENT CERTIFICATE IN THE USER TAB BEFORE PROCEEDING**");
                await PromptToInstallRootCert(certPath, rootCert, apiLevel, cancellationToken);
                return;
            }

            // Expired
            TimeSpan timeLeft = installedRootCert.NotAfter - DateTime.Now;
            if (timeLeft.TotalMinutes <= 0)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "** CERTIFICATE EXPIRED **");
                ColorConsole.WriteLine(ConsoleColor.Red, "** DELETE THE CURRENT CERTIFICATE IN THE USER TAB BEFORE PROCEEDING**");
                await PromptToInstallRootCert(certPath, rootCert, apiLevel, cancellationToken);
                return;
            }

            // Expire warning
            if (timeLeft.Days <= 30)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "** WARNING ** Installed root CA certificate will expire in {0} days", timeLeft.Days);
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Delete .pfx file and relaunch to begin re-install process");
            }

        }

        public async Task InstallRootCert(String pfxPath, CancellationToken cancellationToken)
        {

            // Get API level
            int apiLevel = await GetAPILevel(cancellationToken);

            // Lollipop or higher
            if (apiLevel >= 21)
            {

                // If pfx file exists
                if (File.Exists(pfxPath))
                {

                    // Root Cert in filesystem
                    var rootCert = new X509Certificate2(pfxPath, "")
                    {
                        PrivateKey = null
                    };

                    // If needs install
                    var installStatus = await CheckIfRootCertInstalled(apiLevel);
                    if (!installStatus.Installed)
                    {
                        // Prompt to install
                        await PromptToInstallRootCert(CERTIFICATE_CRT_PATH, rootCert, apiLevel, cancellationToken);

                    } else
                    {
                        // Installed certificate validation checks
                        await ValidateInstalledRootCert(CERTIFICATE_CRT_PATH, installStatus.SystemExists, rootCert, apiLevel, cancellationToken);

                    }
                    
                }

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

        public async Task<ImageDef> FindImages(List<ImageDef> images, int scaleFactor, CancellationToken cancellationToken)
        {
            ImageDef ret = null;

            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, cancellationToken))
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
                            Debug.Print("matches: {0}, closest: {1}", matches.Length, matches[0].Similarity);
                            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}, closest: {1}", matches.Length, matches[0].Similarity);
                            break;
                        }
                    }
                   
                }

            }

            return ret;

        }

        Stopwatch frameBufferStopwatch = new Stopwatch();
        public async Task<List<Color>> GetPixelColorXY(List<Tuple<int, int>> coords, CancellationToken cancellationToken)
        {

            var ret = new List<Color>();
            frameBufferStopwatch.Restart();
            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, cancellationToken))
            {
                frameBufferStopwatch.Stop();
                ColorConsole.Debug(ColorConsole.DebugCategory.Timings, "Frame buffer delay: {0}ms", frameBufferStopwatch.ElapsedMilliseconds);
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

        public async Task SaveScrenshot(String fileName, CancellationToken cancellationToken)
        {
            using (var framebuffer = await AdbClient.Instance.GetFrameBufferAsync(this.Device, cancellationToken))
            {
                framebuffer.Save(fileName, ImageFormat.Png);
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

        public async Task<Tuple<double, double>> GetButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, CancellationToken cancellationToken, double granularity = 0.5)
        {

            if (ColorConsole.CheckCategory(ColorConsole.DebugCategory.Adb))
            {
                var dTargetStart = await ConvertPctToXY(xPct, yPctStart);
                var dTargetEnd = await ConvertPctToXY(xPct, yPctEnd);
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Finding button [{0},{1}-{2}] ({3}): ", dTargetStart.Item1, dTargetStart.Item2, dTargetEnd.Item2, htmlButtonColor);
            }
            // Build input for pixel colors
            var coords = new List<Tuple<double, double>>();
            for (double i = yPctStart; i < yPctEnd; i+= granularity)
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
                Debug.Print("matches: {0}, closest: {1}", matches.Count, min);
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}, closest: {1}", matches.Count, min);
                return matches[min];
            }
            Debug.Print("matches: {0}", matches.Count);
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "matches: {0}", matches.Count);
            return null;

        }

        public async Task<bool> FindButtonAndTap(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries, CancellationToken cancellationToken, double granularity = 0.5)
        {
            
            var button = await FindButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd, retries, cancellationToken, granularity);
            if (button == null)
            {
                return false;
            } else
            {
                await TapPct(button.Item1, button.Item2, cancellationToken);
                return true;
            }

        }

        public async Task<Tuple<double, double>> FindButton(String htmlButtonColor, int threshold, double xPct, double yPctStart, double yPctEnd, int retries, CancellationToken cancellationToken, double granularity = 0.5)
        {

            int tries = 0;
            do
            {
                var b = await GetButton(htmlButtonColor, threshold, xPct, yPctStart, yPctEnd, cancellationToken, granularity);
                if (b != null)
                {
                    return b;
                }
                tries++;
                await Task.Delay(1000, cancellationToken);
            } while (tries < retries);

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

        public class RootCertInstalledStatus
        {
            public bool Installed { get; set; }
            public bool UserExists { get; set; }
            public bool SystemExists { get; set; }

        }

    }

}
