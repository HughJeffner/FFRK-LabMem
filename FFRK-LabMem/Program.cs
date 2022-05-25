using System;
using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_LabMem.Services;
using FFRK_Machines;
using System.Linq;
using FFRK_Machines.Services.Adb;
using Avalonia;
using FFRK_Machines.Threading;
using FFRK_LabMem.Config.UI;

namespace FFRK_LabMem
{
    class Program
    {

        static AppConfig config;

        static void Main(string[] args)
        {

            // Listen for console exit
            ConsoleTasks.ListenForExit(new EventHandler(CurrentDomain_ProcessExit));
            ConsoleTasks.DisableQuickEditMode();
            Console.TreatControlCAsInput = true;

            // Get Configuration
            var configFile = (args.Length > 0) ? args[0] : null;
            config = new AppConfig(configFile);

            // Console
            ColorConsole.Timestamps = config.Console.Timestamps;
            ColorConsole.DebugCategories = config.Console.DebugCategories;
            ColorConsole.LogBuffer.Enabled = config.Console.Logging;
            ColorConsole.LogBuffer.UpdateFolderOrDefault(config.Console.LogFolder);
            ColorConsole.LogBuffer.BufferSize = config.Console.BufferSize;

            // Config arg switch
            if (args.Contains("-c"))
            {
                //ConfigForm.CreateAndShow(config, LabController.Create(config).Result);
                Tray.Hide();
                return;
            }

            // Version check
            var versionCode = Updates.GetVersionCode("beta");
            var versionTitle = String.Format("{0} {1}", Updates.GetName(), versionCode);
            ColorConsole.WriteLine(versionTitle);
            Console.Title = versionTitle;
            if (config.Updates.CheckForUpdates)
                _ = Updates.Check(config.Updates.IncludePrerelease);

            LabController controller = null;
            try
            {
                // Controller
                controller = LabController.CreateAndStart(config).Result;

                // User Interface
                _ = Utility.StartSTATask(() =>
                {
                    BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, Avalonia.Controls.ShutdownMode.OnExplicitShutdown);
                });

                // Ad-hoc command loop
                Console.WriteLine("Press 'D' to Disable, 'E' to Enable, 'C' for Config, 'S' for Stats, 'Ctrl+X' to Exit");
                Console.WriteLine("Type ? for help");
                Tyro.Register(controller);

                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control) break;
                    if (key.Key == ConsoleKey.E) controller.Enable();
                    if (key.Key == ConsoleKey.D) controller.Disable();
                    if (key.Key == ConsoleKey.H) Tray.MinimizeTo(key.Modifiers, controller);
                    if (key.Key == ConsoleKey.C && key.Modifiers == 0) ConfigWindow.CreateAndShow(config, controller);
                    if (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control) Console.Clear();
                    //if (key.Key == ConsoleKey.S) CountersForm.CreateAndShow(controller);
                    if (key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Alt) if (Updates.DownloadInstallerAndRun(config.Updates.IncludePrerelease).Result) break;
                    if (key.Key == ConsoleKey.O && key.Modifiers == ConsoleModifiers.Alt) controller.AutoDetectOffsets(config);
                    if (key.Key == ConsoleKey.B && key.Modifiers == ConsoleModifiers.Control) Clipboard.CopyProxyBypassToClipboard();
                    if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Alt) controller.ManualFFRKRestart();
                    if (key.Key == ConsoleKey.Q) controller.QuickExplore();
                    if (key.Key >= ConsoleKey.D0 && key.Key <= ConsoleKey.D9) controller.SetRestartCount(int.Parse(key.KeyChar.ToString()));
                    if (key.Key == ConsoleKey.OemMinus) controller.SetRestartCount(-1);
                    if (key.KeyChar == '?') Console.WriteLine(Properties.Resources.HelpString);
                    if (key.Key == ConsoleKey.B) Benchmark.FrameCapture(controller);
                    Tyro.ReadConsole(key);
                }
            }
            catch(Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, ex.ToString());
                ColorConsole.WriteLine("Unhandled exception occured, press any key to exit");
                ColorConsole.LogBuffer.Flush();
                Console.ReadKey();
            }
            finally
            {
                // Stop
                if (controller != null) controller.Stop();
                App.Shutdown();
            }

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // Kill adb option
            if (config.Adb.CloseOnExit) Adb.KillAdb();

            // Flush buffers
            ColorConsole.LogBuffer.Flush();
            Data.Counters.Flush().Wait();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

    }
}
