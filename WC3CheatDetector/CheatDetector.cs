using System.Collections.Generic;
using System.IO;
using WC3CheatDetector.JASS;
using WC3CheatDetector.Utils;

namespace WC3CheatDetector
{
    /// <summary>
    /// Class for finding cheats in wc3 map files.
    /// </summary>
    class CheatDetector
    {
        private readonly JASSReader _reader;

        public CheatDetector()
        {
            _reader = new JASSReader();
        }

        /// <summary>
        /// Checks a map at a given file path for cheats.
        /// </summary>
        /// <param name="mapFilePath"></param>
        /// <returns></returns>
        public void FindCheats(string mapFilePath)
        {
            string mapName = Path.GetFileNameWithoutExtension(mapFilePath);
            string JASS = _reader.ReadJASSFile(mapFilePath, mapName);
            if (!string.IsNullOrWhiteSpace(JASS))
            {
                // Creates the raw JASS file
                FileUtil.CreateOutputFile(mapName + "_J.txt", JASS);

                // Search the JASS for cheats
                JSearch h = new JSearch(JASS);
                JSus sm = new JSus();

                // Rating of 0-9 per check. Higher rating indicates more likely to be a cheated map and less likely to be an uncheated map. A cheated map will likely
                // trigger many low rated checks and a few high rated whereas an uncheated map will only likely trigger a few low rated checks.
                checkContains(h, sm, "Dekar", "Map References Dekar.", 9); // Dekar's cheatpack
                checkContainsAny(h, sm, new List<string> { "fai_YauFei", "vfai" }, "Map References Fai.", 9); // Fai's cheatpack
                checkContains(h, sm, "FatherSpace", "Map References FatherSpace.", 9); // FatherSpace's cheatpack
                checkContains(h, sm, "Fukki", "Map References Fukki.", 9); // Fukki's cheatpack
                checkContains(h, sm, "HaxoRico", "Map References HaxoRico.", 9); // HaxoRico's cheatpack
                checkContains(h, sm, "hke_", "Map References HKE.", 9); // HKE's cheatpack
                checkContains(h, sm, "JJ2197", "Map References JJ2197.", 9); // JJ's cheatpack
                checkContainsAny(h, sm, new List<string> { "nzHash", "Nuza" }, "Map References Nuza.", 9); // Nuza's cheatpack
                checkContainsAny(h, sm, new List<string> { "Sabrac", "sbrkw" }, "Map References Subrac.", 9); // Sabrac's cheatpack (type any 5 of the activator words)
                checkContains(h, sm, "Wc3Edit", "Map References Wc3Edit.", 9); // Generic Wc3Edit cheatpack

                checkContains(h, sm, "DoNotSaveReplay", "Map contains DoNotSaveReplay.", 9);

                checkContains(h, sm, "StringHash", "Map contains StringHash.", 8);
                checkContains(h, sm, "CheatPack", "Map contains the word CheatPack.", 8);
                checkContains(h, sm, "Activator", "Map references an Activator", 7);

                checkContainsAny(h, sm, COMMON_COMMANDS, "Map contains very common cheat commands words.", 7, false);

                checkContains(h, sm, "ForceAddPlayer", "Map contains ForceAddPlayer.", 4);
                checkContains(h, sm, "GetPlayerName", "Map contains GetPlayerName.", 4);
                checkContainsAny(h, sm, EVENT_ARROW_KEYS, "Map uses Player Arrow Key Events.", 4);

                checkContains(h, sm, "GetEventPlayerChatString", "Map uses GetEventPlayerChatString.", 2);
                checkContains(h, sm, "TriggerRegisterPlayerChatEvent", "Map uses TriggerRegisterPlayerChatEvent.", 2);

                // Log and create JSus file
                sm.LogWarnings();
                sm.CreateJSusFile(mapName);
            }
        }

        /// <summary>
        /// Checks if the JASS contains a specific substream.
        /// </summary>
        /// <param name="j">The JSearch string to check</param>
        /// <param name="sf">The JSus object to add to</param>
        /// <param name="substring">The substring we are seraching for.</param>
        /// <param name="warningMessage">A warning to give to the player if the text is found.</param>
        /// <param name="rating">The rating of the warning (0 through 9... 9 being very high likely to be cheated))</param>
        /// <param name="ignoreWhiteSpace">Whether to ignore whitespace while searching or not</param>
        /// <returns>True if it contains the substring or false if not</returns>
        private bool checkContains(JSearch j, JSus sf, string substring, string warningMessage, int rating, bool ignoreWhiteSpace = true)
        {
            List<string> lines = j.GetContains(substring, ignoreWhiteSpace);
            if (lines.Count > 0)
            {
                // We only log the corresponding cheat warning once in the console (even if multiple lines match) but all matching lines get logged in the JSus file.
                if (!string.IsNullOrWhiteSpace(warningMessage))
                {
                    sf.AddWarning(rating + ": " + warningMessage);
                }

                foreach (string line in lines)
                {
                    sf.AddSusLine(line);
                }
            }

            return lines.Count > 0;
        }

        /// <summary>
        /// Checks if the JASS contains any of a list of substrings
        /// </summary>
        /// <param name="j">The JSearch string to check</param>
        /// <param name="sf">The JSus object to add to</param>
        /// <param name="substrings">The substrings we are seraching for.</param>
        /// <param name="warningMessage">A warning to give to the player if the text is found.</param>
        /// <param name="rating">The rating of the warning (0 through 9... 9 being very high likely to be cheated))</param>
        /// <param name="ignoreWhiteSpace">Whether to ignore whitespace while searching or not</param>
        /// <returns>True if it contains the substring or false if not</returns>
        private bool checkContainsAny(JSearch j, JSus sf, List<string> substrings, string warningMessage, int rating, bool ignoreWhiteSpace = true)
        {
            List<string> lines = j.GetContainsAny(substrings, ignoreWhiteSpace);
            if (!lines.IsEmpty())
            {
                if (!string.IsNullOrWhiteSpace(warningMessage))
                {
                    sf.AddWarning(rating + ": " + warningMessage);
                }
                foreach (string line in lines)
                {
                    sf.AddSusLine(line);
                }
            }

            return !lines.IsEmpty();
        }

        private static readonly List<string> EVENT_ARROW_KEYS = new List<string> { "EVENT_PLAYER_ARROW_UP", "EVENT_PLAYER_ARROW_DOWN", "EVENT_PLAYER_ARROW_LEFT", "EVENT_PLAYER_ARROW_RIGHT" };
        private static readonly List<string> COMMON_COMMANDS = new List<string> { "-kill", ".kill", "-hear", ".hear", "-maphack", ".maphack" };
    }
}
