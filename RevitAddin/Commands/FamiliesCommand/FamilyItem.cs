using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR
{
    public class FamilyItem
    {
        public string Name { get; set; }     
        public string FilePath { get; set; }   
        public string ThumbnailPath { get; set; } 

        public FamilyItem(string filePath, string thumbsDirectory)
        {
            FilePath = filePath;
            Name = Path.GetFileNameWithoutExtension(filePath);

            string thumbPath = Path.Combine(thumbsDirectory, Name + ".png");
            ThumbnailPath = File.Exists(thumbPath) ? thumbPath : Path.Combine(thumbsDirectory, "projeta.png");
        }
    }
}
