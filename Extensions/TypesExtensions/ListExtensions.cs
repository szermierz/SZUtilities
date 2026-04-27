using System;
using System.Collections.Generic;

namespace SZUtilities.Extensions
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int size, T value)
        {
            while (list.Count > size)
                list.RemoveAt(list.Count - 1);
            while(list.Count < size)
                list.Add(value);
        }

        public static int Remove<T>(this List<T> list, IEnumerable<T> range)
        {
            var result = 0;

            foreach (var item in range)
            {
                var index = list.IndexOf(item);
                if (index < 0)
                    continue;

                list.RemoveAt(index);
                ++result;
            }

            return result;
        }
        public static int Remove<T>(this List<T> list, IEnumerator<T> range)
        {
            var result = 0;

            while(range.MoveNext())
            {
                var item = range.Current;
                var index = list.IndexOf(item);
                if (index < 0)
                    continue;

                list.RemoveAt(index);
                ++result;
            }

            return result;
        }

        public static bool RemoveUnordered<T>(this List<T> list, T value)
        {
            var index = list.IndexOf(value);
            if (index < 0)
                return false;

            RemoveUnorderedAt(list, index);
            return true;
        }

        public static int RemoveUnordered<T>(this List<T> list, IEnumerable<T> range)
        {
            var result = 0;

            foreach (var item in range)
            {
                if (list.RemoveUnordered(item))
                    ++result;
            }

            return result;
        }

        public static int RemoveUnordered<T>(this List<T> list, IEnumerator<T> range)
        {
            var result = 0;

            while (range.MoveNext())
            {
                var item = range.Current;
                if(list.RemoveUnordered(item))
                    ++result;
            }

            return result;
        }

        public static void RemoveUnorderedAt<T>(this List<T> list, int index)
        {
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }

        public static T PopUnorderedAt<T>(this List<T> list, int index)
        {
            var result = list[index];
            list.RemoveUnorderedAt(index);
            return result;
        }

        public static T PopUnordered<T>(this List<T> list)
        {
            return list.PopUnorderedAt(list.Count - 1);
        }

        public static void UnorderedDistinct<T>(this List<T> list)
        {
            if (list == null || list.Count <= 1)
                return;

            if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                list.Sort();
            else
                list.Sort(ComparerCache<T>.Instance);

            var writeIndex = 1;
            for (var readIndex = 1; readIndex < list.Count; readIndex++)
            {
                if (!EqualityComparer<T>.Default.Equals(list[readIndex], list[writeIndex - 1]))
                    list[writeIndex++] = list[readIndex];
            }

            if (writeIndex < list.Count)
                list.RemoveRange(writeIndex, list.Count - writeIndex);
        }

        private static class ComparerCache<T>
        {
            public static readonly IComparer<T> Instance = new EqualityBasedComparer<T>();
        }

        private class EqualityBasedComparer<T> : IComparer<T>
        {
            private static readonly EqualityComparer<T> Eq = EqualityComparer<T>.Default;

            public int Compare(T x, T y)
            {
                if (Eq.Equals(x, y))
                    return 0;

                int hx = x?.GetHashCode() ?? 0;
                int hy = y?.GetHashCode() ?? 0;

                return hx.CompareTo(hy);
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                --n;
                var k = UnityEngine.Random.Range(0, list.Count);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
        {
            for(var i = 0; i < list.Count; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(list[i], value))
                    return i;
            }

            return -1;
        }

        public static void Fill<T>(this IList<T> list, T value, int count)
        {
            for(var i = 0; i < count; ++i)
                list.Add(value);
        }

#if UNITY_2017_1_OR_NEWER

        public static int PickRandomIndex<T>(this IReadOnlyList<T> list)
        {
            return UnityEngine.Random.Range(0, list.Count);
        }

        public static T PickRandom<T>(this IReadOnlyList<T> list)
        {
            return list[PickRandomIndex(list)];
        }

        public static T PopRandomUnordered<T>(this List<T> list)
        {
            var index = PickRandomIndex(list);
            return PopUnorderedAt(list, index);
        }

#endif

    }
}