using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static int Main(string[] args)
    {
        // TODO: Print class name when printing function names when more than one class
        // TODO: Help is broken for multiple commands...
        return ConsoleApp.StartApp<Program>(args);
    }
}
