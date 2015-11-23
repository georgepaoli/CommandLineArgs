using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public struct ArgNameValue
    {
        public string Name;
        public string Value;
    }

    public class ArgToParamBinder
    {
        [Required][PopArg]
        public CommandLineArgs Args;
        [Required][PopArg]
        public ConsoleAppParams Params;

        public StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public Dictionary<string, List<ParameterInformation>> NameToParam;
        public void NameToParamGetValue()
        {
            NameToParam = new Dictionary<string, List<ParameterInformation>>(Comparer);
            foreach (var parameterInformation in Params)
            {
                foreach (string name in parameterInformation.Names)
                {
                    NameToParam.AddValueToList(name, parameterInformation);
                }

                ParamsPositionsInArgs.Add(new List<int>());
            }
        }

        public List<ArgNameValue> ArgsNameValue = new List<ArgNameValue>();
        public void ArgsNameValueGetValue()
        {
            foreach (var argument in Args)
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

        public List<List<int>> ParamsPositionsInArgs = new List<List<int>>();
        public void ParamsPositionsInArgsGetValue()
        {
            throw new NotImplementedException();
        }

        public void Bind()
        {
            // note: order of call is exactly the same as doc order
            // note: it can potentially run it with one command in the future
            NameToParamGetValue();
            ArgsNameValueGetValue();
            ParamsPositionsInArgsGetValue();
        }
    }
}
