using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Zip
{
    public class Zip
    {
        [Required]
        [PopArg]
        public string ZipPath;

        [Required]
        [PopArg]
        public string Directory;

        public void Pack()
        {
            ZipFile.CreateFromDirectory(Directory, ZipPath);
        }

        public void Extract()
        {
            ZipFile.ExtractToDirectory(ZipPath, Directory);
        }
    }
}
