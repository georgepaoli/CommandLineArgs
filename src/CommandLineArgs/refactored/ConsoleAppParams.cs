﻿using System;
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

        private static string GetPrintableString(string s)
        {
            return s == null ? "null" : $"`{s}`";
        }

        // TODO: split or leave as is (easy to understand vs easy to fix)
        public bool Bind()
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
                        if (param.StopProcessingNamedArgsAfterThis)
                        {
                            ignoreNames = true;
                        }

                        if (param.TryBindValue(arg.Value))
                        {
                            continue;
                        }

                        if (arg.Operator != null)
                        {
                            Console.Error.WriteLine($"Unrecognized value: {GetPrintableString(arg.Value)} for {GetPrintableString(arg.Name)}.");
                            Console.Error.WriteLine($"Expected type: {GetPrintableString(param.Field.FieldType.FullName)}.");
                            Console.Error.WriteLine($"Note: Type might not be supported or there might be something wrong with this library");
                            Console.Error.WriteLine($"      File an issue if you think it is wrong");
                            continue;
                        }

                        if (++i >= Args.Count)
                        {
                            break;
                        }
                        if (param.TryBindValue(Args[i].OriginalValue))
                        {
                            continue;
                        }
                        --i;

                        // bool doesn't require value
                        if (param.TryBindValue("true"))
                        {
                            continue;
                        }

                        Console.Error.WriteLine($"No value found for `{arg.Name}`. Expected type: {param.Field.FieldType.FullName}.");
                        Console.Error.WriteLine($"Note: Type might not be supported or there might be something wrong with this library");
                        Console.Error.WriteLine($"      File an issue if you think it is wrong");
                        continue;
                    }
                } // if (!ignoreNames)
                else if (ArgPoppers.Count != 0)
                {
                    ParameterInformation param = ArgPoppers.Peek();
                    if (--param.ArgsToPop == 0)
                    {
                        ArgPoppers.Dequeue();
                    }

                    if (param.TryBindValue(arg.OriginalValue))
                    {
                        continue;
                    }
                }

                // we got to the end of the loop, noone consumed (continue = consumed) so unfortunatelly:
                UnusedArgs.Add(arg.OriginalValue);
            }

            if (UnusedArgs.Count != 0)
            {
                foreach (var arg in UnusedArgs)
                {
                    Console.Error.WriteLine($"Error: Unused arg: {arg}");
                }

                return false;
            }


            // TODO: throw on required
            return true;
        }
    }
}
