using FFRK_Machines;
using System;
using System.Diagnostics;
using System.Threading;

namespace FFRK_LabMem.Services
{
    class Benchmark
    {
        public static void FrameCapture(Adb adb)
        {
            long count = 0;
            long avg = 0;
            var hasTimings = ColorConsole.DebugCategories.HasFlag(ColorConsole.DebugCategory.Timings);
            if (hasTimings) ColorConsole.DebugCategories &= ~ColorConsole.DebugCategory.Timings;
            ColorConsole.WriteLine("Benchmarking frame capture, press any key to stop");
            Stopwatch time = new Stopwatch();
            while (!Console.KeyAvailable)
            {
                time.Restart();
                _ = adb.GetFrame(CancellationToken.None).Result;
                time.Stop();
                count += 1;
                avg -= avg / count;
                avg += time.ElapsedMilliseconds / count;
                ColorConsole.WriteLine(ConsoleColor.DarkGray, $"[Benchmark] Frame capture [{adb.Capture}] delay: {time.ElapsedMilliseconds}ms");
            }
            Console.ReadKey(true);
            ColorConsole.WriteLine($"[Benchmark] {avg}ms average, {count} total");
            if (hasTimings) ColorConsole.DebugCategories |= ColorConsole.DebugCategory.Timings;

        }

    }
}
