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
        public CommandLineArgs Args;
        public ConsoleAppParams Params;
        public StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        public Dictionary<string, ParameterInformation> NameToParam;
        public List<List<int>> ParamToItsPositionsInArgs = new List<List<int>>();

        public ArgToParamBinder(CommandLineArgs args, ConsoleAppParams @params)
        {
            Args = args;
            Params = @params;

            NameToParam = new Dictionary<string, ParameterInformation>(Comparer);
            foreach (var parameterInformation in Params)
            {
                foreach (string name in parameterInformation.Names)
                {
                    NameToParam.Add(name, parameterInformation);
                }

                ParamToItsPositionsInArgs.Add(new List<int>());
            }
        }

        public void ArgsNameValueAndParamsPredicates()
        {
            foreach (var arg in Args)
            {
                if (arg.Name == null)
                {
                    continue;
                }

                ParameterInformation predicate;
                if (NameToParam.TryGetValue(arg.Name, out predicate))
                {
                    if (predicate.PopsRemainingArgs)
                    {
                        if (predicate.Field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                        {
                        }
                    }
                }
            }
        }

        public bool TryBindArgWithParam
    }
}
