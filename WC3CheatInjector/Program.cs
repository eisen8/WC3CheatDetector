using System;
using System.IO;
using WC3CheatToolsSharedLib;

namespace WC3CheatInjector
{
    class Program
    {
        public static void Main()
        {
            try
            {
                run();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            Logger.LogEmptyLine();
            Logger.Log("Program finished");
            Logger.Log($"Total Errors: {Logger.NumberOfErrors}. Total Warnings: {Logger.NumberOfWarnings}");
            Logger.Log("Press Enter to exit");
            Console.ReadKey();
        }

        private static void run()
        {
            AppConfig appConfig = new AppConfig();
            Logger.SetLogLevel(appConfig.LogLevel);
            Logger.Log("Starting WC3CheatInjector");

            //Create Directories and get maps
            CheatInjectorHandler cpInjector = new CheatInjectorHandler();
            try
            {
                FileUtil.SetDirectories(appConfig.InputDir, appConfig.OutputDir);
                FileUtil.CreateDirectories();
                FileInfo[] maps = FileUtil.GetInputMaps(appConfig.MapFilter, appConfig.CheckSubfolders);
                Logger.Log($"{maps.Length} input map files found");
                if (maps.Length == 0)
                {
                    Logger.Warn($"No WC3 maps (.w3x) found in input directory. Place maps in the input directory {appConfig.InputDir} and rerun program.");
                    return;
                }

                // Inject cheats for each map
                for (int i = 0; i < maps.Length; i++)
                {
                    FileInfo map = maps[i];
                    Logger.Log($"---- #{i + 1} --- {map.Name}");
                    Logger.Log($"Processing map");
                    string tempMapFilePath = FileUtil.CopyToTemp(map.FullName);
                    bool success = cpInjector.InjectCheats(tempMapFilePath, appConfig.CheatPackDir, appConfig.CreateVerificationFiles);
                    if (success)
                    {
                        Logger.Log("Cheats Injected");
                        FileUtil.CopyToOutput(tempMapFilePath);
                    }
                }
            }
            finally
            {
                FileUtil.Cleanup();
            }
        }
    }
}
