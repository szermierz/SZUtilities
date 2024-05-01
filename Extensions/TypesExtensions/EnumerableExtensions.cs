using System;
using System.Collections.Generic;
using UnityEngine;

namespace SZUtilities.Extensions
{
    public static class EnumerableMethods
    {
        public static TSource Closest<TSource>(this IEnumerable<TSource> sequence, int pivot, Func<TSource, int> accessor)
        {
            return sequence.Closest((float)pivot, _source => (float)accessor(_source));
        }

        public static int ClosestIndex<TSource>(this IEnumerable<TSource> sequence, int pivot, Func<TSource, int> accessor)
        {
            return sequence.ClosestIndex((float)pivot, _source => (float)accessor(_source));
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

        public static int ClosestIndex<TSource>(this IEnumerable<TSource> sequence, float pivot, Func<TSource, float> accessor)
        {
            var index = -1;
            var it = sequence.GetEnumerator();

            int closestIndex = -1;
            TSource closest = default;
            float? closestValue = null;

            while (it.MoveNext())
            {
                ++index;
                var value = accessor(it.Current);
                var diff = Mathf.Abs(value - pivot);

                if (!closestValue.HasValue || diff < closestValue.Value)
                {
                    closestIndex = index;
                    closest = it.Current;
                    closestValue = diff;
                }
            }

            return closestIndex;
        }
    }
}