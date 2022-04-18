using System;
using System.Collections.Generic;
using UnityEngine;

namespace SZUtilities.Extensions
{
    public static class EnumerableMethods
    {
        public static TSource Closest<TSource>(this IEnumerable<TSource> sequence, int pivot, Func<TSource, int> accessor)
        {
            return sequence.Closest(pivot, _source => accessor(_source));
        }

        public static TSource Closest<TSource>(this IEnumerable<TSource> sequence, float pivot, Func<TSource, float> accessor)
        {
            var it = sequence.GetEnumerator();

            TSource closest = default;
            float? closestValue = null;

            while (it.MoveNext())
            {
                var value = accessor(it.Current);
                var diff = Mathf.Abs(value - pivot);

                if (!closestValue.HasValue || diff < closestValue.Value)
                {
                    closest = it.Current;
                    closestValue = diff;
                }
            }

            return closest;
        }
    }
}