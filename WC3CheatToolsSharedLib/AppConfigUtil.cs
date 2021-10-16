using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace WC3CheatToolsSharedLib
{
    /// <summary>
    /// Class that contains various AppConfig utility methods.
    /// </summary>
    public class AppConfigUtil
    {
        public static string GetString(string key)
        {
            string stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"Missing required AppConfig setting '{key}'");
            }

            return stringValue;
        }

        public static string GetDirectory(string key)
        {
            string stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"Missing required AppConfig setting '{key}'");
            }

            string fullPath = Path.GetFullPath(stringValue);
            if (!fullPath.EndsWith("\\"))
            {
                fullPath += "\\";
            }

            return fullPath;
        }

        public static T GetEnum<T>(string key) where T : Enum
        {
            string stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"Missing required AppConfig setting '{key}'");
            }

            if (Enum.TryParse(typeof(T), stringValue, true, out object value))
            {
                return (T)value;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string[] possibleValues = Enum.GetNames(typeof(T));
                for (int i = 0; i < possibleValues.Length; i++)
                {
                    string possibleValue = possibleValues[i];
                    sb.Append($"'{possibleValue}'");
                    if (i < possibleValues.Length)
                    {
                        sb.Append(", ");
                    }
                }

                throw new ArgumentException($"Invalid AppConfig value '{stringValue}' for key '{key}'. Possible values: {sb}");
            }
        }

        public static bool GetBool(string key)
        {
            string stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"Missing required AppConfig setting '{key}'");
            }
            if (stringValue == "t" || stringValue == "T" || stringValue == "1" || stringValue.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (stringValue == "f" || stringValue == "F" || stringValue == "0" || stringValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"Could not parse AppConfig true/false value for key '{key}'");
            }
        }
    }
}
