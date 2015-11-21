using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    public class Tests
    {
        public static int Main(string[] args)
        {
            ConsoleApp.StartApp<SimpleTests>(args);
            ConsoleApp.StartApp<FutureFeatures>(args);
            return 0;
        }
    }
}
