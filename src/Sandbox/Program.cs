using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLineArgs;
using System.Reflection;

namespace Sandbox
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return ConsoleApp.StartApp<Program>(args);
        }
    }
}
