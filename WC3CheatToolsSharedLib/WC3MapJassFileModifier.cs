using System;
using System.IO;

namespace WC3CheatToolsSharedLib
{
    /// <summary>
    /// Modifies the Warcraft III Map war3map JASS file.
    /// </summary>
    public class WC3MapJassFileModifier
    {
        private const string JASS_FILE_NAME = "war3map.j";
        private const string SCRIPTS_JASS_FILE_PATH = "scripts\\war3map.j";

        private readonly MPQFacade _mpq;

        public WC3MapJassFileModifier()
        {
            _mpq = new MPQFacade();
        }

        /// <summary>
        /// Function for reading the war3map JASS file.
        /// </summary>
        /// <param name="jassReaderFunc">The function to invoke for reading the JASS. Takes in a JassSearchHelper and the mapfile name.</param>
        /// <param name="mapFilePath">The path to the map file to read.</param>
        /// <returns>True if successful or false if there was an issue.</returns>
        public bool ReadJASSFile(Action<JASSSearchHelper, string> jassReaderFunc, string mapFilePath)
        {
            string funcWrapper(JASSSearchHelper x, string y) { jassReaderFunc(x, y); return null; }
            return UpdateJASSFile(funcWrapper, mapFilePath);
        }

        /// <summary>
        /// Function for updating the war3map JASS file.
        /// </summary>
        /// <param name="jassModifierFunc">The function to invoke for reading the JASS. Takes in a JassSearchHelper and the mapfile name. Returns the modififed JASS.</param>
        /// <param name="mapFilePath">The path to the map file to read.</param>
        /// <param name="createVerificationFiles">Whether to create corresponding JASS files in the ouput for manual verification.</param>
        /// <returns>True if successful or false if there was an issue.</returns>
        public bool UpdateJASSFile(Func<JASSSearchHelper, string, string> jassModifierFunc, string mapFilePath, bool createVerificationFiles = true)
        {
            /**
            Note: These APIs commonly give back error codes to maps for a variety of reasons that don't necessarily mean we can't proceed. The general strategy
            here will be to continue even with error codes unless it becomes impossible to continue.
            **/

            bool success = _mpq.OpenArchive(mapFilePath, out IntPtr archive);

            try
            {
                if (!success || archive.Equals(IntPtr.Zero))
                {
                    Logger.Error($"Could not open MPQ archive for {mapFilePath}. Error Code: {_mpq.GetErrorCode()}");
                    success = false;
                }
                else
                {
                    success = updateJASS(jassModifierFunc, mapFilePath, createVerificationFiles, archive);
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

            return success;
        }

        private bool updateJASS(Func<JASSSearchHelper, string, string> jassModifierFunc, string mapFilePath, bool createVerificationFiles, IntPtr archive)
        {
            string mapName = Path.GetFileNameWithoutExtension(mapFilePath);

            // Open the archive file
            string tempJassFilePath = FileUtil.CreateTempFilePath(mapName + ".j");
            _mpq.ExtractFile(archive, JASS_FILE_NAME, tempJassFilePath);
            if (!File.Exists(tempJassFilePath))
            {
                // Sometimes the Jass file is in the scripts folder
                _mpq.ExtractFile(archive, SCRIPTS_JASS_FILE_PATH, tempJassFilePath);
                if (!File.Exists(tempJassFilePath))
                {
                    Logger.Error($"Could not extract MPQ file {JASS_FILE_NAME} for {mapFilePath}");
                    return false;
                }
            }

            // Jass
            string jass = File.ReadAllText(tempJassFilePath);

            JASSSearchHelper inputJassHelper = new JASSSearchHelper(jass);
            string postJass;
            try
            {
                postJass = jassModifierFunc.Invoke(inputJassHelper, mapName);
            }
            catch (Exception e)
            {
                Logger.Error($"Error while working on jass file. {e}");
                return false;
            }

            if (postJass == null || jass.Equals(postJass))
            {
                if (createVerificationFiles)
                {
                    FileUtil.CreateOutputFile(mapName + "_J.txt", jass);
                }
                return true; // No changes were made. So no need to modify the file.
            }

            // Create JASS files
            if (createVerificationFiles)
            {
                FileUtil.CreateOutputFile(mapName + "_PreJ.txt", jass);
                FileUtil.CreateOutputFile(mapName + "_PostJ.txt", postJass);
            }

            File.WriteAllText(tempJassFilePath, postJass); // Overwrite temp Jass file with changes

            // Update MPQ with modified jass file
            bool success = _mpq.SaveFile(archive, tempJassFilePath, JASS_FILE_NAME);
            int errorCode = _mpq.GetErrorCode();
            if (!success)
            {
                Logger.Error($"Could not re-save MPQ file {JASS_FILE_NAME} after disabling cheats for {mapFilePath}. Error Code: {errorCode}");
                return false;
            }

            // Verify the MPQ file
            success = _mpq.VerifyFile(archive, tempJassFilePath);
            errorCode = _mpq.GetErrorCode();
            if (!success)
            {
                Logger.Warn($"Verification of {JASS_FILE_NAME} after removing cheats for {mapFilePath} failed. Error Code: {errorCode}");
            }


            // Optional step of compacting the archive
            success = _mpq.CompactArchive(archive);
            errorCode = _mpq.GetErrorCode();
            if (!success)
            {
                Logger.Warn($"Error compacting MPQ archive {mapFilePath}. Error Code: {errorCode}");
            }

            return true;
        }
    }
}
