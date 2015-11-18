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

        private ConsoleApp(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
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
            return (T)FromCommandLineArgs(typeof(T), args);
        }

        public static object FromCommandLineArgs(Type t, string[] args)
        {
            object ret = Activator.CreateInstance(t);
            FromCommandLineArgs(t, ref ret, new CommandLineArgs(args));
            return ret;
        }

        internal static void FromCommandLineArgs(Type t, ref object obj, CommandLineArgs commandLineArgs)
        {
            ConsoleApp app = new ConsoleApp(t);
            app.BindCommandLineArgs(commandLineArgs);

            bool[] paramsWithSetValues = new bool[app.Params.Length];

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (app.Params[i].TrySetSingleValueFromNamedArg(ref obj, commandLineArgs))
                {
                    paramsWithSetValues[i] = true;
                }
            }

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (!paramsWithSetValues[i] && !app.Params[i].TrySetSingleValueFromConsecutiveArg(ref obj, commandLineArgs))
                {
                    app.Params[i].ThrowIfRequiredArg();
                }
            }
        }

        private class RunCommandsResult
        {
            public int NumberOfRunCommands = 0;
            public int NumberOfFailedCommands = 0;
        }

        private static RunCommandsResult RunCommands(IEnumerable<MethodInfo> functions, object obj)
        {
            RunCommandsResult result = new RunCommandsResult();
            foreach (var function in functions)
            {
                result.NumberOfRunCommands++;
                Console.WriteLine($"--- Running {function.Name} ---");
                try
                {
                    function.Invoke(obj);
                }
                catch (Exception e)
                {
                    result.NumberOfFailedCommands++;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Error.WriteLine($"!!! Finished running {function.Name} with exception !!!");
                    Console.Error.WriteLine(e);
                    Console.ResetColor();
                }
            }

            return result;
        }

        private static bool PrintReport(RunCommandsResult result)
        {
            switch (result.NumberOfRunCommands)
            {
                case 0: return false;
                case 1: return true;
                default:
                {
                    Console.WriteLine($"---=== Finished running {result.NumberOfRunCommands} commands ===---");
                    if (result.NumberOfFailedCommands > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"---=== {result.NumberOfFailedCommands} of them failed ===---");
                        Console.ResetColor();
                    }
                    return true;
                }
            }

        }

        public static void StartApp<T>(string[] args)
        {
            StartApp(typeof(T), args);
        }

        public static void StartApp(Type t, string[] args)
        {
            object ret = Activator.CreateInstance(t);

            var functions = MethodExtensions.GetListFromType(t);

            if (args.Length >= 1)
            {
                FromCommandLineArgs(t, ref ret, new CommandLineArgs(args.Slice(1)));
                if (!PrintReport(RunCommands(functions.MatchName(args[0]), ret)))
                {
                    return;
                }
            }

            bool commandRun = false;
            foreach (var attribute in t.GetTypeInfo().GetCustomAttributes())
            {
                var defaultCommand = attribute as DefaultCommandAttribute;
                if (defaultCommand != null)
                {
                    FromCommandLineArgs(t, ref ret, new CommandLineArgs(args));
                    if (PrintReport(RunCommands(functions.MatchName(defaultCommand.Command), ret)))
                    {
                        commandRun = true;
                        return;
                    }
                }
            }

            if (commandRun)
            {
                return;
            }

            bool headerPrinted = false;
            foreach (var method in functions)
            {
                if (!headerPrinted)
                {
                    Console.WriteLine("List of available commands:");
                    headerPrinted = true;
                }

                Console.WriteLine($"    {method.Name}");
            }

            headerPrinted = false;
            foreach (var field in t.GetTypeInfo().DeclaredFields)
            {
                if (!headerPrinted)
                {
                    Console.WriteLine("List of available parameters:");
                    headerPrinted = true;
                }

                Console.Write("    ");

                if (field.GetCustomAttribute(typeof(RequiredAttribute)) != null)
                {
                    Console.Write("[Required] ");
                }

                Console.Write($"--{field.Name}");
                foreach (var attribute in field.GetCustomAttributes())
                {
                    var alias = attribute as AliasAttribute;
                    if (alias != null)
                    {
                        Console.Write($"-{alias.Name}");
                    }
                }
            }
        }
    }
}
