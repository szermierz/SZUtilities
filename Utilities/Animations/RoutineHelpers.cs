#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace SZUtilities.Animations
{
    public static partial class Routines
    {
        #region Parallel

        private class ParallerRoutineRun
        {
            private readonly List<UniTask> m_tasks = new();
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
                try
                {
                    await task;
                }
                catch(System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }

                --m_toRun;
            }

            public async UniTask Await()
            {
                m_toRun = m_tasks.Count;
                foreach(var task in m_tasks)
                    RunTask(task).Forget();

                while (m_toRun > 0)
                    await UniTask.Yield();
            }
        }

        public static async UniTask Parallel(IReadOnlyList<UniTask> uniTasks)
        {
            using var handle = RentingPool<ParallerRoutineRun>.Rent(out var run);
            run.Clear();
            foreach (var task in uniTasks)
                run.Add(task);
            await run.Await();
            run.Clear();
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

        #region CancelTokens

        public static async UniTask WaitForSeconds(float delay, ReuseableCancellationToken cancelToken = default)
        {
            if (cancelToken.IsCancellationRequested)
                return;

            for (var t = 0.0f; t < delay; t += UnityEngine.Time.deltaTime)
            {
                await UniTask.Yield();

                if (cancelToken.IsCancellationRequested)
                    return;
            }
        }

        #endregion
    }
}

#endif
