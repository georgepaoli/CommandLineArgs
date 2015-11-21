using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    internal class CommandLineArgs
    {
        public CommandLineArg[] CmdLineArgs;
        bool[] _usedArgs;
        int _positionFromLeft = 0;

        public CommandLineArgs(string[] args)
        {
            _usedArgs = new bool[args.Length];

            CmdLineArgs = new CommandLineArg[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                CmdLineArgs[i] = CommandLineArg.FromArg(args[i]);
            }
        }

        public void UseArg(int position)
        {
            if (_usedArgs[position])
            {
                throw new ArgumentException(string.Format("Multiple fields trying to use arg at position {0}", position));
            }

            _usedArgs[position] = true;
        }

        public string PeekPopArg()
        {
            for (; _positionFromLeft < _usedArgs.Length; _positionFromLeft++)
            {
                if (!_usedArgs[_positionFromLeft])
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
            if (_usedArgs[position])
            {
                return false;
            }

            _usedArgs[position] = true;
            return true;
        }
    }
}
