using System;
using UnityEngine;

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

    public class RectSizeTrack : Vector2FieldTrack
    {
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
}
