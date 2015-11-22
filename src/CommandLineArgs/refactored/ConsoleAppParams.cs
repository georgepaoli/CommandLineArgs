using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleAppParams
    {
        public List<ParameterInformation> Params = new List<ParameterInformation>();
        public Dictionary<string, List<ParameterInformation>> NameToParam = new Dictionary<string, List<ParameterInformation>>(Constants.Comparer);

        public void AddParameter(ParameterInformation parameterInformation)
        {
            Params.Add(parameterInformation);
            foreach (string name in parameterInformation.Names)
            {
                NameToParam.AddValueToList(name, parameterInformation);
            }
        }

        public void AddParametersFromType(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddParameter(ParameterInformation.FromField(field));
            }
        }
    }
}
