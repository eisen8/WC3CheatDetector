using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WC3CheatDetector.ProcessUtility;
using WC3CheatDetector.Utils;
using WC3CheatDetector.WhiteList;

namespace WC3CheatDetector
{
    /// <summary>
    /// WC3CheatDetector console app program
    /// </summary>
    class Program
    {
        private static List<WhiteListItem> _whiteList;
        private static List<BlackListItem> _blackList;

        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            try
            {
                // Setup App config and logger
                Logger.Log("Starting WC3CheatDetector");
                AppConfig appConfig = new AppConfig();
                Logger.SetLogLevel(appConfig.LogLevel);
                
                // Setup directories
                FileUtil.SetDirectories(appConfig.InputDir, appConfig.OutputDir);
                FileUtil.CreateDirectories();

                // Setup whitelist and blacklist
                loadWhiteAndBlackLists();

                // Run program
                checkMapsForCheats(appConfig);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            finally
            {
                FileUtil.Cleanup();
            }

            Logger.LogEmptyLine();
            Logger.Log("Program finished");
            Logger.Log($"Total Errors: {Logger.NumberOfErrors}. Total Warnings: {Logger.NumberOfWarnings}");
            Logger.Log("Press Enter to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Loads the whitelist and blacklist
        /// </summary>
        private static void loadWhiteAndBlackLists()
        {
            String whiteListJson = File.ReadAllText("./WhiteList/WhiteList.json");
            String blackListJson = File.ReadAllText("./WhiteList/BlackList.json");

            _whiteList = JsonConvert.DeserializeObject<List<WhiteListItem>>(whiteListJson);
            _blackList = JsonConvert.DeserializeObject<List<BlackListItem>>(blackListJson);
        }

        /// <summary>
        /// Checks the maps for cheats.
        /// </summary>
        /// <param name="appConfig">The app config</param>
        private static void checkMapsForCheats(AppConfig appConfig)
        {
            List<FileInfo> maps = new List<FileInfo>();

            if (appConfig.InGameMode)
            {
                // Get map based on current game
                Logger.Log("InGame Mode. Finding current game.");
                String filePath = getCurrentGameMapFilePath();
                maps.Add(new FileInfo(filePath));
            } else
            {
                // Get maps from input folder
                Logger.Log($"Directory Mode. Input={appConfig.InputDir}, Output={appConfig.OutputDir}.");
                maps = FileUtil.GetInputMaps(appConfig.MapFilter, appConfig.CheckSubfolders).ToList();
                Logger.Log($"{maps.Count} input map files found");
                if (maps.Count == 0)
                {
                    Logger.Warn($"No WC3 maps (.w3x) after filter '{appConfig.MapFilter}' found in input directory. Place maps in the input directory {appConfig.InputDir} and rerun program.");
                    return;
                }
            }

            // Find cheats for each map
            CheatDetector cf = new CheatDetector();
            for (int i = 0; i < maps.Count; i++)
            {
                FileInfo map = maps.ElementAt(i);
                string tempMapFilePath = FileUtil.CopyToTemp(map.FullName);
                string hash = FileUtil.CalculateFileMD5Hash(tempMapFilePath);
                int fileSizeKB = FileUtil.CalculateFileSizeKB(map);
                Logger.LogEmptyLine();
                Logger.Log($"---- #{i + 1} --- {map.Name} --- {fileSizeKB} kb");
                Logger.Log($"---- MD5: {hash}");
                checkWhiteAndBlackLists(hash);
                cf.FindCheats(tempMapFilePath);
            }
        }

        /// <summary>
        /// Uses ProcessUtility to the find the current game's file path.
        /// </summary>
        /// <returns>The file path to the map.</returns>
        private static String getCurrentGameMapFilePath()
        {
            string filePath = null;
            uint pid = 0;
            bool success;
            int retries;

            ProcessUtilityFacade pu = new ProcessUtilityFacade();

            success = false;
            retries = 3;
            while (!success)
            {
                pid = pu.GetProcessId("Warcraft III.exe");
                success = (pid != 0);
                if (!success && retries == 0)
                {
                    PUStatus error = pu.GetLastError();
                    if (error.ErrorCode == 0)
                    {
                        Logger.Error("Could not find Warcraft III.exe. Is the game running?");
                    }
                    else
                    {
                        Logger.Error($"Internal Error getting the PID of Warcraft III.exe. ErrorCode: {error.ErrorCode}. ErrorMessage: {error.ErrorMessage}");
                    }
                    return null;
                }

                retries--;
            }

            success = false;
            retries = 3;
            while (!success)
            {
                success = pu.GetOpenFile(pid, ".w3x", out filePath);
                if (!success || String.IsNullOrWhiteSpace(filePath) && retries == 0)
                {
                    PUStatus error = pu.GetLastError();
                    if (error.ErrorCode == 0)
                    {
                        Logger.Error("Could not find the .w3x file of the current game. Are you in a game? Note: Sometimes WC3 doesn't lock the file and you need to restart your computer.");
                    }
                    else
                    {
                        Logger.Error($"Internal Error getting .w3x file of current game. ErrorCode: {error.ErrorCode}. ErrorMessage: {error.ErrorMessage}");
                    }
                    return null;
                }

                retries--;
            }

            return filePath;
        }

        /// <summary>
        /// Checks if a map MD5 hash is in the whitelist or the blacklist.
        /// </summary>
        /// <param name="hash"></param>
        private static void checkWhiteAndBlackLists(String hash)
        {
            WhiteListItem wl = _whiteList.Find(x => x.Hash.Equals(hash));
            if (wl != null)
            {
                Logger.Log($"** Map is whitelisted. **");
            }
            BlackListItem bl = _blackList.Find(x => x.Hash.Equals(hash));
            if (bl != null)
            {
                Logger.Log($"** Map is blackListed. CheatPack: {bl.CheatPack}, Activator: {bl.Activator}. **");
            }
        }
    }
}
