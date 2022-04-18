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
    }
}