using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SZUtilities
{
    public static partial class Routines
    {
        public static Func<float, float> Lerp => (v) => v;
        public static Func<float, float> Square => (v) => v * (2.0f - 1.0f * v);
        public static Func<float, float> RisingSquare => (v) => v * v;
        public static Func<float, float> DoubleSquared => (v) => Square(Square(v));
        public static Func<float, float> DoubleRisingSquare => (v) => RisingSquare(RisingSquare(v));

        public static Func<float, float> BounceCurve => (v) =>
        {
            if (v < 0.5f)
                return 2.0f * v;
            else
                return 3.0f + v * (-6.0f + 4.0f * v);
        };

        public static IEnumerator Animate(IEnumerable<TrackBase> tracks)
        {
            for (float time = 0.0f; tracks.Any(_track => !_track.Completed); time += Time.deltaTime)
            {
                foreach (var track in tracks)
                    track.Update(time);

                yield return null;
            }
        }

        public static IEnumerator Animate(params TrackBase[] tracks)
        {
            return Animate((IEnumerable<TrackBase>)tracks);
        }

        public abstract class TrackBase
        {
            public readonly float TimeTotal;
            public readonly Func<float, float> Curve;

            public bool Completed { get; private set; } = false;

            public bool Update(float time)
            {
                if (Completed)
                    return false;

                var progress = Mathf.Min(1.0f, time / TimeTotal);
                progress = Mathf.Clamp01(Curve(progress));

                SetProgress(progress);

                if (1.0f == progress)
                    Completed = true;

                return !Completed;
            }

            public abstract void SetProgress(float progress);

            public TrackBase(float timeTotal, Func<float, float> curve)
            {
                TimeTotal = timeTotal;
                Curve = curve;
            }
        }

        public class Track : TrackBase
        {
            public readonly Action<float> ProgressSetter;

            public Track(float timeTotal, Func<float, float> curve, Action<float> progressSetter)
                : base(timeTotal, curve)
            {
                ProgressSetter = progressSetter;
            }

            public override void SetProgress(float progress)
            {
                ProgressSetter(progress);
            }
        }
    }
}