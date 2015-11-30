using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    // TODO: Something doesn't feel right with naming of the class and method in here
    // TODO: this MUST BE at minimum [DefaultCommand(".*", "/number:1 /n=2 -n 3 --n 4 5")]
    [DefaultCommand(".*", "/number:1", "/n=2", "-n", "3", "--n", "4", "5")]
    public class ArraysExample1
    {
        // Should this configuration be auto detected?
        [NoDefaultAlias]
        [Alias("number|n")]
        [PopArg]
        // TODO: This type should literally be: `List of Numbers`
        public List<int> Numbers;

        public void Array_simple_example()
        {
            Assert.Equal(5, Numbers.Count);
            int checksum = 0;
            // order is not guaranteed if mixing different types of passing args
            foreach (var number in Numbers)
            {
                checksum += number;
            }

            Assert.Equal(1 + 2 + 3 + 4 + 5, checksum);
        }
    }
}
