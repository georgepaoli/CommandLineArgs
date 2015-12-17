using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    [DefaultCommand(".*")]
    public class PopArgsWithNamedArgs
    {
        [PopArg]
        public string a;
        [PopArg]
        public string b;

        public void Tricky_binding_order_case()
        {
            var app = ConsoleApp.FromCommandLineArgs<PopArgsWithNamedArgs>("bvalue", "/a:avalue");
            Assert.Equal("avalue", app.a);
            Assert.Equal("bvalue", app.b);
        }
    }
}
