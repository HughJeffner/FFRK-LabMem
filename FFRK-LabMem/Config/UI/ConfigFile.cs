using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FFRK_LabMem.Config.UI
{
    class ConfigFile
    {
        public static readonly string CONFIG_FOLDER = @".\Config\";
        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public ConfigFile(string path)
        {
            this.Path = path;
            this.Name = textInfo.ToTitleCase(path.Replace(CONFIG_FOLDER + "lab.", "").Replace(".json", ""));
        }

        public String Name { get; set; }
        public String Path { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public static List<ConfigFile> GetFiles()
        {
            var ret = new List<ConfigFile>();
            foreach (var item in Directory.GetFiles(CONFIG_FOLDER, "lab.*.json"))
            {
                ret.Add(new ConfigFile(item));
            }
            return ret;
        }
        public static ConfigFile FromName(string name)
        {
            return new ConfigFile(CONFIG_FOLDER + "lab." + name.ToLower() + ".json");
        }

        public static ConfigFile FromObject(object obj)
        {
            return (ConfigFile)obj;
        }

    }
}
