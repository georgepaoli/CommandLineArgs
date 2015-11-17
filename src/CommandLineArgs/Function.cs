using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CommandLineArgs
{
    internal class Function
    {
        public object Object;

        // TODO: Regex
        public string Name;

        public bool Invoke()
        {
            MethodInfo defaultMethod = null;
            string nameToCompare = Name.ToLowerInvariant();
            foreach (var method in Object.GetType().GetTypeInfo().DeclaredMethods)
            {
                if (method.Name.ToLowerInvariant() == nameToCompare
                    && method.GetParameters().Length == 0)
                {
                    method.Invoke(Object, null);
                    return true;
                }

                if (defaultMethod == null)
                {
                    foreach (var customAttribute in method.GetCustomAttributes())
                    {
                        if (customAttribute as DefaultFunctionAttribute != null)
                        {
                            defaultMethod = method;
                            break;
                        }
                    }
                }
            }

            if (defaultMethod != null)
            {
                defaultMethod.Invoke(Object, null);
                return true;
            }

            return false;
        }
    }
}
