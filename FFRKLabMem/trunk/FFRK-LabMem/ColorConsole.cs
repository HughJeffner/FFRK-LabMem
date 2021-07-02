using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem
{
    class ColorConsole
    {

        public static bool Timestamps { get; set; }
        private static bool stamped = false;

        public static void Write(ConsoleColor color, String format, params object[] arg)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(format, arg);
            Console.ForegroundColor = current;
        }

        public static void Write(ConsoleColor color, String value)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Write(value);
            Console.ForegroundColor = current;
        }

        public static void Write(String format, params object[] arg)
        {
            DoTimestamp(false);
            Console.Write(format, arg);
        }

        public static void Write(String value)
        {
            DoTimestamp(false);
            Console.Write(value);
        }

        public static void WriteLine(ConsoleColor color, String format, params object[] arg)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(format, arg);
            Console.ForegroundColor = current;
        }

        public static void WriteLine(ConsoleColor color, String value)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(value);
            Console.ForegroundColor = current;
        }

        public static void WriteLine(String value)
        {
            DoTimestamp(true);
            Console.WriteLine(value);
        }

        public static void WriteLine(String format, params object[] arg)
        {
            DoTimestamp(true);
            Console.WriteLine(format, arg);
        }

        private static void DoTimestamp(bool newLine){

            if (ColorConsole.Timestamps)
            {
                if (!stamped)
                {
                    var current = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("{0:HH:mm:ss} ", DateTime.Now);
                    Console.ForegroundColor = current;
                }
                stamped = !newLine;
            }

        }

    }
}
