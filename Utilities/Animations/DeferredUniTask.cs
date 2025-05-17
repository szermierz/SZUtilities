#if SZUTILITIES_USE_UNITASK && !SZUTILITIES_LEGACY_ROUTINES

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace SZUtilities.Animations
{
    public struct DeferredUniTask
        : IDisposable
    {
        public static explicit operator UniTask(DeferredUniTask deferredUniTask) => deferredUniTask.Invoke();

        private UniTaskCallerBase m_caller;
        private DisposablesGroup m_disposables;

        public void Dispose()
        {
            m_disposables?.Dispose();
            m_disposables = null;
            m_caller = null;
        }

        #region Callers

        private abstract class UniTaskCallerBase
        {
            public abstract UniTask Invoke();
        }

        private sealed class UniTaskCaller
            : UniTaskCallerBase
        {
            public Func<UniTask> Func;

            public override UniTask Invoke()
            {
                return Func.Invoke();
            }
        }

        private sealed class UniTaskCaller<TArg1>
            : UniTaskCallerBase
        {
            public Func<TArg1, UniTask> Func;
            public TArg1 Arg1;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3, TArg4>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, TArg4, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;
            public TArg4 Arg4;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3, Arg4);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, TArg4, TArg5, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;
            public TArg4 Arg4;
            public TArg5 Arg5;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;
            public TArg4 Arg4;
            public TArg5 Arg5;
            public TArg6 Arg6;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;
            public TArg4 Arg4;
            public TArg5 Arg5;
            public TArg6 Arg6;
            public TArg7 Arg7;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);
            }
        }

        private sealed class UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
            : UniTaskCallerBase
        {
            public Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, UniTask> Func;
            public TArg1 Arg1;
            public TArg2 Arg2;
            public TArg3 Arg3;
            public TArg4 Arg4;
            public TArg5 Arg5;
            public TArg6 Arg6;
            public TArg7 Arg7;
            public TArg8 Arg8;

            public override UniTask Invoke()
            {
                return Func.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);
            }
        }

        #endregion

        #region Creation

        public static DeferredUniTask Create(Func<UniTask> func)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller>.Rent(out var caller));
            caller.Func = func;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1>
            (Func<TArg1, UniTask> func, 
            TArg1 arg1)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2>
            (Func<TArg1, TArg2, UniTask> func, 
            TArg1 arg1, TArg2 arg2)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3>
            (Func<TArg1, TArg2, TArg3, UniTask> func, 
            TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3, TArg4>
            (Func<TArg1, TArg2, TArg3, TArg4, UniTask> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3, TArg4>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;
            caller.Arg4 = arg4;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3, TArg4, TArg5>
            (Func<TArg1, TArg2, TArg3, TArg4, TArg5, UniTask> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;
            caller.Arg4 = arg4;
            caller.Arg5 = arg5;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
            (Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, UniTask> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;
            caller.Arg4 = arg4;
            caller.Arg5 = arg5;
            caller.Arg6 = arg6;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
            (Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, UniTask> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;
            caller.Arg4 = arg4;
            caller.Arg5 = arg5;
            caller.Arg6 = arg6;
            caller.Arg7 = arg7;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        public static DeferredUniTask Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
            (Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, UniTask> func,
            TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            var disposables = DisposablesGroup.Rent();
            disposables.Add(RentingPool<UniTaskCaller<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>>
                .Rent(out var caller));
            caller.Func = func;
            caller.Arg1 = arg1;
            caller.Arg2 = arg2;
            caller.Arg3 = arg3;
            caller.Arg4 = arg4;
            caller.Arg5 = arg5;
            caller.Arg6 = arg6;
            caller.Arg7 = arg7;
            caller.Arg8 = arg8;

            return new DeferredUniTask
            {
                m_disposables = disposables,
                m_caller = caller
            };
        }

        #endregion

        #region Invocation

        public async UniTask Invoke()
        {
            try
            {
                if(null != m_caller)
                    await m_caller.Invoke();
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region Parallel

        private class DeferredParallerRoutineRun
        {
            private readonly List<DeferredUniTask> m_tasks = new();
            private int m_toRun;

            public void Clear()
            {
                m_tasks.Clear();
            }

            public void Add(DeferredUniTask task)
            {
                m_tasks.Add(task);
            }

            private async UniTaskVoid RunTask(DeferredUniTask task)
            {
                await task.Invoke();
                --m_toRun;
            }

            public async UniTask Await(IDisposable handle)
            {
                try
                {
                    m_toRun = m_tasks.Count;
                    foreach (var task in m_tasks)
                        RunTask(task).Forget();

                    while (m_toRun > 0)
                        await UniTask.Yield();
                }
                finally
                {
                    m_tasks.Clear();
                    handle.Dispose();
                }
            }
        }

        public static DeferredUniTask Parallel(IReadOnlyList<DeferredUniTask> tasks)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            foreach (var task in tasks)
                run.Add(task);
            return Create(s_parallel, run, handle);
        }


        public static DeferredUniTask Parallel(DeferredUniTask u1, DeferredUniTask u2)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3, DeferredUniTask u4)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3, DeferredUniTask u4,
            DeferredUniTask u5)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3, DeferredUniTask u4,
            DeferredUniTask u5, DeferredUniTask u6)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3, DeferredUniTask u4,
            DeferredUniTask u5, DeferredUniTask u6, DeferredUniTask u7)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            run.Add(u7);
            return Create(s_parallel, run, handle);
        }

        public static DeferredUniTask Parallel(
            DeferredUniTask u1, DeferredUniTask u2, DeferredUniTask u3, DeferredUniTask u4, 
            DeferredUniTask u5, DeferredUniTask u6, DeferredUniTask u7, DeferredUniTask u8)
        {
            var handle = RentingPool<DeferredParallerRoutineRun>.Rent(out var run);
            run.Clear();
            run.Add(u1);
            run.Add(u2);
            run.Add(u3);
            run.Add(u4);
            run.Add(u5);
            run.Add(u6);
            run.Add(u7);
            run.Add(u8);
            return Create(s_parallel, run, handle);
        }

        private static readonly Func<DeferredParallerRoutineRun, IDisposable, UniTask> s_parallel =
            (run, handle) => run.Await(handle);

        #endregion

        #region Concat

        public static DeferredUniTask Concat(DeferredUniTask u1, DeferredUniTask u2)
        {
            return u1.Concat(u2);
        }

        public readonly DeferredUniTask Concat(DeferredUniTask task)
        {
            return Create(s_concat, this, task);
        }

        private static readonly Func<DeferredUniTask, DeferredUniTask, UniTask> s_concat =
            (u1, u2) => ConcatTask(u1, u2);

        private static async UniTask ConcatTask(DeferredUniTask u1, DeferredUniTask u2)
        {
            await u1.Invoke();
            await u2.Invoke();
        }

        #endregion

        #region Action

        public static DeferredUniTask Action(Action action)
        {
            return Create(s_action, action);
        }

        private static readonly Func<Action, UniTask> s_action =
            action => ActionTask(action);
        private static UniTask ActionTask(Action action)
        {
            action.Invoke();
            return default;
        }

        #endregion
    }
}

#endif