namespace WC3CheatDetector.Utils
{
    /// <summary>
    /// Manages the Application Configuration.
    /// </summary>
    class AppConfig
    {
        private const string IN_GAME_MODE_KEY = "InGameMode";
        private const string INPUT_DIR_KEY = "InputDir";
        private const string MAP_FILTER_KEY = "MapFilter";
        private const string CHECK_SUBFOLDERS_KEY = "CheckSubfolders";

        private const string OUTPUT_DIR_KEY = "OutputDir";

        private const string LOG_LEVEL_KEY = "LogLevel";

        public bool InGameMode { get; private set; }

        public string InputDir { get; private set; }
        public string MapFilter { get; private set; }

        public bool CheckSubfolders { get; private set; }

        public string OutputDir { get; private set; }

        public Logger.LogLevel LogLevel { get; private set; }

        public AppConfig()
        {
            InGameMode = AppConfigUtil.GetBool(IN_GAME_MODE_KEY);
            InputDir = AppConfigUtil.GetDirectory(INPUT_DIR_KEY);
            MapFilter = AppConfigUtil.GetString(MAP_FILTER_KEY);
            if (string.IsNullOrWhiteSpace(MapFilter))
            {
                MapFilter = "*";
            }

            CheckSubfolders = AppConfigUtil.GetBool(CHECK_SUBFOLDERS_KEY);
            OutputDir = AppConfigUtil.GetDirectory(OUTPUT_DIR_KEY);
            LogLevel = AppConfigUtil.GetEnum<Logger.LogLevel>(LOG_LEVEL_KEY);
        }
    }
}
