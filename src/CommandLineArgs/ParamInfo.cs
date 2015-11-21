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

        private FieldInfo _field;

        public ParamInfo(int position, FieldInfo field)
        {
            _field = field;
        }

        public bool TrySetSingleValueFromNamedArg<T>(ref T obj, CommandLineArgs commandLineArgs)
        {
            return TrySetSingleValueFromNamedArg(ref obj, commandLineArgs, FieldSetter);
        }

        public bool FieldSetter(CommandLineArgs commandLineArgs, object retObj, string value, int? position)
        {
            object resolvedValue = StringConverters.ToType(value, _field.FieldType);
            if (resolvedValue != null)
            {
                if (position.HasValue)
                {
                    commandLineArgs.UseArg(position.Value);
                }

                _field.SetValue(retObj, value);
                return true;
            }

            return false;
        }

        public bool TrySetSingleValueFromNamedArg<T>(ref T obj, CommandLineArgs commandLineArgs, Func<CommandLineArgs, object, string, int?, bool> setter)
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
            if (commandLineArgs.CmdLineArgs[argPosition].Value != null)
            {
                if (setter(commandLineArgs, obj, commandLineArgs.CmdLineArgs[argPosition + 1].Value, argPosition + 1))
                {
                    return true;
                }
            }

            // -name value /name value
            if (commandLineArgs.CmdLineArgs[argPosition].Value == null
                && argPosition + 1 < commandLineArgs.CmdLineArgs.Length
                && commandLineArgs.CmdLineArgs[argPosition + 1].Name == null
                && commandLineArgs.CmdLineArgs[argPosition + 1].Value != null)
            {
                if (setter(commandLineArgs, obj, commandLineArgs.CmdLineArgs[argPosition + 1].Value, argPosition + 1))
                {
                    return true;
                }

                // TODO: should report that /name found but no valid value found?
            }

            // /flag
            if (commandLineArgs.CmdLineArgs[argPosition].Value == null)
            {
                if (_field.FieldType == typeof(bool) || _field.FieldType == typeof(bool?))
                {
                    _field.SetValue(obj, true);
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
                    object resolvedValue = StringConverters.ToType(value, _field.FieldType);
                    if (resolvedValue != null)
                    {
                        commandLineArgs.PopArg();
                        _field.SetValue(obj, resolvedValue);
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
                throw new ArgumentNullException("Required value {0} not set.", _field.Name);
            }
        }
    }
}
