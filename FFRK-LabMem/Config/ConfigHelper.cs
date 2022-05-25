using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace FFRK_LabMem.Config
{
    public class ConfigHelper
    {

        private Configuration config = null;
        private KeyValueConfigurationCollection appSettings = null;
        private readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

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

        public void SetValues(List<KeyValuePair<string, IConvertible>> values)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var item in values)
            {
                
                if (config.AppSettings.Settings[item.Key] == null)
                {
                    config.AppSettings.Settings.Add(item.Key, item.Value.ToString(invariantCulture));
                }
                else
                {
                    config.AppSettings.Settings[item.Key].Value = item.Value.ToString(invariantCulture);
                }
            }
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void SetValue(String key, String value)
        {
            if (this.config != null)
            {
                if (this.config.AppSettings.Settings[key] == null)
                {
                    this.config.AppSettings.Settings.Add(key, value);
                } else
                {
                    this.config.AppSettings.Settings[key].Value = value;
                }
                this.config.Save();
            }
            else
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                } else
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
        public void SetValue(String key, int value)
        {
            SetValue(key, value.ToString(invariantCulture));
        }

        public void SetValue(String key, decimal value)
        {
            SetValue(key, value.ToString(invariantCulture));
        }

        public void SetValue(String key, double value)
        {
            SetValue(key, value.ToString(invariantCulture));
        }

        public void SetValue(String key, bool value)
        {
            SetValue(key, value ? "true" : "false");
        }

        public String GetString(String key, String defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return this[key];
        }

        public int GetInt(String key, int defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return Int32.Parse(this[key],invariantCulture);
        }
        
        public short GetShort(String key, short defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return short.Parse(this[key], invariantCulture);
        }

        public double GetDouble(String key, double defaultValue)
        {
            if (this[key] == null) return defaultValue;
            return double.Parse(this[key], invariantCulture);
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
