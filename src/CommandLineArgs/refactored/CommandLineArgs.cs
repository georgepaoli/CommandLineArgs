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
                Args.Add(CommandLineArg.FromArg(Args.Count, arg));
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

        public CommandLineArg GetNextUnused(CommandLineArg arg)
        {
            for (int i = arg.Id + 1; i < Args.Count; i++)
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
