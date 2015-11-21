using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleApp
    {
        internal ParamInfo[] Params;
        private Dictionary<string, ParamInfo> _nameOrAliasToName = new Dictionary<string, ParamInfo>(Constants.Comparer);

        private ConsoleApp(TypeInfo type)
        {
            FieldInfo[] fields = type.AsType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            Params = new ParamInfo[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                ParamInfo paramInfo = new ParamInfo(i, field);
                Params[i] = paramInfo;

                _nameOrAliasToName.Add(field.Name, paramInfo);

                foreach (var customAttribute in field.GetCustomAttributes())
                {
                    var asAlias = customAttribute as AliasAttribute;
                    if (asAlias != null)
                    {
                        if (_nameOrAliasToName.ContainsKey(asAlias.Name))
                        {
                            throw new ArgumentException($"More than one value defines the same name {asAlias.Name}");
                        }

                        _nameOrAliasToName.Add(asAlias.Name, paramInfo);
                    }

                    if (customAttribute as RequiredAttribute != null)
                    {
                        paramInfo.IsRequired = true;
                    }

                    if (customAttribute as PopArgAttribute != null)
                    {
                        paramInfo.CanPopArg = true;
                    }

                    if (customAttribute as PopRemainingArgs != null)
                    {
                        // TODO: other types
                        if (paramInfo.Field.FieldType == typeof(List<string>));
                        {
                            paramInfo.IsPoppingRemainingArgs = true;
                        }
                    }
                }
            }
        }

        private void BindCommandLineArgs(CommandLineArgs commandLineArgs)
        {
            var ret = new Dictionary<string, List<int>>(Constants.Comparer);
            for (int i = 0; i < commandLineArgs.CmdLineArgs.Length; i++)
            {
                ParamInfo paramInfo;
                if (commandLineArgs.CmdLineArgs[i].Name != null && _nameOrAliasToName.TryGetValue(commandLineArgs.CmdLineArgs[i].Name, out paramInfo))
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
