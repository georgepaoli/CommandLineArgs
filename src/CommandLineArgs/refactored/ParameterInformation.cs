using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public class ParameterInformation
    {
        public FieldInfo Field = null;
        public Type OutputType { get { return Field.FieldType; } }

        public List<string> Names = new List<string>();
        public bool Required = false;
        public bool CanPopArg = false;
        public bool PopsRemainingArgs = false;
        public bool NoDefaultAlias = false;

        public static ParameterInformation FromField(FieldInfo field)
        {
            var ret = new ParameterInformation() { Field = field };

            foreach (var customAttribute in field.GetCustomAttributes())
            {
                var asAlias = customAttribute as AliasAttribute;
                if (asAlias != null)
                {
                    ret.Names.Add(asAlias.Name);
                }

                if (customAttribute as RequiredAttribute != null)
                {
                    ret.Required = true;
                }

                if (customAttribute as PopArgAttribute != null)
                {
                    ret.CanPopArg = true;
                }

                if (customAttribute as PopRemainingArgs != null)
                {
                    ret.PopsRemainingArgs = true;
                }

                if (customAttribute as NoDefaultAliasAttribute != null)
                {
                    ret.NoDefaultAlias = true;
                }
            }

            if (!ret.NoDefaultAlias)
            {
                ret.Names.Add(field.Name);
            }

            return ret;
        }
    }
}
