using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineArgs
{
    public class RunCommandsResult
    {
        // inputs
        public TypeInfo Type;
        public string[] Args;

        // outputs
        public int NumberOfRunCommands = 0;
        public int NumberOfFailedCommands = 0;

        // TODO: the story is too long, split to chapters
        public void StartApp()
        {
            object ret = Activator.CreateInstance(Type);

            if (Args.Length >= 1)
            {
                ConsoleApp app = 
                FromCommandLineArgs(t, ref ret, new CommandLineArgs(args.Slice(1)));
                RunCommandsResult result = new RunCommandsResult();
                result.RunCommands(MethodExtensions.GetListFromType(t));
                return RunCommands(ret, functions.MatchName(args[0]));
            }

            bool commandRun = false;
            foreach (var attribute in t.GetCustomAttributes())
            {
                var defaultCommand = attribute as DefaultCommandAttribute;
                if (defaultCommand != null)
                {
                    FromCommandLineArgs(t, ref ret, new CommandLineArgs(args));
                    return RunCommands(functions.MatchName(defaultCommand.Command), ret);
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
            foreach (var field in t.DeclaredFields)
            {
                if (!headerPrinted)
                {
                    Console.WriteLine("List of available parameters:");
                    headerPrinted = true;
                }

                PrintCommandDescription(field);
            }
        }

        public void RunCommands(object obj, IEnumerable<MethodInfo> functions)
        {
            // compiler should optimize it (if not the report it)
            bool hasManyCommands = functions.Count() >= 2;

            foreach (var function in functions)
            {
                NumberOfRunCommands++;

                if (hasManyCommands)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"--- Running {function.Name} ---");
                    Console.ResetColor();
                }

                try
                {
                    function.Invoke(obj);
                }
                catch (TargetInvocationException wrapped)
                {
                    NumberOfFailedCommands++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"!!! Finished running {function.Name} with exception !!!");
                    Console.ResetColor();
                    Console.Error.WriteLine(wrapped.InnerException);
                }
            }
        }

        public void PrintReport(RunCommandsResult result)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"---=== Finished running {result.NumberOfRunCommands} commands ===---");
            Console.ResetColor();

            if (result.NumberOfFailedCommands > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"---=== {result.NumberOfFailedCommands} of them failed ===---");
                Console.ResetColor();
            }
        }
    }
}
