using System;
using System.IO;
using System.Security.Cryptography;

namespace WC3CheatDetector
{
    /// <summary>
    /// Various file system related utility methods.
    /// </summary>
    public static class FileUtil
    {
        private static string _outputDir = "output";
        private static string _tempDir = _outputDir + "\\temp";
        private static string _inputDir = "input";
        
        /// <summary>
        /// Sets the input and output directories.
        /// </summary>
        /// <param name="inputDirectoryPath">Path to input directory.</param>
        /// <param name="outputDirectoryPath">Path to output directory.</param>
        public static void SetDirectories(string inputDirectoryPath, string outputDirectoryPath)
        {
            _inputDir = inputDirectoryPath;
            _outputDir = outputDirectoryPath;
            _tempDir = _outputDir + "\\temp";
        }

        /// <summary>
        /// Calculates the MD5 file hash of a file.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>The MD5 hash of the file or an empty string if there was an error.</returns>
        public static string CalculateFileMD5Hash(string filePath)
        {
            try
            {
                using MD5 md5 = MD5.Create();
                using FileStream fs = File.OpenRead(filePath);
                byte[] hash = md5.ComputeHash(fs);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch { return ""; } // Non-critical information. Return default rather than failing.
        }

        /// <summary>
        /// Calculates the file size in KB.
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The file size in KB or 0 if there was an error.</returns>
        public static int CalculateFileSizeKB(FileInfo file)
        {
            try
            {
                return (int)Math.Ceiling(file.Length / 1024D);
            }
            catch { return 0; } // Non-critical information. Return default rather than failing.
        }

        /// <summary>
        /// Creates the input, output, and temp directories if needed.
        /// </summary>
        public static void CreateDirectories()
        {

            // These are unlikely to have issues but we will suppress errors and try to continue on even if they do.
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                }
            } catch { }

            try
            {
                Directory.CreateDirectory(_tempDir);
            }
            catch { }

            try
            {
                if (!Directory.Exists(_inputDir))
                {
                    Directory.CreateDirectory(_inputDir);
                }
            }
            catch { }

            try
            {
                if (!Directory.Exists(_outputDir))
                {
                    Directory.CreateDirectory(_outputDir);
                }
            }
            catch { }
        }

        /// <summary>
        /// Retrieves the maps from the input directory.
        /// </summary>
        /// <param name="filter">A file filter to use. Only returns maps that match the filter.</param>
        /// <param name="checkSubfolders">Whether to check subfolders and directories or just the top level</param>
        /// <returns>The files found.</returns>
        public static FileInfo[] GetInputMaps(string filter = "*", bool checkSubfolders = true)
        {
            string totalFilter = filter + "*.w3x";
            SearchOption opt = checkSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            DirectoryInfo di = new DirectoryInfo(_inputDir);
            return di.GetFiles(totalFilter, opt);
        }

        /// <summary>
        /// Cleans up any temporary files.
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                }
            }
            catch { } // Unlikely to fail and not critical if it does
        }

        /// <summary>
        /// Creates a full path to the temporary directory for a given file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The full path</returns>
        public static string CreateTempFilePath(string fileName)
        {
            return _tempDir + "\\" + fileName;
        }
        /// <summary>
        /// Creates a full path to the output directory for a given file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The full path</returns>

        public static string CreateOutputFilePath(string fileName)
        {
            return _outputDir + "\\" + fileName;
        }

        /// <summary>
        /// Copies a file to the temporary directory
        /// </summary>
        /// <param name="filePath">The filepath to copy</param>
        /// <returns>The filepath to the new temporary file.</returns>
        public static string CopyToTemp(string filePath)
        {
            string newTempFilePath = CreateTempFilePath(Path.GetFileName(filePath));
            if (File.Exists(newTempFilePath))
            {
                Logger.Warn($"Temporary file {newTempFilePath} already exists. Potential input file name duplicates. Overwriting.");
            }

            File.Copy(filePath, newTempFilePath, true);
            return newTempFilePath;
        }

        /// <summary>
        /// Copies a file to the output directory
        /// </summary>
        /// <param name="filePath">The filepath to copy</param>
        /// <returns>The filepath to the new temporary file.</returns>
        public static string CopyToOutput(string filePath)
        {
            string newOutputFilePath = CreateOutputFilePath(Path.GetFileName(filePath));
            File.Copy(filePath, newOutputFilePath, true);
            return newOutputFilePath;
        }

        /// <summary>
        /// Creates a file in the output directory.
        /// </summary>
        /// <param name="fileName">The name of the file (including extension).</param>
        /// <param name="text">The text to write.</param>
        public static void CreateOutputFile(string fileName, string text)
        {
            string filePath = CreateOutputFilePath(fileName);
            File.WriteAllText(filePath, text);
        }

    }
}
