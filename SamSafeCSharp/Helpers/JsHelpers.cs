using System;
using System.Collections.Generic;

namespace SamNetMvc.Helpers
{
    public static class JsHelpers
    {
        public static List<string> map<T>(this List<T> list, Func<T, string> func)
        {
            var retval = new List<string>();
            foreach (var item in list)
            {
                retval.Add(func(item));
            }
            return retval;
        }

        public static bool exists(this string[] stringArray, string key)
        {
            foreach (var item in stringArray)
            {
                if (item == key) return true;
            }
            return false;
        }

        public static string join<T>(this List<T> list, string inbetween)
        {
            return string.Join(inbetween, list.ToArray());
        }

        public static string orDefault(this string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}