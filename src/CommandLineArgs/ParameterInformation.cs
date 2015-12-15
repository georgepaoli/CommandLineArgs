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
        // TODO: following two fields should be in specialized version of this class
        public object Target;
        public FieldInfo Field;

        // Supported parameter metadata
        // TODO: Should all these metadata be auto generated from custom attributes?
        public List<string> Names = new List<string>();
        // TODO: How should Required work with popping args?
        public bool Required = false;
        public bool RequiredSuppressMessages = false;
        public bool PopsRemainingArgs = false;
        public bool NoDefaultAlias = false;
        public bool StopProcessingNamedArgsAfterThis = false;
        public HashSet<char> CombinableSingleLetterAliases = new HashSet<char>();
        public string Description = null;
        // TODO: should this value be resetable (and immutable)
        public int MaxArgsToPop = 0;

        // Output - is this info relevant
        public int NumberOfArgsBound = 0;

        public ParameterInformation(object target, FieldInfo field)
        {
            Target = target;
            Field = field;

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
                            CombinableSingleLetterAliases.Add(name[0]);
                        }
                    }
                }

                var asRequired = customAttribute as RequiredAttribute;
                if (asRequired != null)
                {
                    Required = true;
                    // TODO: too much abstraction, reduce it
                    RequiredSuppressMessages = asRequired.SupressMessages;
                }

                if (customAttribute as PopArgAttribute != null)
                {
                    MaxArgsToPop++;
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
                    CombinableSingleLetterAliases.Add(field.Name[0]);
                }

                Names.AddRange((new AliasAttribute(field.Name)).Names);
            }

            if (!Field.FieldType.IsAssignableFrom(typeof(bool)))
            {
                CombinableSingleLetterAliases.Clear();
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
            string ret = string.Join("|", Names);

            if (string.IsNullOrWhiteSpace(ret))
            {
                // TODO: info about PopArg, what is the best way to display it?
                //       (currently it is when other options not available...)
                ret = $"<{Field.Name}>";
            }
            return ret;
        }
    }
}
