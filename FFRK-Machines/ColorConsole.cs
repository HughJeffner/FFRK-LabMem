using System;
using System.Collections.Generic;
using System.Linq;

namespace FFRK_Machines
{
    public class ColorConsole
    {

        public static bool Timestamps { get; set; }
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
            Watchdog = 1 << 3
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
            }
        }

        public static void Write(string value)
        {
            lock (stampLock)
            {
                DoTimestamp(false);
                Console.Write(value);
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
            }
            
        }

        public static void WriteLine(string format, params object[] arg)
        {
            lock (stampLock)
            {
                DoTimestamp(true);
                Console.WriteLine(format, arg);
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
                        Console.Write("{0:HH:mm:ss} ", DateTime.Now);
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

    }
}
