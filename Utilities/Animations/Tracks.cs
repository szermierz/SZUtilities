#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using System;
using System.Threading;
using UnityEngine;

namespace SZUtilities.Animations
{
    public abstract class Track
        : IDisposable
    {
        protected Transform m_target;
        protected Routines.Curve m_curve;

        protected void Setup(Transform target, Routines.Curve curve)
        {
            m_target = target;
            m_curve = curve;
        }

        public virtual void Dispose()
        {
            m_target = null;
            m_curve = null;
        }

        public virtual void Update(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            progress = m_curve(progress);
            SetProgress(progress, cancellationToken, reuseableCancellationToken);
        }

        protected abstract void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken);
    }

    public class LocalPositionTrack 
        : Track
    {
        protected Vector3 m_from;
        protected Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            var pos = Vector3.Lerp(m_from, m_to, progress);
            m_target.localPosition = pos;
        }
    }

    public class PositionTrack
        : Track
    {
        protected Vector3 m_from;
        protected Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            var pos = Vector3.Lerp(m_from, m_to, progress);
            m_target.position = pos;
        }
    }

    public class LocalRotationTrack
        : Track
    {
        protected Quaternion m_from;
        protected Quaternion m_to;

        public void Setup(Transform target, Routines.Curve curve, Quaternion from, Quaternion to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            var rot = Quaternion.Lerp(m_from, m_to, progress);
            m_target.localRotation = rot;
        }
    }

    public class PositionWithParabolicOffsetTrack : PositionTrack
    {
        protected Vector3 m_parabolicOffset;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to, Vector3 parabolicOffset)
        {
            Setup(target, curve, from, to);
            m_parabolicOffset = parabolicOffset;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            base.SetProgress(progress, cancellationToken, reuseableCancellationToken);

            var parabolicOffset = m_parabolicOffset * Routines.From0Through1To0Parabolic(progress);
            m_target.position += parabolicOffset;
        }
    }

    public class RotationTrack
        : Track
    {
        protected Quaternion m_from;
        protected Quaternion m_to;

        public void Setup(Transform target, Routines.Curve curve, Quaternion from, Quaternion to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            var rot = Quaternion.Lerp(m_from, m_to, progress);
            m_target.rotation = rot;
        }
    }

    public class LocalScaleTrack
        : Track
    {
        protected Vector3 m_from;
        protected Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            var scale = Vector3.Lerp(m_from, m_to, progress);
            m_target.localScale = scale;
        }
    }
}

#endif

