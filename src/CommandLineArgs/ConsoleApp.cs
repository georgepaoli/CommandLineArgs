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
                            throw new ArgumentException("More than one value defines the same name {0}", asAlias.Name);
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
            T ret = Activator.CreateInstance<T>();
            FromCommandLineArgs<T>(ref ret, new CommandLineArgs(args));
            return ret;
        }

        internal static void FromCommandLineArgs<T>(ref T obj, CommandLineArgs commandLineArgs)
        {
            ConsoleApp app = new ConsoleApp(typeof(T));
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

        public static void StartApp<T>(string[] args)
        {
            T ret = Activator.CreateInstance<T>();

            if (args.Length >= 1)
            {
                FromCommandLineArgs<T>(ref ret, new CommandLineArgs(args.Slice(1)));
                if ((new Function() { Object = ret, Name = args[0] }).Invoke())
                {
                    return;
                }
            }

            Console.WriteLine("List of available commands:");
            foreach (var method in typeof(T).GetTypeInfo().DeclaredMethods)
            {
                if (method.GetParameters().Length == 0)
                {
                    Console.WriteLine("    {0}", method.Name);
                }
            }
        }
    }
}
