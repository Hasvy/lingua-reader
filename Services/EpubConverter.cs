using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Services
{
    public class EpubConverter
    {
        public string ebupFile = null!;
        
        public EpubConverter() { }

        public void Convert(string epubFile)
        {
            string dir = "/tmp";
            Directory.Delete(dir, true);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string epubFilePath = epubFile;
            string outputZipFilePath = "/Archive.zip";

            using (FileStream zipStream = new FileStream(outputZipFilePath, FileMode.Create))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    // Добавляем файлы из EPUB архива в ZIP архив
                    using (ZipArchive epubArchive = ZipFile.Open(epubFilePath, ZipArchiveMode.Read))
                    {
                        foreach (var entry in epubArchive.Entries)
                        {
                            // Создаем новую запись в ZIP архиве и копируем содержимое из EPUB архива
                            var zipEntry = zipArchive.CreateEntry(entry.FullName);
                            using (var epubStream = entry.Open())
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                epubStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
            }

            ZipFile.ExtractToDirectory(outputZipFilePath, dir, true);

            //using (ZipArchive epubArchive = ZipFile.OpenRead(ebupFile))
            //{
            //    foreach (var entry in epubArchive.Entries)
            //    {
            //        string entryPath = Path.Combine(dir, entry.FullName);
            //        File.Create(entryPath);
            //        entry.ExtractToFile(entryPath, true);
            //    }
            //}

            //ZipFile.ExtractToDirectory(dir, ebupFile);

            //if (File.Exists(dir + "/index.html"))
            //{
            //    return dir + "/index.html";
            //}
            //return null;
        }
    }
}
