using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dir
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return ConsoleApp.FromCommandLineArgs<Dir>(args).Start();
        }
    }
}
