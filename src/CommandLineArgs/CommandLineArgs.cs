using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class CommandLineArgs : List<CommandLineArg>
    {
        public void AddArgs(string[] args)
        {
            foreach (var arg in args)
            {
                Add(new CommandLineArg(Count, arg));
            }
        }
    }
}
