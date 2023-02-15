using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace SZUtilities
{
    public static partial class Routines
    {
        /*
        This is a routine wrapper which (in opposite to unity internal routines runner)
        disposes enumerators in case of exception
        */
        [DebuggerHidden]
        public static IEnumerator DisposingRoutine(IEnumerator logic)
        {
            if(null == logic)
            {
                yield return null;
                yield break;
            }

            using var _ = ListRenting.Rent(out List<IEnumerator> stack);
            try
            {
                stack.Add(logic);
                while (stack.Any())
                {
                    var last = stack[^1];
                    if(last.MoveNext())
                    {
                        var enumerator = last.Current switch
                        {
                            IEnumerable currentEnumerable => currentEnumerable.GetEnumerator(),
                            IEnumerator currentEnumerator => currentEnumerator,
                            _ => null,
                        };

                        if (null == enumerator)
                            yield return last.Current;
                        else
                            stack.Add(enumerator);
                    }
                    else
                    {
                        if (last is IDisposable disposable)
                            disposable.Dispose();

                        stack.RemoveAt(stack.Count - 1);
                    }
                }
            }
            finally
            {
                for (var i = stack.Count - 1; i >= 0; --i)
                {
                    if (stack[i] is IDisposable disposable)
                        disposable.Dispose();
                }
                stack.Clear();
            }
        }

        public static IEnumerator Concat(params IEnumerator[] enumerators)
        {
            foreach (var enumerator in enumerators)
            {
                if (null == enumerator)
                    continue;

                yield return enumerator;
            }
        }

        public static IEnumerator Action(Action action)
        {
            action();
            yield break;
        }

        public static IEnumerator Parallel(MonoBehaviour routinesHolder, params IEnumerator[] routines)
            => Parallel(routinesHolder, (IEnumerable<IEnumerator>)routines);

        public static IEnumerator Parallel(MonoBehaviour routinesHolder, IEnumerable<IEnumerator> routines)
        {
            int waitingCounter = 0;

            foreach (var routine in routines)
            {
                if (null == routine)
                    continue;

                ++waitingCounter;

                routinesHolder.StartCoroutine(
                    Routines.Concat(
                        routine,
                        Routines.Action(() => --waitingCounter)
                        )
                    );
            }

            while (waitingCounter > 0)
                yield return null;
        }

        public class Animation
        {
            private List<IEnumerator> m_routines = new List<IEnumerator>(8);

            public Animation()
            { }

            public Animation(params IEnumerator[] routines)
            {
                foreach (var routine in routines)
                    Add(routine);
            }

            public Animation Add(IEnumerator routine)
            {
                if (null != routine)
                    m_routines.Add(routine);

                return this;
            }

            public IEnumerator Invoke()
            {
                foreach (var routine in m_routines)
                {
                    if (null == routine)
                        continue;

                    yield return routine;
                }
            }
        }
        
        public static IEnumerator Move(Transform target, Vector3 endPos, Quaternion endRot, Vector3 endScale, float time, Func<float, float> curve)
        {
            return Move(target, target.position, endPos, target.rotation, endRot, target.localPosition, endScale, time, curve);
        }

        public static IEnumerator Move(Transform target, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, Vector3 startScale, Vector3 endScale, float time,
            Func<float, float> curve)
        {
            return Animate(
                new PositionTrack(target, time, curve, startPos, endPos),
                new RotationTrack(target, time, curve, startRot, endRot),
                new LocalScaleTrack(target, time, curve, startScale, endScale)
                );
        }
    }

    public class RoutineTask
    {
        public bool Done { get; private set; } = true;

        public RoutineTask()
            : this(null)
        { }

        public RoutineTask(IEnumerator routine)
        {
            Attach(routine);
        }

        public IEnumerator Attach(IEnumerator routine)
        {
            if (null == routine)
                return null;

            Done = false;

            return Routines.Concat(routine, Routines.Action(MakeDone));
        }

        private void MakeDone()
        {
            Done = true;
        }
    }
}
