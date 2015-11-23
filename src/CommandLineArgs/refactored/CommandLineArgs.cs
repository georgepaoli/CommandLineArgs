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
                Add(new CommandLineArg()
                {
                    Position = Count,
                    OriginalValue = arg,
                    IsUsed = false
                });
            }
        }

        public List<CommandLineArg> PopRemainingArgs()
        {
            List<CommandLineArg> ret = new List<CommandLineArg>();
            foreach (var arg in this)
            {
                if (!arg.IsUsed)
                {
                    arg.IsUsed = true;
                    ret.Add(arg);
                }
            }

            return ret;
        }

        public CommandLineArg GetNextUnused(CommandLineArg arg, bool includeSelf = false)
        {
            int i = arg.Position;

            if (!includeSelf)
            {
                i++;
            }

            for (; i < Count; i++)
            {
                if (!this[i].IsUsed)
                {
                    return this[i];
                }
            }

            return null;
        }
    }
}
