using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace WC3CheatDetector.Utils
{
    /// <summary>
    /// Class that contains various AppConfig utility methods.
    /// </summary>
    public class AppConfigUtil
    {
        /// <summary>
        /// Retrieves a string from the App Config
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value string. Throws an exception if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not found.</exception>
        public static string GetString(string key)
        {
            string stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"Missing required AppConfig setting '{key}'");
            }

            return stringValue;
        }

        /// <summary>
        /// Retrieves a string (formatted as a directory) from the App Config
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The properly full path of the directory string. Throws an exception if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not found.</exception>
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

        /// <summary>
        /// Retrieves an enum from the App Config
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The properly full path of the directory string. Throws an exception if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not found.</exception>
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

        /// <summary>
        /// Retrieves a bool from the App Config. Supports many different formats (t, T, 1, true, True).
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value bool. Throws an exception if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not found.</exception>
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
