using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CommandLineArgs
{
    internal static class MethodExtensions
    {
        public static void Invoke(this MethodInfo method, object obj)
        {
            method.Invoke(method.IsStatic ? null : obj, null);
        }

        public static IEnumerable<MethodInfo> GetListFromType(TypeInfo t)
        {
            foreach (var method in t.DeclaredMethods)
            {
                if (method.IsPublic && method.GetParameters().Length == 0 && method.ReturnType == typeof(void))
                {
                    yield return method;
                }
            }
        }

        public static IEnumerable<MethodInfo> MatchName(this IEnumerable<MethodInfo> functions, string namePattern)
        {
            Regex regex = new Regex(namePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            foreach (var method in functions)
            {
                if (regex.IsMatch(method.Name))
                {
                    yield return method;
                }
            }
        }
    }
}
