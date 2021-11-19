using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    class DataLogger
    {

        private static bool enabled = false;
        private static readonly string folder = @"./Data";
        private static readonly string fileSuffix = "_v01.csv";

        public static Task Initalize(ConfigHelper config)
        {
            DataLogger.enabled = config.GetBool("datalogger.enabled", false);
            if (DataLogger.enabled) ColorConsole.WriteLine(ConsoleColor.DarkGreen, "Data logging enabled, target folder: {0}", folder);
            return Task.CompletedTask;
        }

        public static async Task LogTreasureRate(Lab lab, JArray treasures)
        {
   
            if (treasures != null)
            {
                using (var writer = new StringWriter())
                {
                    var i = 0;
                    foreach (var item in treasures)
                    {
                        var row = CreateDataRow(lab);
                        row.Add(i.ToString());
                        row.Add(item.ToString());
                        WriteLine(writer, row.ToArray(), row.Count, ',');
                        i++;
                    }
                    await AppendFile("treasures", writer);
                }
            }

        }

        public static async Task LogExploreRate(Lab lab, JToken eventData, JToken status, bool insideDoor)
        {

            if (eventData != null)
            {
                using (var writer = new StringWriter())
                {
                    var row = CreateDataRow(lab);
                    if (insideDoor)
                    {
                        switch ((int)status)
                        {
                            case 2:
                            case 4:
                                row.Add("9");
                                break;
                            case 3:
                                row.Add("4");
                                break;
                            default:
                                row.Add("?" + status.ToString());
                                break;
                        }
                    } else
                    {
                        row.Add(eventData["type"].ToString());
                    }
                    row.Add(insideDoor?"1":"0");
                    WriteLine(writer, row.ToArray(), row.Count, ',');
                    await AppendFile("explores", writer);
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
                        var row = CreateDataRow(lab);
                        row.Add(item["item_name"].ToString());
                        row.Add(item["num"].ToString());
                        WriteLine(writer, row.ToArray(), row.Count, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, "Got Item: {0} x{1}",
                            row[3].Replace("★", "*"),
                            row[4]);

                        await InspectDrop(item, "item_type_name", row[3], row[4]);
                    }
                    await AppendFile("drops", writer);
                }
            }

        }

        public static async Task LogBattleDrops(Lab lab)
        {

            if (lab.Data == null) return;
            if (!lab.Data.ContainsKey("result")) return;

            var drops = lab.Data["result"]["prize_master"];
            var qtyMap = lab.Data["result"]["drop_item_id_to_num"];
            if (drops != null && qtyMap != null)
            {
                using (var writer = new StringWriter())
                {
                    foreach (var item in drops)
                    {
                        var row = CreateDataRow(lab);
                        row.Add(item.First["name"].ToString());
                        row.Add(qtyMap[item.First["item_id"].ToString()].ToString());
                        WriteLine(writer, row.ToArray(), row.Count, ',');

                        ColorConsole.WriteLine(ConsoleColor.DarkGreen, " Drop: {0} x{1}",
                            row[3].Replace("★", "*"),
                            row[4]);

                        await InspectDrop(item.First, "type_name", row[3], row[4]);
                    }
                    await AppendFile("drops_battle", writer);
                }

            }

        }

        private static async Task InspectDrop(JToken item, string typeField, string name, string qty)
        {
            var typeName = item[typeField];
            if (typeName != null)
            {
                await Counters.FoundDrop(typeName.ToString(), name, int.Parse(qty));
            }
            
        }

        private static List<String> CreateDataRow(Lab lab)
        {
            var row = new List<String>();
            row.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            row.Add(GetCurrentFloor(lab.CurrentFloor));
            row.Add(GetCurrentPaintingID(lab.CurrentPainting));
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
            Directory.CreateDirectory(folder);

            try
            {
                using (var stream = new StreamWriter(folder + "/" + fileName + fileSuffix, true))
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
