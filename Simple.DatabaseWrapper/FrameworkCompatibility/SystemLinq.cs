#if NET20
using System.Collections.Generic;

namespace System.Linq
{
    internal static class SystemLinq
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> filter)
        {
            foreach (var v in source)
            {
                if (filter(v)) yield return v;
            }
        }
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (var v in source)
            {
                yield return selector(v);
            }
        }
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            foreach (var v in source)
            {
                return v;
            }
            return default;
        }
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var v in source)
            {
                if (predicate(v)) return true;
            }
            return false;
        }
        public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            foreach (var v in first) yield return v;
            foreach (var v in second) yield return v;
        }
        public static IList<T> ToList<T>(this IEnumerable<T> source)
        {
            return new List<T>(source);
        }
        // Not great, but works for small things
        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            var lst = source.ToList();
            T[] arr = new T[lst.Count];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = lst[i];
            }
            return arr;
        }

    }
}
#endif
