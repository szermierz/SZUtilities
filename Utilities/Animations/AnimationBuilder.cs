#if SZUTILITIES_USE_UNITASK

using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SZUtilities.Animations
{
    public struct AnimationBuilder
    {
        private readonly Transform m_target;
        private readonly float m_totalTime;

        private DisposablesGroup m_tracksGroup;
        private List<Track> m_tracks;

        internal AnimationBuilder(Transform target, float totalTime)
        {
            m_target = target;
            m_totalTime = totalTime;
            m_tracksGroup = DisposablesGroup.Rent();
            m_tracksGroup.Add(ListRenting.Rent(out m_tracks));
        }

        private readonly AnimationBuilder Add(IDisposable handle, Track track)
        {
            m_tracksGroup.Add(handle);
            m_tracks.Add(track);
            return this;
        }

        public async UniTask AwaitAnimation()
        {
            try
            {
                for(var time = 0.0f; time < m_totalTime; time += Time.deltaTime)
                {
                    var progress = Mathf.Clamp01(time / m_totalTime);
                    foreach(var track in m_tracks)
                        track.Update(progress);

                    await UniTask.Yield();
                }

                foreach (var track in m_tracks)
                    track.Update(1.0f);
            }
            finally
            {
                m_tracksGroup.Dispose();
                m_tracksGroup = null;
                m_tracks = null;
            }
        }

        public readonly AnimationBuilder LocalPosition(Vector3 to, Routines.Curve curve)
        {
            return LocalPosition(m_target.localPosition, to, curve);
        }

        public readonly AnimationBuilder LocalPosition(Vector3 from, Vector3 to, Routines.Curve curve)
        {
            var handle = RentingPool<LocalPositionTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to);
            return Add(handle, track);
        }

        public readonly AnimationBuilder Position(Vector3 to, Routines.Curve curve)
        {
            return Position(m_target.localPosition, to, curve);
        }

        public readonly AnimationBuilder Position(Vector3 from, Vector3 to, Routines.Curve curve)
        {
            var handle = RentingPool<PositionTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to);
            return Add(handle, track);
        }

        public readonly AnimationBuilder PositionWithParabolicOffset(Vector3 to, Routines.Curve curve, Vector3 parabolicOffset)
        {
            return PositionWithParabolicOffset(m_target.position, to, curve, parabolicOffset);
        }

        public readonly AnimationBuilder PositionWithParabolicOffset(Vector3 from, Vector3 to, Routines.Curve curve, Vector3 parabolicOffset)
        {
            var handle = RentingPool<PositionWithParabolicOffsetTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to, parabolicOffset);
            return Add(handle, track);
        }

        public readonly AnimationBuilder LocalRotation(Quaternion to, Routines.Curve curve)
        {
            return LocalRotation(m_target.localRotation, to, curve);
        }

        public readonly AnimationBuilder LocalRotation(Quaternion from, Quaternion to, Routines.Curve curve)
        {
            var handle = RentingPool<LocalRotationTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to);
            return Add(handle, track);
        }

        public readonly AnimationBuilder Rotation(Quaternion to, Routines.Curve curve)
        {
            return Rotation(m_target.rotation, to, curve);
        }

        public readonly AnimationBuilder Rotation(Quaternion from, Quaternion to, Routines.Curve curve)
        {
            var handle = RentingPool<RotationTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to);
            return Add(handle, track);
        }

        public readonly AnimationBuilder LocalScale(Vector3 to, Routines.Curve curve)
        {
            return LocalScale(m_target.localScale, to, curve);
        }

        public readonly AnimationBuilder LocalScale(Vector3 from, Vector3 to, Routines.Curve curve)
        {
            var handle = RentingPool<LocalScaleTrack>.Rent(out var track);
            track.Setup(m_target, curve, from, to);
            return Add(handle, track);
        }
    }
}

#endif
