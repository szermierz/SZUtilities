#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SZUtilities.Animations
{
    public struct AnimationBuilder
        : IDisposable
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private static bool s_buildingInProgress = false;
        private bool m_needsToReleaseBuilding;
#endif

        private readonly Transform m_target;
        private readonly float m_totalTime;

        private DisposablesGroup m_tracksGroup;
        private List<Track> m_tracks;

        internal AnimationBuilder(Transform target, float totalTime, DisposablesGroup disposables = null, List<Track> tracks = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (s_buildingInProgress)
                throw new Exception($"Memory leaks prevention: Did not finished previous building and started another one. Call {nameof(AwaitAnimation)} on previous builder");

            s_buildingInProgress = true;
            m_needsToReleaseBuilding = true;
#endif

            m_target = target;
            m_totalTime = totalTime;

            m_tracksGroup = disposables ?? DisposablesGroup.Rent();

            if (null != tracks)
                m_tracks = tracks;
            else
                m_tracksGroup.Add(ListRenting.Rent(out m_tracks));
        }

        private readonly AnimationBuilder Add(IDisposable handle, Track track)
        {
            m_tracksGroup.Add(handle);
            m_tracks.Add(track);
            return this;
        }

        public UniTask AwaitAnimation()
        {
            return AwaitAnimation(default, default);
        }

        public UniTask AwaitAnimation(ReuseableCancellationToken cancellationToken)
        {
            return AwaitAnimation(default, cancellationToken);
        }

        public UniTask AwaitAnimation(CancellationToken cancellationToken)
        {
            return AwaitAnimation(cancellationToken, default);
        }

        public async UniTask AwaitAnimation(CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (m_needsToReleaseBuilding)
            {
                s_buildingInProgress = false;
                m_needsToReleaseBuilding = false;
            }
#endif

            try
            {
                for (var time = 0.0f; time < m_totalTime; time += Time.deltaTime)
                {
                    var progress = Mathf.Clamp01(time / m_totalTime);
                    foreach(var track in m_tracks)
                        track.Update(progress, cancellationToken, reuseableCancellationToken);

                    await UniTask.Yield();

                    if (cancellationToken.IsCancellationRequested 
                        || reuseableCancellationToken.IsCancellationRequested)
                        return;
                }

                foreach (var track in m_tracks)
                    track.Update(1.0f, cancellationToken, reuseableCancellationToken);
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (m_needsToReleaseBuilding)
                s_buildingInProgress = false;
            m_needsToReleaseBuilding = false;
#endif
            m_tracksGroup?.Dispose();
            m_tracksGroup = null;
            m_tracks = null;
        }

        public DeferredUniTask DeferAnimation()
        {
            return DeferAnimation(default, default);
        }

        public DeferredUniTask DeferAnimation(CancellationToken cancellationToken)
        {
            return DeferAnimation(cancellationToken, default);
        }

        public DeferredUniTask DeferAnimation(ReuseableCancellationToken reuseableCancellationToken)
        {
            return DeferAnimation(default, reuseableCancellationToken);
        }

        private static readonly Func<AnimationBuilder, CancellationToken, ReuseableCancellationToken, UniTask> s_staticAwaitAnimation 
            = (builder, cancelToken, reuseToken) => builder.AwaitAnimation(cancelToken, reuseToken);
        public DeferredUniTask DeferAnimation(CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (m_needsToReleaseBuilding)
            {
                s_buildingInProgress = false;
                m_needsToReleaseBuilding = false;
            }
#endif

            return DeferredUniTask.Create(s_staticAwaitAnimation, this, cancellationToken, reuseableCancellationToken);
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
