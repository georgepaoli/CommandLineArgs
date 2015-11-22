using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    // TODO: add description for help
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        public string Description;

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    // TODO: add something like this - see [Alias] comment as it might be consolidated
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

    // TODO: This should accept following forms:
    // "name" - adds: --name, -name, /name
    // "-name" - adds only -name
    // "n|name" - adds -n --n /n --name -name /name
    // "-n|name"....
    // you got the idea
    /// <summary>
    /// Alternative name for parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public string Name;

        public AliasAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Disables automatically adding field name as an alias
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NoDefaultAliasAttribute : Attribute
    {
    }

    /// <summary>
    /// Marks parameter as required.
    /// Exception will be thrown when nothing bound.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// For single values: If not bound by name then tries to bind with first free argument from the left if available and when the conversion to target parameter is possible
    /// For arrays: Takes first free arg from the left if available and when the conversion to target parameter is possible
    /// Possible to use it multiple times on arrays (i.e. when exact amount of args to pop is known)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PopArgAttribute : Attribute
    {
    }

    /// <summary>
    /// Binds with all remaining args which can be converted to target parameter
    /// </summary>
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
