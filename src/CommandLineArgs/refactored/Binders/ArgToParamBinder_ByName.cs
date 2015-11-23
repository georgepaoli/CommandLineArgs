using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.refactored.Binders
{
    public struct ArgNameValue
    {
        public string Name;
        public string Value;
    }

    public class ArgToParamBinder_ByName : IBinder<CommandLineArgs, ConsoleAppParams>
    {
        public Dictionary<string, List<ParameterInformation>> NameToParam = new Dictionary<string, List<ParameterInformation>>(Constants.Comparer);
        public List<ArgNameValue> ArgsNameValue = new List<ArgNameValue>();

        public void Bind(CommandLineArgs args, ConsoleAppParams parameters)
        {
            PreprocessParams(parameters);
            PreprocessArgs(args);
            throw new NotImplementedException();
        }

        public void PreprocessParams(ConsoleAppParams parameters)
        {
            foreach (var parameterInformation in parameters.Params)
            {
                foreach (string name in parameterInformation.Names)
                {
                    NameToParam.AddValueToList(name, parameterInformation);
                }
            }
        }

        public void PreprocessArgs(CommandLineArgs args)
        {
            foreach (var argument in args.Args)
            {
                var arg = argument.OriginalValue;

                if (string.IsNullOrEmpty(arg))
                {
                    throw new ArgumentNullException("arg");
                }

                if (arg[0] != '/' && arg[0] != '-')
                {
                    ArgsNameValue.Add(new ArgNameValue()
                    {
                        Name = null,
                        Value = arg
                    });

                    continue;
                }

                int p = arg.IndexOfAny(new char[] { ':', '=' });
                if (p == -1)
                {
                    ArgsNameValue.Add(new ArgNameValue()
                    {
                        Name = arg.Substring(1),
                        Value = null
                    });
                }
                else
                {
                    int startPos = 1;
                    if (arg.Length >= 2 && arg[1] == '-')
                    {
                        startPos++;
                    }

                    ArgsNameValue.Add(new ArgNameValue()
                    {
                        Name = arg.Substring(startPos, p - 1),
                        Value = arg.Substring(p + 1)
                    });
                }
            }
        }
    }
}
