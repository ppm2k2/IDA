using System;
using System.IO;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class IOExtensions
    {
        public static string GetFilePathWithNewExtension(this string fullPath, string extension)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath) + "." + extension;
            string filePath = Path.GetDirectoryName(fullPath);
            string result = Path.Combine(filePath, fileName);

            return result;
        }

        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path);

                    foreach (string file in files)
                        File.Delete(file);

                    foreach (string subDir in Directory.GetDirectories(path))
                        DeleteDirectory(subDir);

                    Directory.Delete(path);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

    }
}
