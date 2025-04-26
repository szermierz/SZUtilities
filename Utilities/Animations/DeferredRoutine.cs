#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace SZUtilities.Animations
{
    public struct DeferredRoutine
    {
        AnimationBuilder m_builder;
        Func<AnimationBuilder, CancellationToken, ReuseableCancellationToken, UniTask> m_functor;

        CancellationToken m_cancellationToken;
        ReuseableCancellationToken m_reuseableCancellationToken;

        private static UniTask InvokeParallel(DeferredRoutine u1, DeferredRoutine u2, )
        {
            return Routines.Parallel(u1.Invoke(), u2.Invoke());
        }

        internal static DeferredRoutine CreateParallel(DeferredRoutine u1, DeferredRoutine u2,
            CancellationToken cancellationToken,
            ReuseableCancellationToken reuseableCancellationToken)
        {

        }

        internal static DeferredRoutine Create(
            AnimationBuilder builder, 
            Func<AnimationBuilder, CancellationToken, ReuseableCancellationToken, UniTask> functor,
            CancellationToken cancellationToken,
            ReuseableCancellationToken reuseableCancellationToken)
        {
            return new DeferredRoutine()
            {
                m_builder = builder,
                m_functor = functor,
                m_cancellationToken = cancellationToken,
                m_reuseableCancellationToken = reuseableCancellationToken,
            };
        }

        public readonly UniTask Invoke()
        {
            return m_functor.Invoke(m_builder, m_cancellationToken, m_reuseableCancellationToken);
        }
    }
}

#endif
