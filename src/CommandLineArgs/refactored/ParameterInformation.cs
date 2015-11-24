using System;
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

                if (customAttribute as PopRemainingArgs != null)
                {
                    PopsRemainingArgs = true;
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

        public bool TryBindValue(string value)
        {
            object resolved = StringToValueType.ToType(value, Field.FieldType);
            if (resolved == null)
            {
                return false;
            }

            NumberOfArgsBound++;

            
            {
                if (Field.FieldType.)
                {
                    return false;
                }

                isList = true;
            }
            if (isList)
            {
                object list = Field.GetValue(Parent.Object);

            }
            else
            {
                if (NumberOfArgsBound >= 1)
                {
                    Field.SetValue(Parent.Object, resolved);
                }
            }

            return true;
        }
    }
}
