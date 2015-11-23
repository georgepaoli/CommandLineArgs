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
        
        public void AddParameter(ParameterInformation parameterInformation)
        {
            Params.Add(parameterInformation);
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
