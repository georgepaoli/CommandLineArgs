using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFun
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
            return FromCommandLineArgs<T>(new CommandLineArgs(args));
        }

        internal static T FromCommandLineArgs<T>(CommandLineArgs commandLineArgs)
        {
            ConsoleApp app = new ConsoleApp(typeof(T));
            app.BindCommandLineArgs(commandLineArgs);

            T ret = Activator.CreateInstance<T>();

            bool[] paramsWithSetValues = new bool[app.Params.Length];

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (app.Params[i].TrySetSingleValueFromNamedArg(ref ret, commandLineArgs))
                {
                    paramsWithSetValues[i] = true;
                }
            }

            for (int i = 0; i < app.Params.Length; i++)
            {
                if (!paramsWithSetValues[i] && !app.Params[i].TrySetSingleValueFromConsecutiveArg(ref ret, commandLineArgs))
                {
                    app.Params[i].ThrowIfRequiredArg();
                }
            }

            return ret;
        }
    }
}
