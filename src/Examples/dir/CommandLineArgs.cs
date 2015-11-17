using ConsoleFun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        for (;  _positionFromLeft < _usedArgs.Length; _positionFromLeft++)
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
