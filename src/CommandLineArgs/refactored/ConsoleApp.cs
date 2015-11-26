using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public static class ConsoleApp
    {
        public static T FromCommandLineArgs<T>(string[] args)
        {
            T ret = Activator.CreateInstance<T>();

            ConsoleAppParams app = new ConsoleAppParams(ret);
            app.Args.AddArgs(args);
            if (!app.Bind())
            {
                throw new ArgumentException();
            }

            return ret;
        }

        private static void StartCommand(string command, string[] args)
        {

        }

        private static IEnumerable<MethodInfo> FilterCommands(IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                if (method.ReturnType != typeof(void))
                {
                    continue;
                }

                if (method.GetParameters().Length != 0)
                {
                    // TODO: implement this
                    continue;
                }

                if (method.GetGenericArguments().Length != 0)
                {
                    // TODO: how would that work ???
                    continue;
                }

                yield return method;
            }
        }

        private static IEnumerable<MethodInfo> FindCommand(string pattern, IEnumerable<MethodInfo> methods)
        {
            Regex namePattern = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            foreach (var method in methods)
            {
                if (!namePattern.IsMatch(method.Name))
                {
                    // TODO: heuristics for similar match?
                    continue;
                }

                yield return method;
            }
        }

        private static void DefaultPrintUsage()
        {
            Console.WriteLine("Usage:");
            foreach (var param in app)
            {
                PrintCommandDescription(param);
            }
        }

        public static void StartApp(string[] args)
        {
            if (args.Length == 0)
            {
                // TODO: Print Help
            }

            foreach (var type in typeof(ConsoleApp).GetTypeInfo().Assembly.DefinedTypes)
            {
                if (type.GetCustomAttribute(typeof(DefaultCommandAttribute)) != null)
                {
                    object ret = Activator.CreateInstance(type.AsType());
                    ConsoleAppParams app = new ConsoleAppParams(ret);

                    var methods = FindCommand(args[0], FilterCommands(type.DeclaredMethods));

                    if (methods.Any())
                    {
                        // shift args
                        args = args.Slice(1);
                    }
                    else
                    {
                        // TODO: add customization
                        DefaultPrintUsage();
                        return;
                    }

                    
                    app.Args.AddArgs(args);
                    app.Bind();

                    foreach (var method in methods)
                    {
                        object target = method.IsStatic ? null : ret;

                        // TODO: feels like adding arguments here and below wouldn't be that hard
                        method.Invoke(target);
                    }
                }
            }
        }

        private static void PrintCommandDescription(ParameterInformation param)
        {
            Console.Write("    ");

            if (!param.Required)
            {
                Console.Write("[");
            }

            string.Join("|", param.Names);
            
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
    }
}
