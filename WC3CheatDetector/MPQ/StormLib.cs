using System;
using System.Runtime.InteropServices;

namespace WC3CheatDetector
{
    /// <summary>
    /// Native calls to the Stormlib. See http://www.zezula.net/en/mpq/stormlib.html for more detailed information about these calls.
    /// </summary>
    class StormLib
    {
        private const string STORMLIB_DLL = "stormlib.dll";

        /// <summary>
        /// Opens an MPQ archive
        /// </summary>
        /// <param name="szMpqName">Archive file name</param>
        /// <param name="dwPriority">Archive Priority</param>
        /// <param name="dwFlags">Open flags</param>
        /// <param name="phMpq">Pointer to the result handle</param>
        /// <returns>On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern bool SFileOpenArchive([MarshalAs(UnmanagedType.LPTStr)] string szMpqName, uint dwPriority, uint dwFlags, out IntPtr phMpq);

        /// <summary>
        /// Compacts (rebuilds) an MPQ archive
        /// </summary>
        /// <param name="hMpq">Handle to the MPQ archive</param>
        /// <param name="szListFile">An additional listfile (optional and not used)</param>
        /// <param name="bReserved">Reserved, not to be used per API</param>
        /// <returns>On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern bool SFileCompactArchive(IntPtr hMpq, [MarshalAs(UnmanagedType.LPTStr)] string szListFile = null, bool bReserved = false);

        /// <summary>
        /// Closes an MPQ archive. Any unsaved MPQ tables are saved to the archive.
        /// </summary>
        /// <param name="hMpq">Handle to the MPQ archive</param>
        /// <returns>On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern bool SFileCloseArchive(IntPtr hMpq);


        /// <summary>
        /// Adds a file to the MPQ Archive.
        /// </summary>
        /// <param name="hMpq">Handle to the MPQ archive</param>
        /// <param name="szFileName">The name of a file to be added</param>
        /// <param name="szArchivedName">The name under which the file will be stored</param>
        /// <param name="dwFlags">Specifies archive flags for the file</param>
        /// <param name="dwCompression">Compression for the first block of the file</param>
        /// <param name="dwCompressionNext">Compression for rest of the file (except the first block)</param>
        /// <returns>On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern bool SFileAddFileEx(IntPtr hMpq, [MarshalAs(UnmanagedType.LPTStr)] string szFileName,
            [MarshalAs(UnmanagedType.LPStr)] string szArchivedName,
            uint dwFlags, uint dwCompression, uint dwCompressionNext);

        /// <summary>
        /// Exracts a file from the MPQ archive.
        /// </summary>
        /// <param name="hMpq">Handle to the MPQ archive</param>
        /// <param name="szToExtract">Name of the file to extract</param>
        /// <param name="szExtracted">Name of local file</param>
        /// <param name="dwSearchScope">Search scope</param>
        /// <returns>On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern bool SFileExtractFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szToExtract, [MarshalAs(UnmanagedType.LPTStr)] string szExtracted, uint dwSearchScope);


        // Ended up not needing the below after testing with them. Leaving here just in case.

        //[DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        //public static extern bool SFileRemoveFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szFileName, uint dwSearchScope);


        //[DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        //public static extern bool SFileVerifyFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szFileName, uint dwFlags);

        //[DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        //public static extern bool SFileFlushArchive(IntPtr hMpq);

        //[DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        //public static extern bool SFileCloseFile(IntPtr hFile);


        //[DllImport(STORMLIB_DLL, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        //public static extern bool SFileCreateArchive([MarshalAs(UnmanagedType.LPTStr)] string szMpqName, uint dwCreateFlags, uint dwMaxFileCount, out IntPtr phMpq);
    }
}
