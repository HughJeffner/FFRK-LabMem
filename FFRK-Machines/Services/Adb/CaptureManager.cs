using SharpAdbClient;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static FFRK_Machines.Services.Adb.Adb;

namespace FFRK_Machines.Services.Adb
{
    class CaptureManager
    {
        private readonly IAdbClient client;
        private readonly DeviceData device;
        private readonly Adb adb;
        private CancellationToken minicapTaskToken = new CancellationToken();
        private SemaphoreSlim minicapStarted = new SemaphoreSlim(0);
        private const String MINICAP_PATH = "/data/local/tmp/";
        private int minicapTimeouts = 0;


        public CaptureManager(Adb adb)
        {
            this.client = AdbClient.Instance;
            this.device = adb.Device;
            this.adb = adb;
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            if (adb.Capture == CaptureType.ADB) return;

            bool installed = false;

            ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Setting up frame capture");
            if (await MinicapInstalled(cancellationToken))
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Minicap installed");
                installed = true;
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Minicap not installed, attempting to install it now");
                if (await MinicapInstall(cancellationToken))
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Minicap installed, testing...");
                    if (await MinicapTest(cancellationToken))
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Minicap installed");
                        installed = true;
                    }
                }
            }

            if (!installed)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Could not verify minicap install, switching to ADB frame capture");
                adb.Capture = CaptureType.ADB;
                return;
            }

            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Starting minicap service");

            // Start on background thread
            _ = Task.Run(async () =>
            {
                try
                {
                    var screenSize = await adb.GetScreenSize();
                    string cmd = $"LD_LIBRARY_PATH={MINICAP_PATH} {MINICAP_PATH}minicap -P {screenSize.Width}x{screenSize.Height}@{screenSize.Width}x{screenSize.Height}/0";
                    minicapStarted.Release();
                    await client.ExecuteRemoteCommandAsync(cmd, device, null, minicapTaskToken, 0);
                }
                catch (OperationCanceledException) { }
                catch (SharpAdbClient.Exceptions.ShellCommandUnresponsiveException) { }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                }
                finally
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Minicap service has shut down, please restart the bot to recover.");
                    adb.Capture = CaptureType.ADB;
                }
            });

            // Forward port
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Forward minicap port");
            client.CreateForward(device, "tcp:1313", "localabstract:minicap", true);

            // Verify install
            if (!await MinicapVerify(cancellationToken))
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Could not verify minicap install, switching to ADB frame capture");
                adb.Capture = CaptureType.ADB;
                return;
            }

        }

        private async Task<bool> MinicapInstall(CancellationToken cancellationToken)
        {

            // Get Abi
            var abi = await adb.GetABI(cancellationToken);

            // Get Api level
            var apiLevel = await adb.GetAPILevel(cancellationToken);

            // Push binary
            using (var service = Factories.SyncServiceFactory(device))
            {
                var source = $"./minicap/{abi}/bin/minicap";
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Copying {source} to {MINICAP_PATH}");
                using (Stream stream = File.OpenRead(source))
                {
                    service.Push(stream, $"{MINICAP_PATH}minicap", 777, DateTime.Now, null, cancellationToken);
                }
            }
            // Push shared library
            using (var service = Factories.SyncServiceFactory(device))
            {
                var source = $"./minicap/{abi}/lib/android-{apiLevel}/minicap.so";
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Copying {source} to {MINICAP_PATH}");
                using (Stream stream = File.OpenRead(source))
                {
                    service.Push(stream, $"{MINICAP_PATH}minicap.so", 777, DateTime.Now, null, cancellationToken);
                }
            }

            // Set permissions
            await client.ExecuteRemoteCommandAsync($"chmod 777 {MINICAP_PATH}minicap", device, null, cancellationToken, 2000);

            return true;
        }

        private async Task<bool> MinicapInstalled(CancellationToken cancellationToken)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Checking minicap");
            using (var service = Factories.SyncServiceFactory(device))
            {
                return await Task.FromResult(service.Stat($"{MINICAP_PATH}minicap").FileMode != 0);
            }

        }

        private async Task<bool> MinicapVerify(CancellationToken cancellationToken)
        {

            String filePath = null;
            try
            {
                // File path unique per emulator
                var emulatorName = device.Name;
                if (String.IsNullOrEmpty(emulatorName)) emulatorName = "unknown";
                foreach (var c in Path.GetInvalidFileNameChars())
                {
                    emulatorName = emulatorName.Replace(c, '-');
                }
                filePath = $@".\minicap\verify_{emulatorName}.jpg";

                // If verification image present then immedately return true
                if (File.Exists(filePath)) return true;

                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Verifying minicap");

                // Need to wait for service to fully start
                await minicapStarted.WaitAsync(5000, cancellationToken);
                await Task.Delay(3000);

                // Save verification image
                using (var frame = await Minicap.CaptureFrame(2000, cancellationToken))
                {
                    // No image, failed
                    if (frame == null) return false;

                    // Save to verification image file
                    frame.Save(filePath);

                    // Examine
                    using (var bitmap = new Bitmap(frame))
                    {
                        var stat = new AForge.Imaging.ImageStatisticsHSL(bitmap);
                        if (stat.Luminance.Max == 0 || stat.Luminance.Mean < 0.001)
                        {
                            // Delete verification file and return false
                            File.Delete(filePath);
                            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Minicap returning blank screen with a avg luminance of {stat.Luminance.Mean}");
                            return false;
                        }
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
            }
            // Delete verification file and return false
            if (filePath != null) File.Delete(filePath);
            return false;
        }



        private async Task<bool> MinicapTest(CancellationToken cancellationToken)
        {

            // Execute minicap on device
            var screenSize = await adb.GetScreenSize();
            string cmd = $"LD_LIBRARY_PATH={MINICAP_PATH} {MINICAP_PATH}minicap -P {screenSize.Width}x{screenSize.Height}@{screenSize.Width}x{screenSize.Height}/0 -t";
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Testing minicap: {cmd}");
            var receiver = new ConsoleOutputReceiver();
            await client.ExecuteRemoteCommandAsync(cmd,
                device,
                receiver,
                cancellationToken,
                2000);

            return receiver.ToString().TrimEnd().EndsWith("OK");

        }

        public async Task<Image> Capture(CancellationToken cancellationToken)
        {
            Image ret;
            var frameBufferStopwatch = new Stopwatch();
            frameBufferStopwatch.Start();
            if (adb.Capture == CaptureType.Minicap)
            {
                ret = await Minicap.CaptureFrame(2000, cancellationToken);
                if (ret == null)
                {
                    minicapTimeouts += 1;
                    if (minicapTimeouts >= 20)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Minicap timed out reverting to ADB screencap");
                        adb.Capture = CaptureType.ADB;
                    }
                    ret = await AdbClient.Instance.GetFrameBufferAsync(device, cancellationToken);
                }
                else
                {
                    minicapTimeouts = 0;
                }
            }
            else
            {
                ret = await AdbClient.Instance.GetFrameBufferAsync(device, cancellationToken);
            }
            frameBufferStopwatch.Stop();
            ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Frame capture [{adb.Capture}] delay: {frameBufferStopwatch.ElapsedMilliseconds}ms");
            return ret;
        }

    }
}
