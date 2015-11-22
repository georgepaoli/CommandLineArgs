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

        public static bool TryAdd<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary[key] = value;
            return true;
        }

        public static void AddValueToList<K, T>(this Dictionary<K, List<T>> dictionary, K key, T value)
        {
            List<T> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                dictionary[key] = list = new List<T>();
            }

            list.Add(value);
        }
    }
}
