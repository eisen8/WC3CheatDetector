using System.Collections.Generic;
using System.Linq;
using WC3CheatDetector.Utils;

namespace WC3CheatDetector.JASS
{
    /// <summary>
    /// Holds the warnings and suspicious lines found related to the JASS file. 
    /// </summary>
    class JSus
    {
        private readonly List<string> _warnings;
        private readonly List<string> _suspiciousLines;

        public JSus()
        {
            _warnings = new List<string>();
            _suspiciousLines = new List<string>();
        }


        public void AddSusLine(string line)
        {
            _suspiciousLines.Add(line);
        }

        public void AddWarning(string warning)
        {
            _warnings.Add(warning);
        }

        public void LogWarnings()
        {
            foreach (string w in getWarnings())
            {
                Logger.Log(w);
            }
        }

        /// <summary>
        /// Creates the JSus file (JASS suspicious file), a file that contains only the most suspicious lines
        /// from the JASS file.
        /// </summary>
        /// <param name="mapName"></param>
        public void CreateJSusFile(string mapName)
        {
            FileUtil.CreateOutputFile(mapName + "_JSus.txt", getSusLines());
        }

        private List<string> getWarnings()
        {
            return _warnings;
        }

        private string getSusLines()
        {
            return string.Join('\n', _suspiciousLines.Distinct());
        }
    }
}
