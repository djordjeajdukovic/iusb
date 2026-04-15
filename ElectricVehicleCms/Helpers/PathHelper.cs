
using System;
using System.IO;
using System.Linq;

namespace ElectricVehicleCms.Helpers
{
    public static class PathHelper
    {
        public static void EnsureDataFolders()
        {
            Directory.CreateDirectory(GetAbsolutePath("Data"));
            Directory.CreateDirectory(GetAbsolutePath(Path.Combine("Data", "Images")));
            Directory.CreateDirectory(GetAbsolutePath(Path.Combine("Data", "Rtf")));
        }

        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return string.Empty;
            }

            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        }

        public static string CopyImageToApplicationFolder(string sourceFilePath, string vehicleName)
        {
            EnsureDataFolders();

            string extension = Path.GetExtension(sourceFilePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png";
            }

            string fileName = MakeSafeFileName(vehicleName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
            string relativePath = Path.Combine("Data", "Images", fileName);
            string absolutePath = GetAbsolutePath(relativePath);

            File.Copy(sourceFilePath, absolutePath, true);

            return relativePath;
        }

        public static string BuildRtfRelativePath(string vehicleName)
        {
            EnsureDataFolders();

            string fileName = MakeSafeFileName(vehicleName) + ".rtf";
            return Path.Combine("Data", "Rtf", fileName);
        }

        public static string MakeSafeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "vehicle";
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            string cleaned = new string(value
                .ToLowerInvariant()
                .Select(c => invalidCharacters.Contains(c) ? '_' : c)
                .ToArray());

            cleaned = cleaned.Replace(' ', '_');

            while (cleaned.Contains("__"))
            {
                cleaned = cleaned.Replace("__", "_");
            }

            cleaned = cleaned.Trim('_');

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                cleaned = "vehicle";
            }

            return cleaned;
        }
    }
}
