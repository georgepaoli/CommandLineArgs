using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    enum FooEnum
    {
        Lol,
        xD
    }

#pragma warning disable 0649 // Disable: X is never assigned to, and will always have its default value
    internal class Foo
    {
        [PopArg]
        public string sdfsdfsdfasfijsdazgioea8rgh;
        public int Abc;
        [PopArg]
        public bool Def;
        public int? Ghi = 101011101;
        public FooEnum Yay;
    }

    internal class FooRequired
    {
        [Required]
        public int n;
    }
#pragma warning restore 0649

    [DefaultCommand(".*")]
    public class Example
    {
        public void IntentionallyFailing()
        {
            Assert.True(false);
        }

        public void EmptyArgListSetsValuesToDefault()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[0]);
            Assert.Equal(0, foo.Abc);
            Assert.Equal(false, foo.Def);
            Assert.Equal(101011101, foo.Ghi);
            Assert.Equal(FooEnum.Lol, foo.Yay);
        }

        public void SettingIntValue()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/Ghi", "123" });
            Assert.Equal(123, foo.Ghi);
        }

        public void SettingBoolValue()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/Def", "true" });
            Assert.True(foo.Def);
        }

        public void SettingStringValue()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/sdfsdfsdfasfijsdazgioea8rgh", "coconut" });
            Assert.Equal("coconut", foo.sdfsdfsdfasfijsdazgioea8rgh);
        }

        public void SettingEnumValue()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/Yay", "xd" });
            Assert.Equal(FooEnum.xD, foo.Yay);
        }

        public void ArgNamesAreCaseInsensitive()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/gHI", "500321" });
            Assert.Equal(500321, foo.Ghi);
        }

        public void ValuesAreAcceptedAlsoAsSingleArg()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/abc:123", "/def=true" });
            Assert.Equal(123, foo.Abc);
            Assert.Equal(true, foo.Def);
        }

        public void BoolsAreAcceptedAlsoWithoutExplicitValue()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/def" });
            Assert.Equal(true, foo.Def);
        }

        public void ImActuallyNotSureWhatToDoWIthExtraArgsAlthoughImPrettySureIDontWantToThrow()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/xyz", "sdfsdf", "ffffffffs" });
        }

        public void ImActuallyNotSureWhatToDoWIthInvalidIntegersAlthoughImPrettySureIDontWantToThrow()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/abc", "23409823420394802938409234802394809238402398402938402394823094823094820394823094823094820394823094820394823094823904" });
        }

        public void ImActuallyNotSureWhatToDoWIthInvalidEnumsAlthoughImPrettySureIDontWantToThrow()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "/yay", "i love pink color mucho" });
        }

        public void SettingRequiredFieldDoesNotThrow()
        {
            ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { "/n", "123" });
        }

        public void NotSettingRequiredFieldThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { }));
        }

        public void SettingInOrderUsingPopArg()
        {
            Foo foo = ConsoleApp.FromCommandLineArgs<Foo>(new string[] { "asd", "true" });
            Assert.Equal("asd", foo.sdfsdfsdfasfijsdazgioea8rgh); // set
            Assert.Equal(0, foo.Abc); // not set
            Assert.True(foo.Def); // set
            Assert.Equal(101011101, foo.Ghi);
            Assert.Equal(FooEnum.Lol, foo.Yay);
        }

        public static int Main(string[] args)
        {
            ConsoleApp.StartApp<Example>(args);
            return 0;
        }
    }
}
