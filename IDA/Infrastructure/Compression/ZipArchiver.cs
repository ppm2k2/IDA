using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ConceptONE.Infrastructure.Compression
{
    public class ZipArchiver
    {

        public MemoryStream GetArchiveStream(Dictionary<string, string> fileContents)
        {

            using (MemoryStream result = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(result, ZipArchiveMode.Create, true))
                {
                    foreach (string fileName in fileContents.Keys)
                    {
                        ZipArchiveEntry entryArchive = zipArchive.CreateEntry(fileName);

                        using (Stream entryStream = entryArchive.Open())
                        using (StreamWriter entryWriter = new StreamWriter(entryStream))
                        {
                            entryWriter.Write(fileContents[fileName]);
                        }
                    }
                }

                return result;
            }

        }

        public List<string> ExtractFilesFromArchive(string archivePath, string extractFolder)
        {
            List<string> result = new List<string>();
            Directory.CreateDirectory(extractFolder);

            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(extractFolder, entry.Name);
                    if (!File.Exists(fullPath))
                        entry.ExtractToFile(Path.Combine(extractFolder, entry.Name));

                    result.Add(fullPath);
                }
            }

            return result;
        }

        public List<string> GetFileNamesFromArchive(string archivePath, string extension)
        {
            List<string> result = new List<string>();

            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (extension == "*" || entry.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
                        result.Add(entry.Name);
                }
            }

            return result;
        }

        public bool CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
        {
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName);
            bool result = File.Exists(destinationArchiveFileName);

            return result;
        }


    }
}
