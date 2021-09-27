using System;
using System.Configuration;

namespace FFRK_LabMem.Config
{
    public class ConfigHelper
    {

        private System.Configuration.Configuration config = null;
        private KeyValueConfigurationCollection appSettings = null;

        public ConfigHelper() : this(null) { }

        public ConfigHelper(String path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = path;
                this.config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                this.appSettings = config.AppSettings.Settings;
            }
        }

        public String this[String key]
        {
            get
            {
                if (appSettings == null || appSettings[key] == null)
                {
                    return ConfigurationManager.AppSettings[key];
                }
                return appSettings[key].Value;
            }
        }

        public void SetValue(String key, String value)
        {
            if (this.config != null)
            {
                this.config.AppSettings.Settings[key].Value = value;
                this.config.Save();
            }
            else
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[key].Value = value;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
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
