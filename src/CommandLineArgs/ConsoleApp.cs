using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleApp
    {
        public List<ConsoleAppParams> Params = new List<ConsoleAppParams>();
        public List<MethodInfo> Commands = new List<MethodInfo>();
        public bool PrintHelp = false;
        public bool AnyCommandRun = false;
        public int NumberOfRunCommands = 0;
        public int NumberOfFailedCommands = 0;

        public static T FromCommandLineArgs<T>(string[] args)
        {
            T ret = Activator.CreateInstance<T>();

            ConsoleAppParams appParams = new ConsoleAppParams();
            appParams.AddTarget(ret);
            appParams.AddArgs(args);
            if (!appParams.Bind())
            {
                throw new ArgumentException("Could not bind args");
            }

            return ret;
        }

        // TODO:
        //public static int StartApp<T>(string[] args)
        //{
        //}

        public static int StartApp(Assembly assembly, string[] args)
        {
            ConsoleApp app = new ConsoleApp();

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.AsType().GetConstructor(Type.EmptyTypes) == null)
                {
                    // no parameterless constructor
                    continue;
                }

                object target = Activator.CreateInstance(type.AsType());
                var typeParams = new ConsoleAppParams();
                typeParams.AddTarget(target);
                app.Params.Add(typeParams);

                var typeCommands = type.DeclaredMethods.GetCommands();
                app.Commands.AddRange(typeCommands);
                string command = null;

                var defaultCmd = type.GetCustomAttribute(typeof(DefaultCommandAttribute)) as DefaultCommandAttribute;
                if (defaultCmd != null)
                {
                    command = defaultCmd.Command;
                }
                else
                {
                    if (args.Length > 0)
                    {
                        command = args[0];
                        args = args.Slice(1);
                    }
                }

                if (command == null)
                {
                    app.PrintHelp = true;
                    continue;
                }

                typeParams.AddArgs(args);
                if (!typeParams.Bind())
                {
                    app.PrintHelp = true;
                    continue;
                }

                var matchedCommands = typeCommands.MatchName(command);

                if (!matchedCommands.Any())
                {
                    app.PrintHelp = true;
                    continue;
                }

                foreach (var method in matchedCommands)
                {
                    // TODO: feels like adding arguments here and below wouldn't be that hard
                    try
                    {
                        Console.WriteLine($"---=== Running `{method.Name}` ===---");
                        method.Invoke(
                            obj: method.IsStatic ? null : target,
                            parameters: null);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"---=== `{method.Name}` finished successfuly ===---");
                        Console.ResetColor();
                    }
                    catch (TargetInvocationException wrapped)
                    {
                        app.NumberOfFailedCommands++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"---=== Error when running `{method.Name}` ===---");
                        Console.ResetColor();
                        Console.Error.WriteLine(wrapped.InnerException);
                    }

                    app.AnyCommandRun = true;
                    app.NumberOfRunCommands++;
                }
            }

            app.PrintHelp &= !app.AnyCommandRun;

            if (app.PrintHelp)
            {
                app.DefaultPrintUsage();
            }

            if (app.AnyCommandRun)
            {
                app.PrintReport();
            }

            return app.AnyCommandRun ? 0 : 1;
        }

        public void PrintListOfParams()
        {
            bool headerPrinted = false;
            foreach (var typeParams in Params)
            {
                foreach (var param in typeParams)
                {
                    if (!headerPrinted)
                    {
                        Console.WriteLine("Usage:");
                        headerPrinted = true;
                    }

                    PrintParamDescription(param);
                }
            }
        }

        public void PrintListOfCommands()
        {
            bool headerPrinted = false;
            foreach (var command in Commands)
            {
                if (!headerPrinted)
                {
                    Console.WriteLine("Commands:");
                    headerPrinted = true;
                }

                Console.WriteLine($"    {command.Name}");
            }
        }

        // TODO: add customization of help
        public void DefaultPrintUsage()
        {
            PrintListOfParams();
            PrintListOfCommands();
        }

        private static void PrintParamDescription(ParameterInformation param)
        {
            Console.Write("    ");

            if (!param.Required)
            {
                Console.Write("[");
            }

            Console.Write(param.ToString());
            
            Console.Write($"=<{param.Field.FieldType.Name}>");

            if (!param.Required)
            {
                Console.Write("]  ");
            }

            if (param.Description != null)
            {
                Console.Write(param.Description);
            }

            Console.WriteLine();
        }

        public void PrintReport()
        {
            if (NumberOfRunCommands >= 2)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"---=== Finished running {NumberOfRunCommands} commands ===---");
                Console.ResetColor();

                if (NumberOfFailedCommands > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"---=== {NumberOfFailedCommands} of them failed ===---");
                    Console.ResetColor();
                }
            }
        }
    }
}
