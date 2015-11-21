using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static int Main(string[] args)
    {
        ConsoleApp.FromCommandLineArgs<Example2>(args).Start();
        return 0;
    }
}
