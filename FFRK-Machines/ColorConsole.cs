﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFRK_Machines
{
    public class ColorConsole
    {

        public static bool Timestamps { get; set; }
        public static LogFileBuffer LogBuffer { get; set; } = new LogFileBuffer();
        public static DebugCategory DebugCategories { get; set; }
        private static bool stamped = false;
        private static object stampLock = new object();
        private static object colorLock = new object();
        

        [Flags]
        public enum DebugCategory : short
        {
            Adb = 1 << 0,
            Proxy = 1 << 1,
            Lab = 1 << 2,
            Watchdog = 1 << 3,
            Notifcation = 1 << 4
        }

        public static void Write(ConsoleColor color, string format, params object[] arg)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Write(format, arg);
                Console.ForegroundColor = current;
            }
        }

        public static void Write(ConsoleColor color, string value)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Write(value);
                Console.ForegroundColor = current;
            }

        }

        public static void Write(string format, params object[] arg)
        {
            lock (stampLock)
            {
                DoTimestamp(false);
                Console.Write(format, arg);
                LogBuffer.Add(String.Format(format, arg));
            }
        }

        public static void Write(string value)
        {
            lock (stampLock)
            {
                DoTimestamp(false);
                Console.Write(value);
                LogBuffer.Add(value);
            }
        }

        public static void Debug(DebugCategory category, string format, params object[] arg)
        {
            if (CheckCategory(category)) WriteLine(ConsoleColor.DarkGray, string.Format("[{0}] {1}", category, format), arg);
            
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] arg)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                WriteLine(format, arg);
                Console.ForegroundColor = current;
            }
            
        }
        public static void Debug(DebugCategory category, string value)
        {
            if (CheckCategory(category)) WriteLine(ConsoleColor.DarkGray, "[{0}] {1}", category, value);
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                WriteLine(value);
                Console.ForegroundColor = current;
            }
        }

        public static void WriteLine(string value)
        {
            lock (stampLock)
            {
                DoTimestamp(true);
                Console.WriteLine(value);
                LogBuffer.Add($"{value}{Environment.NewLine}");
            }
            
        }

        public static void WriteLine(string format, params object[] arg)
        {
            lock (stampLock)
            {
                DoTimestamp(true);
                Console.WriteLine(format, arg);
                LogBuffer.Add(String.Format(format, arg) + Environment.NewLine);
            }
           
        }

        private static void DoTimestamp(bool newLine)
        {
            if (Timestamps)
            {
                if (!stamped)
                {
                    lock (colorLock)
                    {
                        var current = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        var stamp = string.Format("{0:HH:mm:ss} ", DateTime.Now);
                        Console.Write(stamp);
                        LogBuffer.Add(stamp);
                        Console.ForegroundColor = current;
                    }
                }
                stamped = !newLine;
            }

        }

        public static bool CheckCategory(DebugCategory categoryToCheck)
        {
            return DebugCategories.HasFlag(categoryToCheck);
        }

        public static List<DebugCategory> GetCategories()
        {
            var ret = new List<DebugCategory>();
            foreach (var item in Enum.GetValues(typeof(DebugCategory)).Cast<DebugCategory>())
            {
                ret.Add(item);
            }
            return ret;
        }

        public static String GetSelectedCategories(DebugCategory categories)
        {
            var selected = new List<string>();
            foreach (var item in GetCategories())
            {
                if (categories.HasFlag(item)) selected.Add(item.ToString());
            }
            if (selected.Count == 0) return "None";
            return String.Join(",", selected.ToArray());
        }

        public static string ReadLine(TimeSpan timeout)
        {
            Task<string> task = Task.Factory.StartNew(Console.ReadLine);

            string result = Task.WaitAny(new Task[] { task }, timeout) == 0
                ? task.Result
                : string.Empty;
            return result;
        }

        public static ConsoleKeyInfo ReadKey(int timeout)
        {

            ConsoleKeyInfo k = new ConsoleKeyInfo();
            for (int cnt = timeout; cnt > 0; cnt--)
            {
                if (Console.KeyAvailable)
                {
                    k = Console.ReadKey();
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            return k;

        }

        public class LogFileBuffer : ConcurrentQueue<string>
        {

            public int BufferSize { get; set; } = 10;
            public bool Enabled { get; set; } = true;
            public string LogDirectory { get; set; } = @".\Logs";

            public void Add(string value)
            {
                // Do nothing if disabled
                if (!Enabled) return;

                // Queue item
                this.Enqueue(value);

                // Flush buffer
                if (this.Count >= BufferSize && value.EndsWith(Environment.NewLine)) Flush();

            }

            public void Flush()
            {
                // Do nothing if disabled
                if (!Enabled) return;

                // Ensure directory exists
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                // Format file name
                var fileName = String.Format("{0:yyyyMMdd}.log", DateTime.Now);
                
                // Write queue to file
                try
                {
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(this.LogDirectory, fileName), true))
                    {
                        string text;
                        while (this.TryDequeue(out text))
                        {
                            outputFile.Write(text);
                        }
                    }
                } catch (Exception ex)
                {
                    // Write exceptions direct to console to prevent looping
                    Console.WriteLine("Error writing to log file!");
                    Console.WriteLine(ex.ToString());
                }
                
            }

        }

    }
}
