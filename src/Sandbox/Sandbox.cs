using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Sandbox
{
    [DefaultRunAllCommands] // TODO: make this work instead of the bottom option - or perhaps make it a default
    [DefaultCommand(".*")]
    public class Sandbox
    {
        public void Start()
        {
            Console.WriteLine("Hello World!");
        }

        public void PrintTypeInfo(Type t)
        {
            var ti = t.GetTypeInfo();
            Console.WriteLine($"{ti.FullName}, IsArray? {ti.IsArray}, UnderlyingType: {ti.GetElementType()}");
        }

        public void TypeOfArray()
        {
            PrintTypeInfo(typeof(int[]));
            PrintTypeInfo(typeof(string[]));
            PrintTypeInfo(typeof(List<int>));
            PrintTypeInfo(typeof(List<int>[]));
            PrintTypeInfo(typeof(IEnumerable<int>));
        }
    }
}
