using WC3CheatToolsSharedLib;

namespace WC3CheatInjector
{
    /// <summary>
    /// Manages the Application Configuration.
    /// </summary>
    class AppConfig
    {
        private const string CHEAT_PACK_DIR_KEY = "CheatPackDir";
        private const string CREATE_VERIFICATION_FILES_KEY = "CreateVerificationFiles";
        private const string LOG_LEVEL_KEY = "LogLevel";

        private const string INPUT_DIR_KEY = "InputDir";
        private const string MAP_FILTER_KEY = "MapFilter";
        private const string CHECK_SUBFOLDERS_KEY = "CheckSubfolders";
        private const string OUTPUT_DIR_KEY = "OutputDir";

        public string InputDir { get; private set; }
        public string MapFilter { get; private set; }
        public bool CheckSubfolders { get; private set; }

        public string OutputDir { get; private set; }

        public string CheatPackDir { get; private set; }
        public bool CreateVerificationFiles { get; private set; }
        public Logger.LogLevel LogLevel { get; private set; }

        public AppConfig()
        {
            CreateVerificationFiles = AppConfigUtil.GetBool(CREATE_VERIFICATION_FILES_KEY);
            CheatPackDir = AppConfigUtil.GetDirectory(CHEAT_PACK_DIR_KEY);
            LogLevel = AppConfigUtil.GetEnum<Logger.LogLevel>(LOG_LEVEL_KEY);
            InputDir = AppConfigUtil.GetDirectory(INPUT_DIR_KEY);
            MapFilter = AppConfigUtil.GetString(MAP_FILTER_KEY);
            CheckSubfolders = AppConfigUtil.GetBool(CHECK_SUBFOLDERS_KEY);
            OutputDir = AppConfigUtil.GetDirectory(OUTPUT_DIR_KEY);
        }
    }
}
