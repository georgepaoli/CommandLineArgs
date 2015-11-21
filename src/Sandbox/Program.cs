using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLineArgs;

namespace Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[]
                    {

                    };
            }

            ConsoleApp.StartApp<Sandbox>(args);
        }
    }
}
