using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WC3CheatDetector.Models;
using WC3CheatDetector.Utils;
using WC3CheatDetector.WhiteList;

namespace WC3CheatDetector
{
    class Program
    {
        private static List<WhiteListItem> _whiteList;
        private static List<BlackListItem> _blackList;

        public static void Main()
        {
            try
            {
                AppConfig appConfig = new AppConfig();
                Logger.SetLogLevel(appConfig.LogLevel);
                Logger.Log("Starting WC3CheatDetector");
                FileUtil.SetDirectories(appConfig.InputDir, appConfig.OutputDir);
                FileUtil.CreateDirectories();
                loadWhiteList();

                if (appConfig.InGameMode)
                {
                    Logger.Log("InGame Mode. Finding current game.");
                    runInGameMode();
                }
                else
                {
                    Logger.Log($"Directory Mode. Input={appConfig.InputDir}, Output={appConfig.OutputDir}.");
                    runDirMod(appConfig);
                }
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
        private static void loadWhiteList()
        {
            String whiteListJson = File.ReadAllText("./WhiteList/WhiteList.json");
            String blackListJson = File.ReadAllText("./WhiteList/BlackList.json");

            _whiteList = JsonConvert.DeserializeObject<List<WhiteListItem>>(whiteListJson);
            _blackList = JsonConvert.DeserializeObject<List<BlackListItem>>(blackListJson);
        }

        private static void runDirMod(AppConfig appConfig)
        {
            // Get maps from input folder
            FileInfo[] maps = FileUtil.GetInputMaps(appConfig.MapFilter, appConfig.CheckSubfolders);
            Logger.Log($"{maps.Length} input map files found");
            if (maps.Length == 0)
            {
                Logger.Warn($"No WC3 maps (.w3x) after filter '{appConfig.MapFilter}' found in input directory. Place maps in the input directory {appConfig.InputDir} and rerun program.");
                return;
            }

            // Find cheats for each map
            CheatFinderHandler cf = new CheatFinderHandler();
            for (int i = 0; i < maps.Length; i++)
            {
                FileInfo map = maps[i];
                string tempMapFilePath = FileUtil.CopyToTemp(map.FullName);
                string hash = FileUtil.CalculateFileMD5Hash(tempMapFilePath);
                int fileSizeKB = FileUtil.CalculateFileSizeKB(map);
                Logger.LogEmptyLine();
                Logger.Log($"---- #{i + 1} --- {map.Name} --- {fileSizeKB} kb");
                Logger.Log($"MD5: {hash}");
                checkWhiteAndBlackLists(hash);
                cf.FindCheats(tempMapFilePath);
            }
        }

        private static void runInGameMode()
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
                    return;
                }

                retries--;
            }

            success = false;
            retries = 3;
            while (!success)
            {
                success = pu.GetOpenFile(pid, ".w3x", out filePath);
                if (!success || string.IsNullOrWhiteSpace(filePath) && retries == 0)
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
                    return;
                }

                retries--;
            }

            CheatFinderHandler cf = new CheatFinderHandler();
            string tempMapFilePath = FileUtil.CopyToTemp(filePath);
            FileInfo map = new FileInfo(tempMapFilePath);
            string hash = FileUtil.CalculateFileMD5Hash(tempMapFilePath);
            int fileSizeKB = FileUtil.CalculateFileSizeKB(map);
            Logger.Log($"Processing InGame map {map.Name} --- {fileSizeKB} kb");
            Logger.Log($"MD5: {hash}");
            checkWhiteAndBlackLists(hash);
            cf.FindCheats(tempMapFilePath);
        }

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
