using FFRK_LabMem.Machines;
using FFRK_Machines;
using System;
using System.Diagnostics;
using System.Threading;

namespace FFRK_LabMem.Services
{
    class Benchmark
    {
        public static void FrameCapture(LabController controller)
        {
            if (controller.Enabled)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Benchmarking only available when bot is Disabled");
                return;
            }

            long count = 0;
            long avg = 0;
            var hasTimings = ColorConsole.DebugCategories.HasFlag(ColorConsole.DebugCategory.Timings);
            if (hasTimings) ColorConsole.DebugCategories &= ~ColorConsole.DebugCategory.Timings;
            ColorConsole.WriteLine("Benchmarking frame capture, press any key to stop");
            ColorConsole.WriteLine("Make sure the screen has motion!");
            Stopwatch time = new Stopwatch();
            while (!Console.KeyAvailable)
            {
                // Time the image capture
                time.Restart();
                var img = controller.Adb.GetFrame(CancellationToken.None).Result;
                time.Stop();

                // Dispose the result
#pragma warning disable CA1416 // Validate platform compatibility
                img.Dispose();
#pragma warning restore CA1416 // Validate platform compatibility

                // Stats
                count += 1;
                avg -= avg / count;
                avg += time.ElapsedMilliseconds / count;
                ColorConsole.WriteLine(ConsoleColor.DarkGray, $"[Benchmark] Frame capture [{controller.Adb.Capture}] delay: {time.ElapsedMilliseconds}ms");
            }
            Console.ReadKey(true);
            ColorConsole.WriteLine($"[Benchmark] {avg}ms average, {count} total");
            if (hasTimings) ColorConsole.DebugCategories |= ColorConsole.DebugCategory.Timings;

        }

    }
}
