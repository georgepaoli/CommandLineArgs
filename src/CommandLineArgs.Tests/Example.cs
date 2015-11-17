using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    public class Example
    {
        public void Start()
        {
        }

        [DefaultFunction]
        public void RunAllTests()
        {
        }

        public static int Main(string[] args)
        {
            ConsoleApp.StartApp<Example>(args);
            return 0;
        }
    }
}
