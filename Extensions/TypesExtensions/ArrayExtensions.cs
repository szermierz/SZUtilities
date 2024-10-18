using System.Collections;
using System.Collections.Generic;

namespace SZUtilities.Extensions
{
    public struct ArrayEnumerator<T> 
        : IEnumerator<T>
        , IEnumerator
    {
        private IReadOnlyList<T> m_collection;
        private int m_index;

        public ArrayEnumerator(IReadOnlyList<T> collection)
        {
            m_collection = collection;
            m_index = -1;
        }

        public readonly T Current => m_collection[m_index];

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            m_collection = null;
            m_index = -1;
        }

        public bool MoveNext()
        {
            ++m_index;
            return m_index < m_collection.Count;
        }

        public void Reset()
        {
            m_index = -1;
        }
    }

    public static class ArrayMethods
    {
        public static ArrayEnumerator<T> GetNonAllocatingEnumerator<T>(this IReadOnlyList<T> collection)
        {
            return new ArrayEnumerator<T>(collection);
        }

        public static List<T> SortedArraysDiff<T>(IList<T> lhs, IList<T> rhs, List<T> cacheResult = null)
        {
            if(null == cacheResult)
                cacheResult = new List<T>();

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