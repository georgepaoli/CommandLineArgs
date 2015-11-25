using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleAppParams : List<ParameterInformation>
    {
        public object Object;
        public CommandLineArgs Args = new CommandLineArgs();

        public StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        public Dictionary<string, ParameterInformation> NameToParam;

        public ConsoleAppParams(object target)
        {
            Object = target;
            foreach (var field in Object.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddParameter(new ParameterInformation(this, field));
            }
        }

        public void AddParameter(ParameterInformation parameterInformation)
        {
            Add(parameterInformation);
        }

        public void Bind()
        {
            throw new NotImplementedException();
        }
    }
}
