using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Root
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> DistinctBy<T, K> (this IEnumerable<T> source, Func<T, K> keySelector)
        {
            HashSet<K> seenKeys = new HashSet<K>();
            foreach (T element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in source) action(e, i++);
        }
    }
}
