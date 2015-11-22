using CommandLineArgs;
using System;
using System.IO;

namespace Dir
{
    public class Dir
    {
        [Description("Search pattern")]
        [PopArg]
        [Alias("r")]
        public string SearchPattern = "*";

        [Description("Path to the folder of which to enumerate files and folders")]
        [PopArg]
        [Alias("p")]
        public string Path = ".";

        [Description("Disables colors")]
        public bool NoColor = false;

        public ConsoleColor? FolderTextColor = ConsoleColor.DarkYellow;
        public ConsoleColor? FileTextColor;
        public int TabWidth = 4;

        [Alias("s")]
        public bool SubDirectories = false;

        public int Start()
        {
            if (NoColor)
            {
                FolderTextColor = null;
                FileTextColor = null;
            }
            PrettyEnumerateDirectories();
            return 0;
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace('\\', System.IO.Path.DirectorySeparatorChar);
        }

        private void PrettyEnumerateDirectories(int numberOfTabs = 0)
        {
            Path = NormalizePath(Path);
            SearchOption searchOptions = SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            string indentation = new string(' ', TabWidth * numberOfTabs);
            try
            {
                if (FolderTextColor.HasValue)
                {
                    Console.ForegroundColor = FolderTextColor.Value;
                }

                foreach (var dir in Directory.EnumerateDirectories(Path, SearchPattern, searchOptions))
                {
                    Console.Write(indentation);
                    Console.WriteLine("[DIR] {0}", dir);
                }

                if (FolderTextColor.HasValue)
                {
                    Console.ResetColor();
                }

                if (FileTextColor.HasValue)
                {
                    Console.ForegroundColor = FileTextColor.Value;
                }

                foreach (var file in Directory.EnumerateFiles(Path, SearchPattern, searchOptions))
                {
                    Console.Write(indentation);
                    Console.WriteLine("{0}", file);
                }
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
