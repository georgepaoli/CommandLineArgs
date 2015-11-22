using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleApp2
    {


        private void BindCommandLineArgs(CommandLineArgs commandLineArgs)
        {
            var ret = new Dictionary<string, List<int>>(Constants.Comparer);
            for (int i = 0; i < commandLineArgs.CmdLineArgs.Length; i++)
            {
                ParamInfo paramInfo;
                if (commandLineArgs.CmdLineArgs[i].Name != null && NameToParam.TryGetValue(commandLineArgs.CmdLineArgs[i].Name, out paramInfo))
                {
                    if (paramInfo.PositionsInCommandLineArgs == null)
                    {
                        paramInfo.PositionsInCommandLineArgs = new List<int>();
                    }

                    paramInfo.PositionsInCommandLineArgs.Add(i);
                }
            }
        }

        public static T FromCommandLineArgs<T>(string[] args)
        {
            return (T)FromCommandLineArgs(typeof(T).GetTypeInfo(), args);
        }

        public static object FromCommandLineArgs(TypeInfo t, string[] args)
        {
            object ret = Activator.CreateInstance(t.AsType());
            FromCommandLineArgs(t, ref ret, new CommandLineArgs(args));
            return ret;
        }

        internal static void FromCommandLineArgs(TypeInfo t, ref object obj, CommandLineArgs commandLineArgs)
        {
            ConsoleApp app = new ConsoleApp(t);
            app.BindCommandLineArgs(commandLineArgs);

            bool[] paramsWithSetValues = new bool[app.Params.Length];

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (app.Params[i].TryBindValueWithArgs(ref obj, commandLineArgs))
                {
                    paramsWithSetValues[i] = true;
                }
            }

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (app.Params[i].IsPoppingRemainingArgs)
                {
                    // TODO: Do not substitute list and add instead
                    List<string> remainingArgs = commandLineArgs.PopRemainingArgs();

                    // TODO: support different types
                    List<string> existing = app.Params[i].Field.GetValue(obj) as List<string>;
                    if (existing != null)
                    {
                        existing.AddRange(remainingArgs);
                    }
                    else
                    {
                        app.Params[i].Field.SetValue(obj, remainingArgs);
                    }
                    
                    continue;
                }
                if (!paramsWithSetValues[i] && !app.Params[i].TrySetSingleValueFromConsecutiveArg(ref obj, commandLineArgs))
                {
                    app.Params[i].ThrowIfRequiredArg();
                }
            }
        }

        public static void StartApp(string[] args)
        {
            foreach (var type in typeof(ConsoleApp).GetTypeInfo().Assembly.DefinedTypes)
            {
                if (type.GetCustomAttribute(typeof(DefaultCommandAttribute)) != null)
                {
                    StartApp(type, args);
                }
            }
        }

        public static void StartApp<T>(string[] args)
        {
            StartApp(typeof(T).GetTypeInfo(), args);
        }

        private static void PrintCommandDescription(FieldInfo field)
        {
            bool required = field.GetCustomAttribute(typeof(RequiredAttribute)) != null;

            Console.Write("    ");

            if (!required)
            {
                Console.Write("[");
            }

            List<string> names = new List<string>();
            names.Add(field.Name);

            foreach (var attribute in field.GetCustomAttributes())
            {
                var alias = attribute as AliasAttribute;
                if (alias != null)
                {
                    names.Add(alias.Name);
                }
            }

            if (names.Count == 1)
            {
                Console.Write($"/{names[0]}");
            }
            else
            {
                Console.Write("/(");
                bool first = true;
                foreach (var name in names)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        Console.Write("|");
                    }
                    Console.Write($"{name}");
                }
                Console.Write(")");
            }

            Console.Write($"=<{field.FieldType.Name}>");

            if (!required)
            {
                Console.Write("]");
            }

            Console.WriteLine();
        }
    }
}
