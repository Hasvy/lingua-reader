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

        public string? Convert(string ebupFile)
        {
            string dir = "/tmp";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            ZipFile.ExtractToDirectory(ebupFile, dir);

            if (File.Exists(dir + "/index.html"))
            {
                return dir + "/index.html";
            }
            return null;
        }
    }
}
