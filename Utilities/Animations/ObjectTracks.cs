using System;
using UnityEngine;

namespace SZUtilities
{
    public static partial class Routines
    {
        public class PositionTrack : Vector3FieldTrack
        {
            public readonly Transform Transform;

            public PositionTrack(Transform transform, float timeTotal, Func<float, float> curve, Vector3 from, Vector3 to)
                : base(timeTotal, curve, from, to)
            {
                Transform = transform;
            }

            public PositionTrack(Transform transform, float timeTotal, Func<float, float> curve, Vector3 to)
                : this(transform, timeTotal, curve, transform.position, to)
            { }

            public override void SetFieldProgress(Vector3 value)
            {
                Transform.position = value;
            }
        }

    public class RectAnchorMinMaxTrack : TrackBase
    {
        public readonly RectTransform RectTransform;

        public readonly Vector2 MinFrom;
        public readonly Vector2 MinTo;
        public readonly Vector2 MaxFrom;
        public readonly Vector2 MaxTo;

        public RectAnchorMinMaxTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 minFrom, Vector2 minTo, Vector2 maxFrom, Vector2 maxTo)
            : base(timeTotal, curve)
        {
            RectTransform = transform;

            MinFrom = minFrom;
            MinTo = minTo;
            MaxFrom = maxFrom;
            MaxTo = maxTo;
        }

        public RectAnchorMinMaxTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 minTo, Vector2 maxTo)
            : this(transform, timeTotal, curve, transform.anchorMin, minTo, transform.anchorMax, maxTo)
        { }


        public override void SetProgress(float progress)
        {
            RectTransform.anchorMin = Vector2.LerpUnclamped(MinFrom, MinTo, progress);
            RectTransform.anchorMax = Vector2.LerpUnclamped(MaxFrom, MaxTo, progress);
        }
    }

    public class RectAnchorCenterTrack : RectAnchorMinMaxTrack
    {
        protected static Vector2 GetCurrentAnchorSize(RectTransform transform) => transform.anchorMax - transform.anchorMin;
        protected static Vector2 GetCurrentHalfAnchorSize(RectTransform transform) => GetCurrentAnchorSize(transform) / 2.0f;
        protected static Vector2 GetCurrentAnchorCenter(RectTransform transform) => transform.anchorMin + GetCurrentHalfAnchorSize(transform);

        public RectAnchorCenterTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 from, Vector2 to)
            : base(transform, timeTotal, curve, 
                  from - GetCurrentHalfAnchorSize(transform), to - GetCurrentHalfAnchorSize(transform),
                  from + GetCurrentHalfAnchorSize(transform), to + GetCurrentHalfAnchorSize(transform))
        { }

        public RectAnchorCenterTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 to)
            : this(transform, timeTotal, curve, GetCurrentAnchorCenter(transform), to)
        { }
    }

    public class RectSizeTrack : Vector2FieldTrack
    {
        public readonly RectTransform RectTransform;

        public RectSizeTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 from, Vector2 to)
            : base(timeTotal, curve, from, to)
        {
            RectTransform = transform;
        }
            public readonly RectTransform RectTransform;

            public RectSizeTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 from, Vector2 to)
                : base(timeTotal, curve, from, to)
            {
                RectTransform = transform;
            }

            public RectSizeTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 to)
                : this(transform, timeTotal, curve, transform.sizeDelta, to)
            { }

            public override void SetFieldProgress(Vector2 value)
            {
                RectTransform.sizeDelta = value;
            }
        }

        public class AnchoredPositionTrack : Vector2FieldTrack
        {
            public readonly RectTransform RectTransform;

            public AnchoredPositionTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 from, Vector2 to)
                : base(timeTotal, curve, from, to)
            {
                RectTransform = transform;
            }

            public AnchoredPositionTrack(RectTransform transform, float timeTotal, Func<float, float> curve, Vector2 to)
                : this(transform, timeTotal, curve, transform.anchoredPosition, to)
            { }

            public override void SetFieldProgress(Vector2 value)
            {
                RectTransform.anchoredPosition = value;
            }
        }

        public class RotationTrack : QuaternionFieldTrack
        {
            public readonly Transform Transform;

            public RotationTrack(Transform transform, float timeTotal, Func<float, float> curve, Quaternion from, Quaternion to)
                : base(timeTotal, curve, from, to)
            {
                Transform = transform;
            }

            public RotationTrack(Transform transform, float timeTotal, Func<float, float> curve, Quaternion to)
                : this(transform, timeTotal, curve, transform.rotation, to)
            { }

            public override void SetFieldProgress(Quaternion value)
            {
                Transform.rotation = value;
            }
        }

        public class LocalScaleTrack : Vector3FieldTrack
        {
            public readonly Transform Transform;

            public LocalScaleTrack(Transform transform, float timeTotal, Func<float, float> curve, Vector3 from, Vector3 to)
                : base(timeTotal, curve, from, to)
            {
                Transform = transform;
            }

            public LocalScaleTrack(Transform transform, float timeTotal, Func<float, float> curve, Vector3 to)
                : this(transform, timeTotal, curve, transform.localScale, to)
            { }

            public override void SetFieldProgress(Vector3 value)
            {
                Transform.localScale = value;
            }
        }
        
        public class CanvasGroupAlphaTrack : FloatFieldTrack
        {
            public readonly CanvasGroup CanvasGroup;

            public CanvasGroupAlphaTrack(CanvasGroup canvasGroup, float timeTotal, Func<float, float> curve, float from, float to)
                : base(timeTotal, curve, from, to)
            {
                CanvasGroup = canvasGroup;
            }

            public CanvasGroupAlphaTrack(CanvasGroup canvasGroup, float timeTotal, Func<float, float> curve, float to)
                : this(canvasGroup, timeTotal, curve, canvasGroup.alpha, to)
            { }

            public override void SetFieldProgress(float value)
            {
                CanvasGroup.alpha = value;
            }
        }
    }
}