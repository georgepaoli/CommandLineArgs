using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleAppParams : List<ParameterInformation>
    {
        public CommandLineArgs Args = new CommandLineArgs();

        public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        public Dictionary<string, ParameterInformation> NameToParam = new Dictionary<string, ParameterInformation>(Comparer);

        public List<string> UnusedArgs = new List<string>();

        public void AddTarget(object target)
        {
            foreach (var field in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddParameter(new ParameterInformation(this, target, field));
            }

            foreach (var param in this)
            {
                foreach (var name in param.Names)
                {
                    if (!NameToParam.TryAdd(name, param))
                    {
                        Console.Error.WriteLine($"Duplicate name `{name}`.");
                        Console.Error.WriteLine($"First orrucance has type `{NameToParam[name].Field.FieldType.FullName}`.");
                        Console.Error.WriteLine($"Another occurance has type `{param.Field.FieldType.FullName}`.");
                    }
                }
            }
        }

        // TODO: this looks bad
        public void AddArgs(string[] args)
        {
            if (args != null)
            {
                Args.AddArgs(args);
            }
        }

        public static bool TryFromCommandLineArgs<T>(string[] args, out ConsoleAppParams app)
        {
            app = new ConsoleAppParams();
            app.AddTarget(Activator.CreateInstance<T>());
            app.AddArgs(args);
            if (!app.Bind())
            {
                return false;
            }

            return true;
        }

        public void AddParameter(ParameterInformation parameterInformation)
        {
            Add(parameterInformation);
        }

        private static string GetPrintableString(string s)
        {
            return s == null ? "null" : $"`{s}`";
        }

        // TODO: should this method not return anything and just throw? right now it is mixed :P
        // TODO: split or leave as is (easy to understand vs easy to fix)
        public bool Bind()
        {
            // TODO: add case of combining bools as in: `git clean -fdx` (which is equivalent to `git clean -f -d -x`)
            bool ignoreNames = false;
            for (int i = 0; i < Args.Count; i++)
            {
                CommandLineArg arg = Args[i];
                if (!ignoreNames)
                {
                    // everything after this separator is treated as 
                    if (arg.OriginalValue == "--")
                    {
                        ignoreNames = true;
                        continue;
                    }

                    ParameterInformation param;
                    if (NameToParam.TryGetValue(arg.Name, out param))
                    {
                        if (param.StopProcessingNamedArgsAfterThis)
                        {
                            ignoreNames = true;
                        }

                        // TODO: should this take an arg itself?
                        if (param.TryBindValue(arg.Value))
                        {
                            continue;
                        }

                        if (arg.Operator != null)
                        {
                            Console.Error.WriteLine($"Warning: Unrecognized value: {GetPrintableString(arg.Value)} for {GetPrintableString(arg.Name)}.");
                            Console.Error.WriteLine($"Warning: Expected type: {GetPrintableString(param.Field.FieldType.FullName)}.");
                            Console.Error.WriteLine($"Warning: Note: Type might not be supported or there might be something wrong with this library");
                            Console.Error.WriteLine($"Warning:       File an issue if you think it is wrong");
                            continue;
                        }

                        // try use next arg as value
                        ++i;
                        if (i < Args.Count && param.TryBindValue(Args[i].OriginalValue))
                        {
                            continue;
                        }
                        --i;

                        // bool doesn't require value
                        if (param.TryBindValue("true"))
                        {
                            continue;
                        }

                        Console.Error.WriteLine($"Warning: No value found for `{arg.Name}`. Expected type: {param.Field.FieldType.FullName}.");
                        Console.Error.WriteLine($"Warning: Note: Type might not be supported or there might be something wrong with this library");
                        Console.Error.WriteLine($"Warning:       File an issue if you think it is wrong");
                        continue;
                    }
                } // if (!ignoreNames)

                // TODO: This looks horrible
                // TODO: optimize
                // Bind unnamed args
                foreach (var param in this)
                {
                    if (param.ArgsToPop == 0 && !param.PopsRemainingArgs)
                    {
                        continue;
                    }

                    Console.WriteLine(param.ToString());
                    if (param.TryBindValue(arg.OriginalValue))
                    {
                        if (param.ArgsToPop > 0)
                        {
                            param.ArgsToPop--;
                        }

                        // nested loop, continue outer :(
                        goto ParseNextArg;
                    }
                }

                // TODO: This looks horrible
                // TODO: optimize
                // Special logic for cases like: git clean -fdx (equivalent to: git clean -x -d- f)
                if (arg.OriginalValue.StartsWith("-"))
                {
                    bool argUsed = true;
                    foreach (var letter in arg.OriginalValue.Skip(1))
                    {
                        bool letterUsed = false;
                        foreach (var param in this)
                        {
                            if (param.CombiningSingleLetter == letter)
                            {
                                // TODO: for now errors ok, should print a warning though
                                if (param.TryBindValue("true"))
                                {
                                    letterUsed = true;
                                }
                            }
                        }

                        if (!letterUsed)
                        {
                            argUsed = false;
                        }
                    }

                    if (argUsed)
                    {
                        continue;
                    }
                }

                // we got to the end of the loop, no one consumed the arg (continue = consumed) so unfortunatelly:
                UnusedArgs.Add(arg.OriginalValue);

                ParseNextArg:;
            }

            bool ret = true;
            if (UnusedArgs.Count != 0)
            {
                foreach (var arg in UnusedArgs)
                {
                    Console.Error.WriteLine($"Error: Unused arg: {arg}");
                }

                Console.Error.WriteLine($"Error: Unused args are usually caused by typos.");
                Console.Error.WriteLine($"Error: They are forbidden by default because they may cause significant change in behavior.");
                Console.Error.WriteLine($"Error: I.e. instead of restarting computer you may shut it down.");

                ret = false;
            }

            foreach (var param in this)
            {
                if (param.Required && param.NumberOfArgsBound == 0)
                {
                    Console.Error.WriteLine($"Error: Required param `{param.ToString()}` not provided");
                    ret = false;
                }
            }

            return ret;
        }
    }
}
