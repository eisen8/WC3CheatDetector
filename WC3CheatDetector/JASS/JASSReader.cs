using System;
using System.IO;
using WC3CheatDetector.Utils;

namespace WC3CheatDetector.JASS
{
    /// <summary>
    /// Class for opening and reading the Warcraft III Map war3map JASS file.
    /// </summary>
    public class JASSReader
    {
        private const string JASS_FILE_NAME = "war3map.j";
        private const string SCRIPTS_JASS_FILE_PATH = "scripts\\war3map.j";

        private readonly MPQFacade _mpq;

        public JASSReader()
        {
            _mpq = new MPQFacade();
        }

        /// <summary>
        /// Function for reading the war3map JASS file.
        /// </summary>
        /// <param name="mapFilePath">The path to the map file to read.</param>
        /// <param name="mapName">The name of the map</param>
        /// <returns>The JASS file text or null if unsuccessful.</returns>
        public String ReadJASSFile(string mapFilePath, string mapName)
        {
            bool success = _mpq.OpenArchive(mapFilePath, out IntPtr archive);

            try
            {
                if (!success || archive.Equals(IntPtr.Zero))
                {
                    Logger.Error($"Could not open MPQ archive for {mapFilePath}. Error Code: {_mpq.GetErrorCode()}");
                    return null;
                }
                else
                {
                    // Extract the JASS file and put it in a new temporary file
                    string tempJassFilePath = FileUtil.CreateTempFilePath(mapName + ".j");
                    _mpq.ExtractFile(archive, JASS_FILE_NAME, tempJassFilePath);
                    if (!File.Exists(tempJassFilePath))
                    {
                        // Sometimes the Jass file is in the scripts folder
                        _mpq.ExtractFile(archive, SCRIPTS_JASS_FILE_PATH, tempJassFilePath);
                        if (!File.Exists(tempJassFilePath))
                        {
                            Logger.Error($"Could not extract MPQ file {JASS_FILE_NAME} for {mapFilePath}");
                            return null;
                        }
                    }

                    // The JASS
                    return File.ReadAllText(tempJassFilePath);
                }
            }
            finally
            {
                // Ensure we close the archive
                if (!archive.Equals(IntPtr.Zero))
                {
                    _mpq.CloseArchive(archive);
                }
            }
        }
    }
}
