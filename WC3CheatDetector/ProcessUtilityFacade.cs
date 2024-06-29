using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using WC3CheatDetector.Models;
using WC3CheatDetector.Utils;

namespace WC3CheatDetector
{
    /// <summary>
    /// Facade for interacting with WC3CheatDetectorCpp.dll
    /// </summary>
    public class ProcessUtilityFacade
    {
        private const string DLL = "ProcessUtilityCpp.dll";

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        private static extern uint FindProcessId([MarshalAs(UnmanagedType.LPWStr)] string processName);


        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern bool FindOpenFile(uint processId, [MarshalAs(UnmanagedType.LPWStr)] string fileNameSubstring, StringBuilder fileDirBuffer, int bufferLength);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern uint GetLastPUError(StringBuilder messageBuffer, int bufferLength);

        /// <summary>
        /// Gets the Last Error returned from the Process Utility functions.
        /// </summary>
        /// <returns>The PUStatus containing the last error. The ErrorCode will be 0 if there was no errors.</returns>
        public PUStatus GetLastError()
        {
            StringBuilder messageBuffer = new StringBuilder(2048);
            uint status = GetLastPUError(messageBuffer, messageBuffer.Capacity);
            return new PUStatus(status, messageBuffer.ToString());
        }

        /// <summary>
        /// Gets the Process Id for a given process name.
        /// </summary>
        /// <param name="processName">The process name</param>
        /// <returns>
        ///     The processId if found. Returns 0 if not found or if there was an error. 
        ///     Use GetLastError() to get more error details if there was an error. 
        /// </returns>
        public uint GetProcessId(string processName)
        {
            return FindProcessId(processName);
        }

        /// <summary>
        /// Gets the filePath to an open (locked) file from a given process.
        /// </summary>
        /// <param name="processId">The processId that is locking the file.</param>
        /// <param name="fileNameSubstring">A substring of the filename of the locked file.</param>
        /// <param name="filePath">The output filepath if found or null if not.</param>
        /// <returns>
        ///     True if a file was found (and sets the filepath). False if the file was not found or if there was an error.
        ///     Use GetLastError() to get more error details if there was an error. 
        /// </returns>
        public bool GetOpenFile(uint processId, string fileNameSubstring, out string filePath)
        {
            StringBuilder fileDirBuffer = new StringBuilder(2048);
            bool success = FindOpenFile(processId, fileNameSubstring, fileDirBuffer, fileDirBuffer.Capacity);
            if (success)
            {
                string ntFileDir = fileDirBuffer.ToString();
                filePath = convertFromNTFilePath(ntFileDir);
                if (filePath == null)
                {
                    Logger.Error($"Could not convert NTFileDirectory to Win32FileDirectory. NTFileDir:'{ntFileDir}'");
                }
            }
            else
            {
                filePath = null;
            }

            return success;
        }

        /// <summary>
        /// Converts the NT style directory path to a Windows directory path.
        ///  Example NT Path: "\\Device\\HarddiskVolume2\\Users\\Eisen8\\Documents\\Warcraft III\\Maps\\Download\\test.w3x"
        ///  Example Windows Path: "C:\\Users\\Eisen8\\Documents\\Warcraft III\\Maps\\Download\\test.w3x"
        /// </summary>
        /// <param name="ntFilePath">The ntFilePath</param>
        /// <returns>The Windows directory path or null if unable to convert.</returns>
        private string convertFromNTFilePath(string ntFilePath)
        {
            // Guess the drive until we find the file. Then convert it.
            char currentDrive = 'A';
            while(currentDrive <= 'Z')
            {
                if (Directory.Exists(currentDrive + ":\\"))
                {
                    int index = ntFilePath.FindNthOccurence('\\', 3) + 1;
                    string newPath = currentDrive + ":\\" + ntFilePath[index..];
                    if (File.Exists(newPath))
                    {
                        return newPath;
                    }
                }
                currentDrive++;
            }

            return null;
        }
    }
}
