using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Utilities.VectorExtensions;

namespace SerializationExtensions
{
    public static class VectorExcentionsImpl
    {
        public static Vector2 Deserialize_Vector2(this string value)
        {
            var values = value.Trim('(', ')').Split('|');
            if (values.Length != 2)
                throw new ArgumentException($"Vector2 has invalid format {value}! Expected \"(x|y)\"");

            if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float x))
                throw new ArgumentException($"Couldn't parse x of {value}!");

            if (!float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float y))
                throw new ArgumentException($"Couldn't parse y of {value}!");

            return new Vector2(x, y);
        }

        public static string Serialize(this Vector2 value)
        {
            return $"({value.x}|{value.y})";
        }
    }
}

namespace GizmoUtilities
{
    public static class GizmoEx
    {
        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction.IsZero())
                return;

            var prevColor = Gizmos.color;

            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);

            Gizmos.color = prevColor;
        }
    }
}

namespace LinqExtenstions
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

namespace ArrayExtensions
{
    public static class ArrayMethods
    {
        public static List<T> SortedArraysDiff<T>(IList<T> lhs, IList<T> rhs, List<T> cacheResult = null)
        {
            if (null == cacheResult)
                cacheResult = new List<T>();

            int i = 0, j = 0;
            int n = lhs.Count;
            int m = rhs.Count;

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