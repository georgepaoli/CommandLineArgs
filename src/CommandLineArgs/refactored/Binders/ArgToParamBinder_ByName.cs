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

                ParamsPredicatesPositionsInArgs.Add(new List<int>());
            }
        }

        public List<ArgNameValue> ArgsNameValue = new List<ArgNameValue>();
        public List<List<int>> ParamsPredicatesPositionsInArgs = new List<List<int>>();
        public void ArgsNameValueAndParamsPredicates()
        {
            // TODO: feels like this should have nested functions, unfortunatelly they are not supported...
            for (int i = 0; i < Args.Count; i++)
            {
                var arg = Args[i].OriginalValue;
                // void "process arg"(var arg = Args[i].OriginalValue)

                if (string.IsNullOrEmpty(arg))
                {
                    throw new ArgumentNullException("arg");
                }

                // Would be nice if this code was auto-generated from:
                // "Is this Arg or Value? Arg starts with '/' or '-'. If this is Arg then we are finished here."();
                // +--------------------+---------------------------+
                // | this is interface  | Arg is a string.          |
                // | for Arg and Value  |---------------------------|
                // |--------------------| (Arg|string)."starts with"Y--> 
                // | We want to deter-  | ({ '/', '-'})?            |
                // | mine instance type ->------------------------>-|
                // |--------------------|
                if (arg[0] != '/' && arg[0] != '-')
                {
                    ArgsNameValue.Add(new ArgNameValue()
                    {
                        Name = null,
                        Value = arg
                    });
                    continue;
                }

                ArgNameValue argInfo; // GetArgInfo()
                {
                    int p = arg.IndexOfAny(new char[] { ':', '=' });
                    if (p == -1)
                    {
                        argInfo = new ArgNameValue()
                        {
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

                        argInfo = new ArgNameValue()
                        {
                            Name = arg.Substring(startPos, p - 1),
                            Value = arg.Substring(p + 1)
                        };
                    }
                }

                // do i need to have list of arg name values?
                ArgsNameValue.Add(argInfo);

                // void "TODO: no name"()
                List<ParameterInformation> listParametersBoundWith;
                if (NameToParam.TryGetValue(
                ParamsPredicatesPositionsInArgs
            }
        }

        public void Bind()
        {
            // note: order of call is exactly the same as doc order
            // note: it can potentially run it with one command in the future
            NameToParamGetValue();
            ArgsNameValueAndParamsPredicates();
        }
    }
}
