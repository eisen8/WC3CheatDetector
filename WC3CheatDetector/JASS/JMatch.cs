using System;

namespace WC3CheatDetector.JASS
{
    /// <summary>
    /// Represents a Match from the JassRegexHelper
    /// </summary>
    public class JMatch
    {
        public string Value { get; private set; }
        public int Index { get; private set; }
        public bool Success { get; private set; }
        public int Length { get { return Value.Length; } }

        public JMatch(string value, int index)
        {
            Value = value ?? throw new ArgumentNullException("value", "value cannot be null");
            Index = index;
            Success = true;
        }

        /// <summary>
        /// Constructor for unsuccessful match
        /// </summary>
        public JMatch()
        {
            Value = string.Empty;
            Index = 0;
            Success = false;
        }
    }
}
