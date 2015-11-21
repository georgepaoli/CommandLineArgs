using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    enum FooEnum
    {
        SomeEnumValue,
        AnotherValue
    }

#pragma warning disable 0649 // Disable: X is never assigned to, and will always have its default value
    internal class ExampleClass
    {
        [PopArg]
        public string StringValue;
        public int IntValue;
        [PopArg]
        public bool BoolValue;
        public int? NullableInt = 101011101;
        public FooEnum EnumValue;
    }

    internal class FooRequired
    {
        [Required]
        public int RequiredIntValue;
    }
#pragma warning restore 0649
}
