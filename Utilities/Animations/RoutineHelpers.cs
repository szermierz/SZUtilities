#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SZUtilities.Animations
{
    public static partial class Routines
    {
        #region Parallel

        private class ParallerRoutineRun
        {
            private List<UniTask> m_tasks = new();
            private int m_toRun;

            public void Clear()
            {
                m_tasks.Clear();
            }

            public void Add(UniTask task)
            {
                m_tasks.Add(task);
            }

            private async UniTaskVoid RunTask(UniTask task)
            {
                await task;
                --m_toRun;
            }

            public async UniTask Await()
            {
                m_toRun = m_tasks.Count;
                foreach(var task in m_tasks)
                    RunTask(task).Forget();

                while (m_toRun >= 0)
                    await UniTask.Yield();
            }
        }

        private class ParallelContext2
        {
            public BoundFunctor<StructContainer<UniTask>> U1;
            public BoundFunctor<StructContainer<UniTask>> U2;
        }

        public static DeferredRoutine DeferredParallel(
            DeferredRoutine u1,
            DeferredRoutine u2)
        {
            var (functor, context) = DeferredRoutine.Create(DeferredParallel);
            context.U1 = u1;
            context.U2 = u2;
            return functor;
        }

        private static DeferredRoutine DeferredParallel(AnimationBuilder builder, CancellationToken cancellationToken, ReuseableCancellationToken reuseableCancellationToken)
        {
            DeferredRoutine.Create()
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3, UniTask u4)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3, UniTask u4, UniTask u5)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3, UniTask u4, UniTask u5, UniTask u6)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3, UniTask u4, UniTask u5, UniTask u6, UniTask u7)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            run.Add(u7);
            await run.Await();
            run.Clear();
        }

        public static async UniTask Parallel(UniTask u1, UniTask u2, UniTask u3, UniTask u4, UniTask u5, UniTask u6, UniTask u7, UniTask u8)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            run.Add(u7);
            run.Add(u8);
            await run.Await();
            run.Clear();
        }

        #endregion
    }
}

#endif
