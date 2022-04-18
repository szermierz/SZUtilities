using System.Collections.Generic;

namespace SZUtilities.Extensions
{
    public static class ArrayMethods
    {
        public static List<T> SortedArraysDiff<T>(IList<T> lhs, IList<T> rhs, List<T> cacheResult = null)
        {
            cacheResult ??= new List<T>();

            var i = 0;
            var j = 0;
            var n = lhs.Count;
            var m = rhs.Count;

            while (i < n && j < m)
            {
                if (Comparer<T>.Default.Compare(lhs[i], rhs[j]) < 0)
                {
                    cacheResult.Add(lhs[i]);
                    i++;
                }
                else if (Comparer<T>.Default.Compare(lhs[i], rhs[j]) > 0)
                {
                    cacheResult.Add(rhs[i]);
                    j++;
                }
                else
                {
                    i++;
                    j++;
                }
            }

            return cacheResult;
        }
    }
}