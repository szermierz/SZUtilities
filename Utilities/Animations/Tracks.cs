#if SZUTILITIES_USE_UNITASK

using UnityEngine;

namespace SZUtilities.Animations
{
    public abstract class Track
    {
        protected Transform m_target;
        protected Routines.Curve m_curve;

        protected void Setup(Transform target, Routines.Curve curve)
        {
            m_target = target;
            m_curve = curve;
        }

        public virtual void Update(float progress)
        {
            progress = m_curve(progress);
            SetProgress(progress);
        }

        protected abstract void SetProgress(float progress);
    }

    public class LocalPositionTrack 
        : Track
    {
        private Vector3 m_from;
        private Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress)
        {
            var pos = Vector3.Lerp(m_from, m_to, progress);
            m_target.localPosition = pos;
        }
    }

    public class PositionTrack
        : Track
    {
        private Vector3 m_from;
        private Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress)
        {
            var pos = Vector3.Lerp(m_from, m_to, progress);
            m_target.position = pos;
        }
    }

    public class LocalRotationTrack
        : Track
    {
        private Quaternion m_from;
        private Quaternion m_to;

        public void Setup(Transform target, Routines.Curve curve, Quaternion from, Quaternion to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress)
        {
            var rot = Quaternion.Lerp(m_from, m_to, progress);
            m_target.localRotation = rot;
        }
    }

    public class RotationTrack
        : Track
    {
        private Quaternion m_from;
        private Quaternion m_to;

        public void Setup(Transform target, Routines.Curve curve, Quaternion from, Quaternion to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress)
        {
            var rot = Quaternion.Lerp(m_from, m_to, progress);
            m_target.rotation = rot;
        }
    }

    public class LocalScaleTrack
        : Track
    {
        private Vector3 m_from;
        private Vector3 m_to;

        public void Setup(Transform target, Routines.Curve curve, Vector3 from, Vector3 to)
        {
            Setup(target, curve);
            m_from = from;
            m_to = to;
        }

        protected override void SetProgress(float progress)
        {
            var scale = Vector3.Lerp(m_from, m_to, progress);
            m_target.localScale = scale;
        }
    }
}

#endif

