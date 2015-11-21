using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        public string Description;

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    // TODO: add something like this
    [Flags]
    public enum AliasType
    {
        Value = 0,
        Any = SlashName | SlashNameColonValue | SlashNameEqualsValue | DashName | DashDashName | DashDashNameEqualsValue,
        SlashName = 1,
        SlashNameColonValue = 2,
        SlashNameEqualsValue = 4,
        DashName = 8,
        DashDashName = 16,
        DashDashNameEqualsValue = 32
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public string Name;

        public AliasAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PopArgAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PopRemainingArgs : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class DefaultCommandAttribute : Attribute
    {
        public string Command;

        public DefaultCommandAttribute(string command)
        {
            Command = command;
        }
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class DefaultRunAllCommandsAttribute : Attribute
    {
        public string Command;

        public DefaultRunAllCommandsAttribute()
        {
            Command = ".*";
        }
    }
}
