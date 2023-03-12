using System.Collections.Generic;

namespace SZUtilities.Extensions
{
    public static class ListExtensions
    {
        public static bool RemoveUnordered<T>(this List<T> list, T value)
        {
            var index = list.IndexOf(value);
            if (index < 0)
                return false;
            
            RemoveUnorderedAt(list, index);
            return true;
        }

        public static void RemoveUnorderedAt<T>(this List<T> list, int index)
        {
            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
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
    }
}