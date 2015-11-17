using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    internal class CommandLineArg
    {
        public string Name;
        public string Value;

        private CommandLineArg() { }

        private static CommandLineArg FromName(string name)
        {
            return new CommandLineArg()
            {
                Name = name,
                Value = null
            };
        }

        private static CommandLineArg FromValue(string value)
        {
            return new CommandLineArg()
            {
                Name = null,
                Value = value
            };
        }

        private static CommandLineArg FromNameValue(string name, string value)
        {
            return new CommandLineArg()
            {
                Name = name,
                Value = value
            };
        }

        public static CommandLineArg FromArg(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentNullException("arg");
            }

            if (arg[0] != '/')
            {
                return FromValue(arg);
            };

            int p = arg.IndexOfAny(new char[] { ':', '=' });
            if (p == -1)
            {
                return FromName(arg.Substring(1));
            }
            else
            {
                return FromNameValue(
                    name: arg.Substring(1, p - 1),
                    value: arg.Substring(p + 1));
            }
        }
    }
}
