using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public static class ConsoleApp
    {
        public static T FromCommandLineArgs<T>(string[] args)
        {
            T ret = Activator.CreateInstance<T>();

            ConsoleAppParams @params = new ConsoleAppParams(ret);
            @params.Args.AddArgs(args);
            if (!@params.Bind())
            {
            }

            return ret;
        }

        //public static void StartApp(string[] args)
        //{
        //    foreach (var type in typeof(ConsoleApp).GetTypeInfo().Assembly.DefinedTypes)
        //    {
        //        if (type.GetCustomAttribute(typeof(DefaultCommandAttribute)) != null)
        //        {
        //            StartApp(type, args);
        //        }
        //    }
        //}


        //public static void StartApp<T>(string[] args)
        //{
        //    StartApp(typeof(T).GetTypeInfo(), args);
        //}

    }
}
