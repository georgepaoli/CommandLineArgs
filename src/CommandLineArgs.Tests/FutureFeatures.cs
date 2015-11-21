using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    [DefaultCommand("FutureFeature_.*")]
    public class FutureFeatures
    {
        public bool a;
        public bool b;
        public bool c;

        public void FutureFeature_SupportsMultipleSingleCharBoolFlags()
        {
            ConsoleApp.StartApp<FutureFeatures>(new string[] { "-abc" });
        }

        public void Start()
        {
            Assert.True(a);
            Assert.True(b);
            Assert.True(c);
        }
    }
}
