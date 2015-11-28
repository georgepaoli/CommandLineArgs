using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // TODO: Syntax is not intuitive. Fix it ASAP!
            return ConsoleApp.StartApp<Program>(args);
        }
    }
}
