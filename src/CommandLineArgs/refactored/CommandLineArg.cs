using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class CommandLineArg
    {
        public int Id;
        public string OriginalArgValue;
        public string Name;
        public string Value;
        public bool IsUsed = false;

        public static CommandLineArg FromArg(int id, string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentNullException("arg");
            }

            if (arg[0] != '/' && arg[0] != '-')
            {
                return new CommandLineArg()
                {
                    Id = id,
                    OriginalArgValue = arg,
                    Name = null,
                    Value = arg
                };
            };

            int p = arg.IndexOfAny(new char[] { ':', '=' });
            if (p == -1)
            {
                return new CommandLineArg()
                {
                    Id = id,
                    OriginalArgValue = arg,
                    Name = arg.Substring(1),
                    Value = null
                };
            }
            else
            {
                int startPos = 1;
                if (arg.Length >= 2 && arg[1] == '-')
                {
                    startPos++;
                }

                return new CommandLineArg()
                {
                    Id = id,
                    OriginalArgValue = arg,
                    Name = arg.Substring(startPos, p - 1),
                    Value = arg.Substring(p + 1)
                };
            }
        }
    }
}
