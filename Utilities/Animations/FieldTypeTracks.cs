using System;
using UnityEngine;

namespace SZUtilities
{
    public static partial class Routines
    {
        public abstract class FieldTrack<FieldType> : TrackBase
        {
            public readonly FieldType ValueFrom;
            public readonly FieldType ValueTo;

            public delegate FieldType Interpolate(FieldType from, FieldType to, float x);
            public readonly Interpolate Interpolator;

            public FieldTrack(float timeTotal, Func<float, float> curve, Interpolate interpolator, FieldType from, FieldType to)
                : base(timeTotal, curve)
            {
                Interpolator = interpolator;
                ValueFrom = from;
                ValueTo = to;
            }

            public override void SetProgress(float progress) => SetFieldProgress(Interpolator(ValueFrom, ValueTo, progress));

            public abstract void SetFieldProgress(FieldType value);
        }

        public abstract class FloatFieldTrack : FieldTrack<float>
        {
            public FloatFieldTrack(float timeTotal, Func<float, float> curve, float from, float to)
                : base(timeTotal, curve, FloatInterpolator, from, to)
            { }

            public static float FloatInterpolator(float from, float to, float x)
            {
                return from + (to - from) * x;
            }
        }

        public abstract class Vector2FieldTrack : FieldTrack<Vector2>
        {
            public Vector2FieldTrack(float timeTotal, Func<float, float> curve, Vector2 from, Vector2 to)
                : base(timeTotal, curve, Vector2Interpolator, from, to)
            { }

            public static Vector2 Vector2Interpolator(Vector2 from, Vector2 to, float x)
            {
                return from + (to - from) * x;
            }
        }

        public abstract class Vector3FieldTrack : FieldTrack<Vector3>
        {
            public Vector3FieldTrack(float timeTotal, Func<float, float> curve, Vector3 from, Vector3 to)
                : base(timeTotal, curve, Vector3Interpolator, from, to)
            { }

            public static Vector3 Vector3Interpolator(Vector3 from, Vector3 to, float x)
            {
                return from + (to - from) * x;
            }
        }

        public abstract class QuaternionFieldTrack : FieldTrack<Quaternion>
        {
            public QuaternionFieldTrack(float timeTotal, Func<float, float> curve, Quaternion from, Quaternion to)
                : base(timeTotal, curve, Vector3Interpolator, from, to)
            { }

            public static Quaternion Vector3Interpolator(Quaternion from, Quaternion to, float x)
            {
                return Quaternion.Lerp(from, to, x);
            }
        }
    }
}
