using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ConsoleApp
    {
        public ConsoleAppParams Params = new ConsoleAppParams();
        public CommandLineArgs Args = new CommandLineArgs();

        public static T FromCommandLineArgs<T>(string[] args)
        {
            T ret = Activator.CreateInstance<T>();

            ConsoleApp app = new ConsoleApp();
            app.Params.AddParametersFromType(typeof(T));
            app.Args.AddArgs(args);
            app.ReadValueIntoObject(ret);

            return ret;
        }

        public void ReadValueIntoObject(object obj)
        {
            Type type = obj.GetType();

            throw new NotImplementedException();
        }
    }
}
