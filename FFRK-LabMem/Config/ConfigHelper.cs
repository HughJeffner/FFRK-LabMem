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

        public int GetInt(String key)
        {
            return Int32.Parse(this[key]);
        }

        public bool GetBool(String key)
        {
            return this[key].Equals("1") || this[key].ToLower().Equals("true");
        }

        public TEnum GetEnum<TEnum>(String key)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), this[key]);
        }
    }
}
