using System;
using System.IO;
using System.Text.RegularExpressions;
using WC3CheatToolsSharedLib;
using WC3CheatToolsSharedLib.Models;

namespace WC3CheatInjector
{
    /// <summary>
    /// Injects the CheatPack into the wc3 map file.
    /// </summary>
    class CheatInjectorHandler
    {
        private const RegexOptions MULTILINE_IC = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        private readonly WC3MapJassFileModifier _modifer = new WC3MapJassFileModifier();
        private string _globals;
        private string _endGlobals;
        private string _main;

        public bool InjectCheats(string mapFilePath, string cheatPackDir, bool createVerificationFiles)
        {
            string globalsFilePath = cheatPackDir + "globals.txt";
            string endGlobalsFilePath = cheatPackDir + "endglobals.txt";
            string mainFilePath = cheatPackDir + "main.txt";

            if (!Directory.Exists(cheatPackDir))
            {
                throw new DirectoryNotFoundException($"Could not find CheatPack Directory {cheatPackDir}");
            }
            if (!File.Exists(globalsFilePath))
            {
                throw new FileNotFoundException($"Could not find globals.txt file in CheatPack Directory {cheatPackDir}");
            }
            if (!File.Exists(endGlobalsFilePath))
            {
                throw new FileNotFoundException($"Could not find endglobals.txt file in CheatPack Directory {cheatPackDir}");
            }
            if (!File.Exists(mainFilePath))
            {
                throw new FileNotFoundException($"Could not find main.txt file in CheatPack Directory {cheatPackDir}");
            }

            _globals = File.ReadAllText(globalsFilePath) + "\n";
            _endGlobals = File.ReadAllText(endGlobalsFilePath) + "\n";
            _main = File.ReadAllText(mainFilePath) + "\n";

            return _modifer.UpdateJASSFile(jassModiferFunc, mapFilePath, createVerificationFiles);
        }

        private string jassModiferFunc(JASSSearchHelper inputHelper, string mapName)
        {
            int insertIndex;

            // Inject globals
            JRMatch r = inputHelper.FindMatch(@"^globals$", MULTILINE_IC);
            if (!r.Success)
            {
                throw new FormatException("Could not find globals section of JASS file");
            }
            insertIndex = r.Index + r.Value.Length + 1;
            inputHelper.Insert(insertIndex, _globals);

            // Inject endglobals
            r = inputHelper.FindMatch(@"^endglobals$", MULTILINE_IC);
            if (!r.Success)
            {
                throw new FormatException("Could not find endglobals section of JASS file");
            }

            insertIndex = r.Index + r.Value.Length + 1;
            inputHelper.Insert(insertIndex, _endGlobals);

            // Inject main (after last local declaration)
            r = inputHelper.FindMatch(@"^function main takes nothing returns nothing$", MULTILINE_IC);
            if (!r.Success)
            {
                throw new FormatException("Could not find main function section of JASS file");
            }

            int mainStartIndex = r.Index + r.Value.Length + 1;

            // find first line after main that doesn't start with local
            r = inputHelper.FindMatch(@"^(?!local).+$", mainStartIndex, MULTILINE_IC);
            if (!r.Success)
            {
                throw new FormatException("Could not find non-local line in main function section of JASS file");
            }

            inputHelper.Insert(r.Index, _main);

            return inputHelper.Input;
        }
    }
}
