using System;
using System.IO;

namespace StoreBusinessLayer.Utilities
{
    public static class ImageStorageHelper
    {
        private static readonly string WebRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public static string EnsureFolder(string folderName)
        {
            var target = Path.Combine(WebRoot, folderName);
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            return target;
        }

        public static bool TryDelete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return true;
            }

            var trimmed = relativePath.TrimStart('/', '\\');
            var normalized = trimmed.Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(WebRoot, normalized);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return true;
        }

        public static string GetRelativePath(string folderName, string fileName)
        {
            return $"/{folderName.Trim('/')}/{fileName}";
        }
    }
}


