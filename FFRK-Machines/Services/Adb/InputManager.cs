﻿using SharpAdbClient;
using System;
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
                catch (Exception ex)
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                }
                finally
                {
                    ColorConsole.WriteLine(ConsoleColor.Red, "Minitouch service has shut down, please restart the bot to recover.");
                }
            });

            // Forward port
            ColorConsole.Debug(ColorConsole.DebugCategory.Adb, "Forward minitouch port");
            client.CreateForward(device, "tcp:1111", "localabstract:minitouch", true);

            // Start minitouch client
            _ = Task.Delay(500).ContinueWith(async t =>
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

        public async Task Tap(int X, int Y, CancellationToken cancellationToken)
        {

            if (adb.Input == InputType.Minitouch)
            {
                await Task.Delay(adb.TapDelay, cancellationToken);
                await minitouch.Tap(X, Y);
            }
            else
            {
                await client.ExecuteRemoteCommandAsync(String.Format("input tap {0} {1}", X, Y),
                device,
                null,
                cancellationToken,
                1000);
            }

        }

    }
}
