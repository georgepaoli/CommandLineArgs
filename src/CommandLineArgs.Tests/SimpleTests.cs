using System;

namespace CommandLineArgs.Tests
{
    [DefaultCommand(".*")]
    public class SimpleTests
    {
        //public void IntentionallyFailing()
        //{
        //    Assert.True(false);
        //}

        public void EmptyArgListSetsValuesToDefault()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[0]);
            Assert.Equal(0, foo.IntValue);
            Assert.Equal(false, foo.BoolValue);
            Assert.Equal(101011101, foo.NullableInt);
            Assert.Equal(FooEnum.SomeEnumValue, foo.EnumValue);
        }

        public void SettingIntValue()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/IntValue", "123" });
            Assert.Equal(123, foo.IntValue);
        }

        public void SettingBoolValue()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/BoolValue", "true" });
            Assert.True(foo.BoolValue);
        }

        public void SettingStringValue()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/StringValue", "sometext" });
            Assert.Equal("sometext", foo.StringValue);
        }

        public void SettingEnumValue()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/EnumValue", "AnotherValue" });
            Assert.Equal(FooEnum.AnotherValue, foo.EnumValue);
        }

        public void ArgNamesAreCaseInsensitive()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/nuLLaBleinT", "500321" });
            Assert.Equal(500321, foo.NullableInt);
        }

        public void ValuesAreAcceptedInDifferentFormats()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/intvalue:123", "/boolvalue=true" });
            Assert.Equal(123, foo.IntValue);
            Assert.Equal(true, foo.BoolValue);
        }

        public void BoolsAreAcceptedAlsoWithoutExplicitValue()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/boolvalue" });
            Assert.Equal(true, foo.BoolValue);
        }

        public void ImActuallyNotSureWhatToDoWIthExtraArgsAlthoughImPrettySureIDontWantToThrow()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/xyz", "sdfsdf", "ffffffffs" });
        }

        public void ImActuallyNotSureWhatToDoWIthInvalidIntegersAlthoughImPrettySureIDontWantToThrow()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/abc", "2340982342039480293840923480239480923840239840" });
        }

        public void ImActuallyNotSureWhatToDoWIthInvalidEnumsAlthoughImPrettySureIDontWantToThrow()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/yay", "i love pink color mucho" });
        }

        public void SettingRequiredFieldDoesNotThrow()
        {
            ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { "/requiredintvalue", "123" });
        }

        public void NotSettingRequiredFieldThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { }));
        }

        public void SettingInOrderUsingPopArg()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "asd", "true" });
            Assert.Equal("asd", foo.StringValue); // set
            Assert.Equal(0, foo.IntValue); // not set
            Assert.True(foo.BoolValue); // set
            Assert.Equal(101011101, foo.NullableInt);
            Assert.Equal(FooEnum.SomeEnumValue, foo.EnumValue);
        }

        public static int Main(string[] args)
        {
            ConsoleApp.StartApp<SimpleTests>(args);
            return 0;
        }
    }
}
