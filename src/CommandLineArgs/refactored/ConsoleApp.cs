using System;
using System.Collections.Generic;
using System.Linq;
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
            @params.Bind();

            return ret;
        }
    }
}
