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

        //public IEnumerable<CommandLineArg> PopRemainingArgs()
        //{
        //    foreach (var arg in this)
        //    {
        //        if (!arg.IsUsed)
        //        {
        //            arg.IsUsed = true;
        //            yield return arg;
        //        }
        //    }
        //}

        //public CommandLineArg GetNextUnused(CommandLineArg arg, bool includeSelf = false)
        //{
        //    int i = arg.Position;

        //    if (!includeSelf)
        //    {
        //        i++;
        //    }

        //    for (; i < Count; i++)
        //    {
        //        if (!this[i].IsUsed)
        //        {
        //            return this[i];
        //        }
        //    }

        //    return null;
        //}
    }
}
