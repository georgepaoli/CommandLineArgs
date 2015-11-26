using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ParameterInformation
    {
        public ConsoleAppParams Parent;
        public FieldInfo Field;

        public List<string> Names = new List<string>();
        public bool Required = false;
        public bool CanPopArg = false;
        public bool PopsRemainingArgs = false;
        public bool NoDefaultAlias = false;
        public bool StopProcessingNamedArgsAfterThis = false;
        public int NumberOfArgsBound = 0;
        public List<int> PositionsInArgs = new List<int>();

        public ParameterInformation(ConsoleAppParams parent, FieldInfo field)
        {
            Parent = parent;
            Field = field;

            foreach (var customAttribute in field.GetCustomAttributes())
            {
                var asAlias = customAttribute as AliasAttribute;
                if (asAlias != null)
                {
                    Names.Add(asAlias.Name);
                }

                if (customAttribute as RequiredAttribute != null)
                {
                    Required = true;
                }

                if (customAttribute as PopArgAttribute != null)
                {
                    CanPopArg = true;
                }

                if (customAttribute as PopRemainingArgsAttribute != null)
                {
                    PopsRemainingArgs = true;
                }

                if (customAttribute as LastProcessedNamedArgAttribute != null)
                {
                    StopProcessingNamedArgsAfterThis = true;
                }

                if (customAttribute as NoDefaultAliasAttribute != null)
                {
                    NoDefaultAlias = true;
                }
            }

            if (!NoDefaultAlias)
            {
                Names.Add(field.Name);
            }
        }

        private bool TryAddValueToList(string value)
        {
            if (Field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
            {
                return false;
            }

            // This must have exactly one arg - this is List<T>
            Type underlyingType = Field.FieldType.GetGenericArguments()[0];

            object resolved = StringToValueType.ToType(value, underlyingType);
            if (resolved == null)
            {
                return false;
            }

            IList list = Field.GetValue(Parent.Object) as IList;
            if (list == null)
            {
                list = (IList)Activator.CreateInstance(Field.FieldType);
                Field.SetValue(Parent.Object, list);
            }

            return list.Add(resolved) != -1;
        }

        private bool TryAddValueToField(string value)
        {
            object resolved = StringToValueType.ToType(value, Field.FieldType);
            if (resolved == null)
            {
                return false;
            }

            if (NumberOfArgsBound >= 1)
            {
                return false;
            }

            NumberOfArgsBound++;
            Field.SetValue(Parent.Object, resolved);

            return true;
        }

        public bool TryBindValue(string value)
        {
            if (TryAddValueToField(value))
            {
                return true;
            }

            if (TryAddValueToList(value))
            {
                return true;
            }

            return false;
        }
    }
}
