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
        public Dictionary<string, ParameterInformation> NameToParam = new Dictionary<string, ParameterInformation>();
        public Queue<ParameterInformation> ArgPoppers = new Queue<ParameterInformation>();
        public List<string> UnusedArgs = new List<string>();

        public ConsoleAppParams(object target)
        {
            Object = target;
            foreach (var field in Object.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddParameter(new ParameterInformation(this, field));
            }

            foreach (var param in this)
            {
                foreach (var name in param.Names)
                {
                    bool success = NameToParam.TryAdd(name, param);
                    if (!success)
                    {
                        Console.Error.WriteLine($"Duplicate name `{name}`.");
                        Console.Error.WriteLine($"First orrucance has type `{NameToParam[name].Field.FieldType.FullName}`.");
                        Console.Error.WriteLine($"Another occurance has type `{param.Field.FieldType.FullName}`.");
                    }
                }

                if (param.ArgsToPop > 0 || param.PopsRemainingArgs)
                {
                    ArgPoppers.Enqueue(param);
                }
            }
        }

        public void AddParameter(ParameterInformation parameterInformation)
        {
            Add(parameterInformation);
        }

        public void Bind()
        {
            bool ignoreNames = false;
            for (int i = 0; i < Args.Count; i++)
            {
                CommandLineArg arg = Args[i];
                if (!ignoreNames)
                {
                    if (arg.OriginalValue == "--")
                    {
                        ignoreNames = true;
                        continue;
                    }

                    ParameterInformation param;
                    if (NameToParam.TryGetValue(arg.Name, out param))
                    {
                        if (param.TryBindValue(arg.Value))
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }
}
