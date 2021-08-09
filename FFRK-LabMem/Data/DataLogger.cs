using FFRK_LabMem.Config;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    class DataLogger
    {

        private static Boolean enabled = false;

        public static void Initalize(ConfigHelper config)
        {
            DataLogger.enabled = config.GetBool("datalogger.enabled", false);
        }

        public static async Task LogGotItem(JObject data, JToken currentPainting)
        {

            var items = data["given_unsettled_items"];
            if (items != null)
            {
                using (var writer = new StringWriter())
                {
                    foreach (var item in items)
                    {
                        String[] row = CreateDataRow(4, currentPainting);
                        row[2] = item["item_name"].ToString();
                        row[3] = item["num"].ToString();
                        WriteLine(writer, row, row.Length, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, "Got Item: {0} x{1}",
                            row[2].Replace("★", "*"),
                            row[3]);

                    }
                    await AppendFile("drops.csv", writer);
                }
            }

        }

        public static async Task LogBattleDrops(JObject data, JToken currentPainting)
        {

            var drops = data["result"]["prize_master"];
            var qtyMap = data["result"]["drop_item_id_to_num"];
            if (drops != null && qtyMap != null)
            {
                using (var writer = new StringWriter())
                {
                    foreach (var item in drops)
                    {
                        String[] row = CreateDataRow(4, currentPainting);
                        row[2] = item.First["name"].ToString();
                        row[3] = qtyMap[item.First["item_id"].ToString()].ToString();
                        WriteLine(writer, row, row.Length, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, " Drop: {0} x{1}",
                            row[2].Replace("★", "*"),
                            row[3]);

                    }
                    await AppendFile("drops_battle.csv", writer);
                }

            }

        }

        private static string[] CreateDataRow(int columns, JToken currentPainting)
        {
            String[] row = new string[columns];
            row[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            row[1] = GetCurrentPaintingID(currentPainting);
            return row;
        }

        private static String GetCurrentPaintingID(JToken currentPainting)
        {

            if (currentPainting == null) return "?";
            return currentPainting["type"].ToString();

        }

        private static async Task AppendFile(String fileName, TextWriter data)
        {

            if (!enabled) return;

            Directory.CreateDirectory("./DataLog");

            try
            {
                using (var stream = new StreamWriter(@"./DataLog/" + fileName, true))
                {
                    await stream.WriteAsync(data.ToString());
                }


            } catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Error writing to file: {0}", ex.Message);
            }


        }

        private static void WriteLine(TextWriter writer, string[] data, int columnCount, char separator)
        {
            var escapeChars = new[] { separator, '\'', '\n' };
            for (var i = 0; i < columnCount; i++)
            {
                if (i > 0)
                    writer.Write(separator);

                if (i < data.Length)
                {
                    var escape = false;
                    var cell = data[i];
                    if (cell.Contains("\""))
                    {
                        escape = true;
                        cell = cell.Replace("\"", "\"\"");
                    }
                    else if (cell.IndexOfAny(escapeChars) >= 0)
                        escape = true;
                    if (escape)
                        writer.Write('"');
                    writer.Write(cell);
                    if (escape)
                        writer.Write('"');
                }
            }
            writer.WriteLine();
        }

    }
}
