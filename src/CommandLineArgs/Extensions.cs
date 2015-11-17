using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    public static class Extensions
    {
        public static T[] Slice<T>(this T[] array, int index, int length)
        {
            T[] ret = new T[length];
            Array.Copy(array, index, ret, 0, length);
            return ret;
        }

        public static T[] Slice<T>(this T[] array, int index)
        {
            return Slice(array, index, array.Length - index);
        }
    }
}
