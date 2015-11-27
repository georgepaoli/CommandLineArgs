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
            // TODO: Syntax if fucked up. Fix it ASAP!
            return ConsoleApp.StartApp(typeof(Program).GetTypeInfo().Assembly, args);
        }
    }
}
