using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    // TODO: Make simpler syntax for this
    // TODO: i.e.: [DefaultCommand(".*", "-abc /a /s /d /f")]
    // TODO: or even: [DefaultCommand(".* -abc /a /s /d /f")] <-- although here there might be ambiguities
    [DefaultCommand(".*", "-abc")]
    public class MultipleSingleCharBoolFlags
    {
        public bool a;
        public bool b;
        public bool c;

        // TODO: Feels like this should be MultipleSingleCharBoolFlags_Simple_example(bool a, bool b, bool c)
        // TODO: or maybe this whole file should be a function
        public void MultipleSingleCharBoolFlags_Simple_example()
        {
            Assert.True(a);
            Assert.True(b);
            Assert.True(c);
        }
    }
}
