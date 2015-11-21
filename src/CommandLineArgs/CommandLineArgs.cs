using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    internal class CommandLineArgs
    {
        public string[] Args;
        public CommandLineArg[] CmdLineArgs;
        public bool[] UsedArgs;
        int _positionFromLeft = 0;

        public CommandLineArgs(string[] args)
        {
            Args = args;
            UsedArgs = new bool[args.Length];

            CmdLineArgs = new CommandLineArg[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                CmdLineArgs[i] = CommandLineArg.FromArg(args[i]);
            }
        }

        public void UseArg(int position)
        {
            if (UsedArgs[position])
            {
                throw new ArgumentException(string.Format("Multiple fields trying to use arg at position {0}", position));
            }

            UsedArgs[position] = true;
        }

        public string PeekPopArg()
        {
            for (; _positionFromLeft < UsedArgs.Length; _positionFromLeft++)
            {
                if (!UsedArgs[_positionFromLeft])
                {
                    if (CmdLineArgs[_positionFromLeft].Value != null)
                    {
                        return CmdLineArgs[_positionFromLeft].Value;
                    }
                }
            }

            return null;
        }

        public List<string> PopRemainingArgs()
        {
            List<string> ret = new List<string>();
            string s;
            while ((s = PeekPopArg()) != null)
            {
                PopArg();
                ret.Add(s);
            }

            return ret;
        }

        public void PopArg()
        {
            UseArg(_positionFromLeft);
        }

        public bool TryUseArg(int position)
        {
            if (UsedArgs[position])
            {
                return false;
            }

            UsedArgs[position] = true;
            return true;
        }
    }
}
