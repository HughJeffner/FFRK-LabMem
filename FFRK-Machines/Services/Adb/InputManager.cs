using SharpAdbClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static FFRK_Machines.Services.Adb.Adb;

namespace FFRK_Machines.Services.Adb
{
    class InputManager
    {
        private readonly IAdbClient client;
        private readonly DeviceData device;
        private readonly Adb adb;
        private CancellationToken minicapTaskToken = new CancellationToken();
        private Minitouch minitouch;
        private ulong tapCount = 0;
        private Tuple<int, int> lastTap = null;

        private const String MINITOUCH_PATH = "/data/local/tmp/";

        public InputManager(Adb adb)
        {
            this.client = AdbClient.Instance;
            this.device = adb.Device;
            this.adb = adb;
        }

        public async Task Setup(CancellationToken cancellationToken)
        {

            if (adb.Input == InputType.ADB) return;

            bool installed = false;

            ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Setting up input");
            if (await MinitouchInstalled(cancellationToken))
            {
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Minitouch installed");
                installed = true;
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Minitouch not installed, attempting to install it now");
                if (await MinitouchInstall(cancellationToken))
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Minitouch installed, testing...");
                    if (await MinitouchInstalled(cancellationToken))
                    {
                        ColorConsole.WriteLine(ConsoleColor.Yellow, "Minitouch installed");
                        installed = true;
                    }
                }
            }

            if (!installed)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Could not start minitouch client, switching to ADB input");
                adb.Input = InputType.ADB;
                return;
            }


            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Starting minitouch service");

            // Start service on device
            _ = Task.Run(async () =>
            {
                try
                {
                    string cmd = $"{MINITOUCH_PATH}minitouch";
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
                    ColorConsole.WriteLine(ConsoleColor.Red, "Minitouch service has shut down, please restart the bot to recover.");
                    adb.Input = InputType.ADB;
                }
            });

            // Forward port
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Forward minitouch port");
            client.CreateForward(device, "tcp:1111", "localabstract:minitouch", true);

            // Start minitouch client
            _ = Task.Delay(1000).ContinueWith(async t =>
            {
                minitouch = new Minitouch();
                if (!await minitouch.Connect())
                {
                    ColorConsole.WriteLine(ConsoleColor.Yellow, "Could not start minitouch client, switching to ADB input");
                    adb.Input = InputType.ADB;
                }
            });

        }
        async Task<bool> MinitouchInstall(CancellationToken cancellationToken)
        {

            // Get Abi
            var abi = await adb.GetABI(cancellationToken);

            // Get Api level
            var apiLevel = await adb.GetAPILevel(cancellationToken);

            // Push binary
            using (var service = Factories.SyncServiceFactory(device))
            {
                var source = $"./minicap/{abi}/bin/minitouch";
                ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Copying {source} to {MINITOUCH_PATH}");
                using (Stream stream = File.OpenRead(source))
                {
                    service.Push(stream, $"{MINITOUCH_PATH}minitouch", 777, DateTime.Now, null, cancellationToken);
                }
            }

            // Set permissions
            await client.ExecuteRemoteCommandAsync($"chmod 777 {MINITOUCH_PATH}minitouch", device, null, cancellationToken, 2000);

            return true;
        }
        private async Task<bool> MinitouchInstalled(CancellationToken cancellationToken)
        {
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Checking minitouch");
            using (var service = Factories.SyncServiceFactory(device))
            {
                return await Task.FromResult(service.Stat($"{MINITOUCH_PATH}minitouch").FileMode != 0);
            }

        }

        private bool IsMeMU()
        {
            return adb.Host.Length >= 5
                && adb.Host.EndsWith("3")
                && adb.Host.Substring(adb.Host.Length - 5, 3).Equals("215");
        }

        public async Task Tap(int X, int Y, CancellationToken cancellationToken)
        {
            // Internal tap counter
            tapCount++;

            // Debug
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, $"Tapping screen [{adb.Input}] at: [{X},{Y}]");
            var inputStopWatch = new Stopwatch();
            inputStopWatch.Start();

            // Do tap based on input type
            if (adb.Input == InputType.Minitouch)
            {
                await Task.Delay(adb.TapDelay, cancellationToken);
                await minitouch.Tap(X, Y, adb.TapPressure, adb.TapDuration);
            }
            else
            {
                var cmd = $"input tap {X} {Y}";
                if (adb.TapDuration > 0) cmd = $"input swipe {X} {Y} {X} {Y} {adb.TapDuration}";
                await client.ExecuteRemoteCommandAsync(cmd, device, null, cancellationToken, adb.TapDuration + 1000);
            }

            // Debug
            inputStopWatch.Stop();
            ColorConsole.Debug(ColorConsole.DebugCategory.Timings, $"Input [{adb.Input}] delay: {inputStopWatch.ElapsedMilliseconds}ms");

            // Retain last tap
            lastTap = Tuple.Create(X, Y);

            // Need duplicate tap for MeMU
            // See: https://github.com/HughJeffner/FFRK-LabMem/issues/177
            if (adb.Input == InputType.Minitouch && tapCount == 12 && IsMeMU())
            {
                await RedoTap(cancellationToken);
            }

        }

        public async Task RedoTap(CancellationToken cancellationToken)
        {
            if (lastTap != null) await Tap(lastTap.Item1, lastTap.Item2, cancellationToken);
        }

    }
}
