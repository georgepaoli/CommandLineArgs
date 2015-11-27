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
        public ConsoleAppParams Params = new ConsoleAppParams();

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

        public static int StartApp(string[] args)
        {
            foreach (var type in typeof(ConsoleApp).GetTypeInfo().Assembly.DefinedTypes)
            {
                ConsoleApp app = new ConsoleApp();
                object target = Activator.CreateInstance(type);
                app.Params.AddTarget(target);

                var defaultCmd = type.GetCustomAttribute(typeof(DefaultCommandAttribute)) as DefaultCommandAttribute;
                if (defaultCmd != null)
                {
                    var methods = type.DeclaredMethods.GetCommands().MatchName(defaultCmd.Command);

                    app.Params.AddArgs(args);
                    app.Params.Bind();

                    foreach (var method in methods)
                    {
                        // TODO: feels like adding arguments here and below wouldn't be that hard
                        method.Invoke(
                            obj: method.IsStatic ? null : target,
                            parameters: null);
                    }
                }

                //args[0]
                //if (methods.Any())
                //{
                //    // shift args
                //    args = args.Slice(1);
                //}
                //else
                //{
                //    // TODO: add customization of help
                //    DefaultPrintUsage();
                //    return;
                //}
            }
        }

        private void DefaultPrintUsage()
        {
            Console.WriteLine("Usage:");
            foreach (var param in Params)
            {
                PrintParamDescription(param);
            }
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
    }
}
