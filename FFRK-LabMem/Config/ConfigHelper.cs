using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace FFRK_LabMem.Config
{
    class ConfigHelper
    {

        private KeyValueConfigurationCollection config = null;

        public ConfigHelper() : this(null) { }

        public ConfigHelper(String path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = path;
                System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                config = configuration.AppSettings.Settings;
            }
        }

        public String this[String key]
        {
            get
            {
                if (config == null || config[key] == null)
                {
                    return ConfigurationManager.AppSettings[key];
                }
                return config[key].Value;
            }
        }

        public String GetString(String key, String defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return this[key];
        }

        public int GetInt(String key, int defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return Int32.Parse(this[key]);
        }

        public bool GetBool(String key, bool defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return this[key].Equals("1") || this[key].ToLower().Equals("true");
        }

        public TEnum GetEnum<TEnum>(String key, TEnum defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return (TEnum)Enum.Parse(typeof(TEnum), this[key]);
        }
    }
}
