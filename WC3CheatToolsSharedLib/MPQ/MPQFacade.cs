using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WC3CheatToolsSharedLib
{
    /// <summary>
    /// Facade for interacting with MPQ archives. See http://zezula.net/en/mpq/stormlib.html for details on the functions.
    /// </summary>
    public class MPQFacade
    {
        // These values come from StormLib.h
        private const uint MPQ_COMPRESSION_NEXT_SAME = 0xFFFFFFFF;
        private const uint MPQ_FILE_COMPRESS = 0x00000200;
        private const uint MPQ_FILE_REPLACE_EXISTING = 0x80000000;
        private const uint MPQ_COMPRESSION_PKWARE = 0x08;


        public bool OpenArchive(string filePath, out IntPtr archiveHandle)
        {
            return StormLib.SFileOpenArchive(filePath, 0, 0x0000, out archiveHandle);
        }

        public bool CompactArchive(IntPtr archiveHandle)
        {
            return StormLib.SFileCompactArchive(archiveHandle);
        }

        public bool CloseArchive(IntPtr archiveHandle)
        {
            return StormLib.SFileCloseArchive(archiveHandle);
        }

        public bool ExtractFile(IntPtr archiveHandle, string sourceFile, string extractedFile)
        {
            return StormLib.SFileExtractFile(archiveHandle, sourceFile, extractedFile, 0x00000000);
        }

        public bool SaveFile(IntPtr archiveHandle, string sourceFile, string archivedName)
        {
            uint dwFlags = MPQ_FILE_REPLACE_EXISTING | MPQ_FILE_COMPRESS;
            return StormLib.SFileAddFileEx(archiveHandle, sourceFile, archivedName, dwFlags, MPQ_COMPRESSION_PKWARE, MPQ_COMPRESSION_NEXT_SAME);
        }

        public bool RemoveFile(IntPtr archiveHandle, string sourceFile)
        {
            return StormLib.SFileRemoveFile(archiveHandle, sourceFile, 0);
        }

        public bool VerifyFile(IntPtr archiveHandle, string filePath)
        {
            uint dwFlags = 0x0000000F; // Verify for all errors
            return StormLib.SFileVerifyFile(archiveHandle, filePath, dwFlags);
        }

        public int GetErrorCode()
        {
            return Marshal.GetLastWin32Error();
        }

        public int GetErrorCodeAndMessage(out string message)
        {
            int errorCode = Marshal.GetLastWin32Error();
            message = new Win32Exception(errorCode).Message;
            return Marshal.GetLastWin32Error();
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
