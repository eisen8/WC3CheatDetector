using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WC3CheatDetector
{
    /// <summary>
    /// Facade for interacting with MPQ archives using StormLib.dll. See http://zezula.net/en/mpq/stormlib.html for details on the functions.
    /// </summary>
    public class MPQFacade
    {
        // These values come from StormLib.h
        private const uint MPQ_COMPRESSION_NEXT_SAME = 0xFFFFFFFF;
        private const uint MPQ_FILE_COMPRESS = 0x00000200;
        private const uint MPQ_FILE_REPLACE_EXISTING = 0x80000000;
        private const uint MPQ_COMPRESSION_PKWARE = 0x08;


        /// <summary>
        /// Opens an MPQ archive from a WC3 map file.
        /// </summary>
        /// <param name="filePath">The file path to the map</param>
        /// <param name="archiveHandle">Outputs a handle to the archive.</param>
        /// <returns>True if successful and archiveHandle is set. On an error, the function returns false and GetLastError returns an error code.</returns>
        public bool OpenArchive(string filePath, out IntPtr archiveHandle)
        {
            return StormLib.SFileOpenArchive(filePath, 0, 0x0000, out archiveHandle);
        }

        /// <summary>
        /// Compacts (rebuilds) an MPQ archive
        /// </summary>
        /// <param name="archiveHandle">Handle to the MPQ archive</param>
        /// <returns>True if successful. On an error, the function returns false and GetLastError returns an error code.</returns>
        public bool CompactArchive(IntPtr archiveHandle)
        {
            return StormLib.SFileCompactArchive(archiveHandle);
        }

        /// <summary>
        /// Closes an MPQ archive. Any unsaved MPQ tables are saved to the archive.
        /// </summary>
        /// <param name="archiveHandle">Handle to the MPQ archive</param>
        /// <returns>True if successful. On an error, the function returns false and GetLastError returns an error code.</returns>
        public bool CloseArchive(IntPtr archiveHandle)
        {
            return StormLib.SFileCloseArchive(archiveHandle);
        }

        /// <summary>
        ///  Exracts a file from the MPQ archive
        /// </summary>
        /// <param name="archiveHandle">Handle to the MPQ archive</param>
        /// <param name="sourceMPQFilePath">Full path of the file within the MPQ archive to extract from</param>
        /// <param name="extractedFilePath">A new file location (including name) to extract the file to. This function will copy the file from the MPQ Archive and create a local file.</param>
        /// <returns>True if successful and copies the extracted file to the extractedFilePath. On an error, the function returns false and GetLastError returns an error code.</returns>
        public bool ExtractFile(IntPtr archiveHandle, string sourceMPQFilePath, string extractedFilePath)
        {
            return StormLib.SFileExtractFile(archiveHandle, sourceMPQFilePath, extractedFilePath, 0x00000000);
        }

        /// <summary>
        /// Adds a file to the MPQ Archive. Will overwrite a file if it already exists in the Archive.
        /// </summary>
        /// <param name="archiveHandle">Handle to the MPQ archive</param>
        /// <param name="sourceFilePath">The file path of the file to save to the MPQ archive.</param>
        /// <param name="mpqArchivePath">The full path within the MPQ archive to save the file to (including name).</param>
        /// <returns>True if successful. On an error, the function returns false and GetLastError returns an error code.</returns>
        public bool SaveFile(IntPtr archiveHandle, string sourceFilePath, string mpqArchivePath)
        {
            uint dwFlags = MPQ_FILE_REPLACE_EXISTING | MPQ_FILE_COMPRESS;
            return StormLib.SFileAddFileEx(archiveHandle, sourceFilePath, mpqArchivePath, dwFlags, MPQ_COMPRESSION_PKWARE, MPQ_COMPRESSION_NEXT_SAME);
        }


        /// <summary>
        /// Retrieves the last Win32 error code.
        /// </summary>
        /// <returns>Nonzero if there is an error code.</returns>
        public int GetErrorCode()
        {
            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// Retrieves the last Win32 error code and the corresponding message.
        /// </summary>
        /// <param name="message">The message to be set if there is one.</param>
        /// <returns>Nonzero if there is an error code. The message will be set to the error message if there is an error message.</returns>
        public int GetErrorCodeAndMessage(out string message)
        {
            int errorCode = Marshal.GetLastWin32Error();
            message = new Win32Exception(errorCode).Message;

            return errorCode;
        }

        [Flags]
        public enum VERIFY_FILE_ERRORS
        {
            OPEN_ERROR = 0x0001, // Failed to open the file
            READ_ERROR = 0x0002,  // Failed to read all data from the file
            FILE_HAS_SECTOR_CRC = 0x0004, // File has sector CRC
            FILE_SECTOR_CRC_ERROR = 0x0008, // Sector CRC check failed
            FILE_HAS_CHECKSUM = 0x0010, // File has CRC32
            FILE_CHECKSUM_ERROR = 0x0020, // CRC32 check failed
            FILE_HAS_MD5 = 0x0040, // File has data MD5
            FILE_MD5_ERROR = 0x0080, // MD5 check failed
            FILE_HAS_RAW_MD5 = 0x0100 // File has raw data MD5
        }
    }
}
