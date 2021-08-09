using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
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

        public static async Task LogExploreResult(Lab lab, JToken eventData, JToken status, bool insideDoor)
        {

            if (eventData != null)
            {
                using (var writer = new StringWriter())
                {
                    String[] row = CreateDataRow(5, lab);
                    if (insideDoor)
                    {
                        switch ((int)status)
                        {
                            case 2:
                            case 4:
                                row[2] = "9";
                                break;
                            case 3:
                                row[3] = "4";
                                break;
                            default:
                                row[3] = "?" + status.ToString();
                                break;
                        }
                    } else
                    {
                        row[3] = eventData["type"].ToString();
                    }
                    row[4] = insideDoor?"1":"0";
                    WriteLine(writer, row, row.Length, ',');
                    await AppendFile("explore_results_v01.csv", writer);
                }
            }
                        
        }

        public static async Task LogGotItem(Lab lab)
        {

            var items = lab.Data["given_unsettled_items"];
            if (items != null)
            {
                using (var writer = new StringWriter())
                {
                    foreach (var item in items)
                    {
                        String[] row = CreateDataRow(5, lab);
                        row[3] = item["item_name"].ToString();
                        row[4] = item["num"].ToString();
                        WriteLine(writer, row, row.Length, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, "Got Item: {0} x{1}",
                            row[2].Replace("★", "*"),
                            row[3]);

                    }
                    await AppendFile("drops_v01.csv", writer);
                }
            }

        }

        public static async Task LogBattleDrops(Lab lab)
        {

            var drops = lab.Data["result"]["prize_master"];
            var qtyMap = lab.Data["result"]["drop_item_id_to_num"];
            if (drops != null && qtyMap != null)
            {
                using (var writer = new StringWriter())
                {
                    foreach (var item in drops)
                    {
                        String[] row = CreateDataRow(5, lab);
                        row[3] = item.First["name"].ToString();
                        row[4] = qtyMap[item.First["item_id"].ToString()].ToString();
                        WriteLine(writer, row, row.Length, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, " Drop: {0} x{1}",
                            row[3].Replace("★", "*"),
                            row[4]);

                    }
                    await AppendFile("drops_battle_v01.csv", writer);
                }

            }

        }

        private static string[] CreateDataRow(int columns, Lab lab)
        {
            String[] row = new string[columns];
            row[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            row[1] = GetCurrentFloor(lab.CurrentFloor);
            row[2] = GetCurrentPaintingID(lab.CurrentPainting);
            return row;
        }

        private static String GetCurrentPaintingID(JToken currentPainting)
        {

            if (currentPainting == null) return "?";
            return currentPainting["type"].ToString();

        }

        private static String GetCurrentFloor(int floor)
        {
            if (floor == 0) return "?";
            return floor.ToString();
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
