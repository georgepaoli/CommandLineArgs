using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    internal class ParamInfo
    {
        public bool IsRequired = false;
        public bool CanPopArg = false;
        public bool IsPoppingRemainingArgs = false;
        public List<int> PositionsInCommandLineArgs;
        public FieldInfo Field;

        public ParamInfo(int position, FieldInfo field)
        {
            Field = field;
        }

        public bool FieldFromArgumentOnPositionSetter(CommandLineArgs commandLineArgs, object retObj, int position)
        {
            if (commandLineArgs.Args[position].Value == null)
            {
                return false;
            }

            object resolvedValue = StringConverters.ToType(commandLineArgs.Args[position].Value, Field.FieldType);
            if (resolvedValue != null)
            {
                commandLineArgs.UseArg(position);
                Field.SetValue(retObj, resolvedValue);
                return true;
            }

            return false;
        }

        public bool TryBindValueWithArgs<T>(ref T obj, CommandLineArgs commandLineArgs)
        {
            if (PositionsInCommandLineArgs == null)
            {
                return false;
            }

            if (PositionsInCommandLineArgs.Count == 0)
            {
                return false;
            }

            int argPosition = PositionsInCommandLineArgs[0];
            // -name=value  -name:value  /name=value  /name:value
            if (FieldFromArgumentOnPositionSetter(commandLineArgs, obj, argPosition))
            {
                return true;
            }

            // -name value /name value
            if (commandLineArgs.Args[argPosition].Value == null
                && argPosition + 1 < commandLineArgs.Args.Length
                && commandLineArgs.Args[argPosition + 1].Name == null)
            {
                if (FieldFromArgumentOnPositionSetter(commandLineArgs, obj, argPosition + 1))
                {
                    return true;
                }

                // TODO: should report that /name found but no valid value found?
            }

            // /flag
            if (commandLineArgs.Args[argPosition].Value == null)
            {
                if (Field.FieldType == typeof(bool) || Field.FieldType == typeof(bool?))
                {
                    commandLineArgs.UseArg(argPosition);
                    Field.SetValue(obj, true);
                }
            }

            return false;
        }

        public bool TrySetSingleValueFromConsecutiveArg<T>(ref T obj, CommandLineArgs commandLineArgs)
        {
            if (CanPopArg)
            {
                string value = commandLineArgs.PeekPopArg();
                if (value != null)
                {
                    object resolvedValue = StringConverters.ToType(value, Field.FieldType);
                    if (resolvedValue != null)
                    {
                        commandLineArgs.PopArg();
                        Field.SetValue(obj, resolvedValue);
                        return true;
                    }

                    // TODO: Should be intelligent about matching args here? (i.e. try to do it by checking type)
                }
            }

            return false;
        }

        public void ThrowIfRequiredArg()
        {
            if (IsRequired)
            {
                throw new ArgumentNullException("Required value {0} not set.", Field.Name);
            }
        }
    }
}
