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
        public List<int> PositionsInCommandLineArgs;

        private FieldInfo _field;

        public ParamInfo(int position, FieldInfo field)
        {
            _field = field;
        }

        public bool TrySetSingleValueFromNamedArg<T>(ref T obj, CommandLineArgs commandLineArgs)
        {
            if (PositionsInCommandLineArgs == null)
            {
                return false;
            }

            if (PositionsInCommandLineArgs.Count != 1)
            {
                return false;
            }

            object resolvedValue;
            int argPosition = PositionsInCommandLineArgs[0];
            commandLineArgs.UseArg(argPosition);
            if (commandLineArgs.CmdLineArgs[argPosition].Value == null)
            {
                int i = argPosition + 1;
                if (i < commandLineArgs.CmdLineArgs.Length && commandLineArgs.CmdLineArgs[i].Name == null && commandLineArgs.CmdLineArgs[i].Value != null)
                {
                    resolvedValue = StringConverters.ToType(commandLineArgs.CmdLineArgs[i].Value, _field.FieldType);
                    if (resolvedValue != null)
                    {
                        commandLineArgs.UseArg(i);
                        _field.SetValue(obj, resolvedValue);
                        // TODO: should report invalid type?
                        return true;
                    }
                }

                // Bool is the only exception where value is not required as presence of flag tells you "true"
                if (_field.FieldType == typeof(bool))
                {
                    _field.SetValue(obj, true);
                    return true;
                }

                // TODO: should report that /name found but value is missing?
            }
            else
            {
                resolvedValue = StringConverters.ToType(commandLineArgs.CmdLineArgs[argPosition].Value, _field.FieldType);
                if (resolvedValue != null)
                {
                    _field.SetValue(obj, resolvedValue);
                    return true;
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
