using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class CommandLineArgs
    {
        public List<CommandLineArg> Args = new List<CommandLineArg>();

        public void AddArgs(string[] args)
        {
            foreach (var arg in args)
            {
                Args.Add(new CommandLineArg()
                {
                    Position = Args.Count,
                    OriginalValue = arg,
                    IsUsed = false
                });
            }
        }

        public List<CommandLineArg> PopRemainingArgs()
        {
            List<CommandLineArg> ret = new List<CommandLineArg>();
            foreach (var arg in Args)
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

            for (; i < Args.Count; i++)
            {
                if (!Args[i].IsUsed)
                {
                    return Args[i];
                }
            }

            return null;
        }
    }
}
