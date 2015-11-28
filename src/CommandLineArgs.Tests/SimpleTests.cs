using System;

namespace CommandLineArgs.Tests
{
    [DefaultCommand(".*")]
    public class SimpleTests
    {
        public void When_args_are_not_provided_default_values_are_kept()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[0]);
            Assert.Equal(0, foo.IntValue);
            Assert.Equal(false, foo.BoolValue);
            Assert.Equal(101011101, foo.NullableInt);
            Assert.Equal(FooEnum.SomeEnumValue, foo.EnumValue);
        }

        public void It_works_with_integers()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/IntValue", "123" });
            Assert.Equal(123, foo.IntValue);
        }

        public void It_works_with_bools()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/BoolValue", "true" });
            Assert.True(foo.BoolValue);
        }

        public void It_works_with_strings()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/StringValue", "sometext" });
            Assert.Equal("sometext", foo.StringValue);
        }

        public void It_works_with_enums()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/EnumValue", "AnotherValue" });
            Assert.Equal(FooEnum.AnotherValue, foo.EnumValue);
        }

        public void Argument_names_are_case_insensitive_by_default()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/nuLLaBleinT", "500321" });
            Assert.Equal(500321, foo.NullableInt);
        }

        public void Name_and_value_can_be_also_separated_with_colon_or_equal_sign()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/intvalue:123", "/boolvalue=true" });
            Assert.Equal(123, foo.IntValue);
            Assert.Equal(true, foo.BoolValue);
        }

        public void Bools_are_additionally_accepted_also_without_an_explicit_value_as_it_is_treated_as_flag()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/boolvalue" });
            Assert.Equal(true, foo.BoolValue);
        }

        public void Extra_args_should_throw_as_they_may_completely_change_behavior_of_the_program_when_something_is_mistyped()
        {
            // TODO: decide on the exception type
            Assert.Throws<Exception>(() =>
            {
                ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/xyz", "sdfsdf", "ffffffffs" });
            });
        }

        public void Invalid_integer_is_treated_as_error_as_it_is_treated_as_unused_arg_which_is_an_error()
        {
            Assert.Throws<Exception>(() =>
            {
                ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/abc", "2340982342039480293840923480239480923840239840" });
            });
        }

        public void Invalid_enum_is_treated_as_error_as_it_is_treated_as_unused_arg_which_is_an_error()
        {
            Assert.Throws<Exception>(() =>
            {
                ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "/yay", "i love pink color mucho" });
            });
        }

        public void It_can_set_a_required_param()
        {
            ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { "/requiredintvalue", "123" });
        }

        public void It_throws_when_required_is_not_set()
        {
            // TODO: review exception types
            Assert.Throws<Exception>(() =>
            {
                ConsoleApp.FromCommandLineArgs<FooRequired>(new string[] { });
            });
        }

        public void It_accepts_values_as_unnamed_args_when_using_PopArg_attribute()
        {
            ExampleClass foo = ConsoleApp.FromCommandLineArgs<ExampleClass>(new string[] { "asd", "true" });
            Assert.Equal("asd", foo.StringValue); // pops arg
            Assert.Equal(0, foo.IntValue);
            Assert.True(foo.BoolValue); // pops arg
            Assert.Equal(101011101, foo.NullableInt);
            Assert.Equal(FooEnum.SomeEnumValue, foo.EnumValue);
        }
    }
}
