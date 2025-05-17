#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SZUtilities.Animations
{
    public static partial class Routines
    {
        public delegate float Curve(float progress);

        public static Curve Lerp => (v) => v;
        public static Curve Square => (v) => v * (2.0f - 1.0f * v);
        public static Curve RisingSquare => (v) => v * v;
        public static Curve DoubleSquare => (v) => Square(Square(v));
        public static Curve DoubleRisingSquare => (v) => RisingSquare(RisingSquare(v));
        public static Curve From0Through1To0Parabolic => (v) => 4.0f * v * (1.0f - v);

        public static Curve BounceCurve => (v) =>
        {
            if (v < 0.5f)
                return 2.0f * v;
            else
                return 3.0f + v * (-6.0f + 4.0f * v);
        };

        public static AnimationBuilder Animate(Transform target, float totalTime)
        {
            return new AnimationBuilder(target, totalTime);
        }

        public static UniTask Move(Transform target, Transform marker, float totalTime, Curve curve, 
            CancellationToken cancellationToken = default, ReuseableCancellationToken reuseableCancellationToken = default)
        {
            return Animate(target, totalTime)
                .Position(marker.position, curve)
                .Rotation(marker.rotation, curve)
                .LocalScale(marker.localScale, curve)
                .AwaitAnimation(cancellationToken, reuseableCancellationToken);
        }

        public static DeferredUniTask MoveDeferred(Transform target, Transform marker, float totalTime, Curve curve,
            CancellationToken cancellationToken = default, ReuseableCancellationToken reuseableCancellationToken = default)
        {
            return Animate(target, totalTime)
                .Position(marker.position, curve)
                .Rotation(marker.rotation, curve)
                .LocalScale(marker.localScale, curve)
                .DeferAnimation(cancellationToken, reuseableCancellationToken);
        }
    }
}

#endif