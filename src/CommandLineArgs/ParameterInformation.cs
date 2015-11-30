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
        // TODO: is parent needed?
        public ConsoleAppParams Parent;
        public object Target;
        public FieldInfo Field;

        public List<string> Names = new List<string>();
        public bool Required = false;
        public int ArgsToPop = 0;
        public bool PopsRemainingArgs = false;
        public bool NoDefaultAlias = false;
        public bool StopProcessingNamedArgsAfterThis = false;
        public char? CombiningSingleLetter = null;
        public int NumberOfArgsBound = 0;
        public string Description = null;

        public ParameterInformation(ConsoleAppParams parent, object target, FieldInfo field)
        {
            Parent = parent;
            Target = target;
            Field = field;

            char? singleLetterAlias = null;
            foreach (var customAttribute in field.GetCustomAttributes())
            {
                var asAlias = customAttribute as AliasAttribute;
                if (asAlias != null)
                {
                    Names.AddRange(asAlias.Names);
                    foreach (var name in Names)
                    {
                        if (name.Length == 1)
                        {
                            singleLetterAlias = name[0];
                        }
                    }
                }

                if (customAttribute as RequiredAttribute != null)
                {
                    Required = true;
                }

                if (customAttribute as PopArgAttribute != null)
                {
                    ArgsToPop++;
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

                var asDescription = customAttribute as DescriptionAttribute;
                if (asDescription != null)
                {
                    Description = asDescription.Description;
                }
            }

            if (!NoDefaultAlias)
            {
                if (field.Name.Length == 1)
                {
                    singleLetterAlias = field.Name[0];
                }

                Names.AddRange((new AliasAttribute(field.Name)).Names);
            }

            if (Field.FieldType.IsAssignableFrom(typeof(bool)) && singleLetterAlias.HasValue)
            {
                CombiningSingleLetter = singleLetterAlias;
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

            IList list = Field.GetValue(Target) as IList;
            if (list == null)
            {
                list = (IList)Activator.CreateInstance(Field.FieldType);
                Field.SetValue(Target, list);
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
            Field.SetValue(Target, resolved);

            return true;
        }

        public bool TryBindValue(string value)
        {
            if (value == null)
            {
                return false;
            }

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

        public override string ToString()
        {
            // TODO: info about PopArg, what is the best way to display it?
            return string.Join("|", Names);
        }
    }
}
