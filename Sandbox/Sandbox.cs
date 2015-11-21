using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sandbox
{
    [DefaultCommand(".*")]
    public class Sandbox
    {
        public void Start()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
