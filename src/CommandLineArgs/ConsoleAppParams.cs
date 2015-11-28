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

        public Queue<ParameterInformation> ArgPoppers = new Queue<ParameterInformation>();
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

        // TODO: this looks bad
        public void AddArgs(string[] args)
        {
            Args.AddArgs(args);
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

                if (ArgPoppers.Count != 0)
                {
                    // TODO: this is fucked up. It should be linked list or something more clever and it should iterate until the end when cannot bind
                    //       i.e. this won't work:
                    //       argoftype1 argoftype2 argoftype1 argoftype2
                    //       when binding to two PopsRemainingArgs
                    //       let's leave as is for now, not sure if there are other cases
                    ParameterInformation param = ArgPoppers.Peek();
                    if (!param.PopsRemainingArgs)
                    {
                        --param.ArgsToPop;
                        if (param.ArgsToPop == 0)
                        {
                            ArgPoppers.Dequeue();
                        }
                    }

                    if (param.TryBindValue(arg.OriginalValue))
                    {
                        continue;
                    }
                }

                // we got to the end of the loop, noone consumed (continue = consumed) so unfortunatelly:
                UnusedArgs.Add(arg.OriginalValue);
            }

            // Try to use Unused args
            // List<string> TryBind(List<string> unusedArgs)
            {
                List<string> reducedUnusedArgs = new List<string>();
                foreach (var arg in UnusedArgs)
                {
                    bool deleteArg = true;
                    if (!arg.StartsWith("-"))
                    {
                        deleteArg = false;
                    }
                    else
                    {
                        // TODO: start from position 1 - do i need to use regular loop or is there a better way to write this? (i.e. optimized slice(1))
                        for (int i = 1; i < arg.Length; i++)
                        {
                            bool letterUsed = false;
                            foreach (var param in this)
                            {
                                if (param.CombiningFlag == arg[i])
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
                                deleteArg = false;
                            }
                        }
                    }

                    if (!deleteArg)
                    {
                        reducedUnusedArgs.Add(arg);
                    }
                }

                // return
                UnusedArgs = reducedUnusedArgs;
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
