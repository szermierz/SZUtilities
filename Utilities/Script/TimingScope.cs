using System;
using System.Collections.Generic;
using SZUtilities.Extensions;

namespace SZUtilities
{
    public static class TimingScope
    {
        internal static DebugEx Debug = new DebugEx("SZUtilities.Profiling");

        public interface ITimingResultReceiver
        {
            void OnTimingResult(string name, TimeSpan elapsed);
        }

        private class DefaultTimingResultReceiver
            : ITimingResultReceiver
        {
            public void OnTimingResult(string name, TimeSpan elapsed)
            {
                if(elapsed.TotalMilliseconds > 5000)
                {
                    if (string.IsNullOrEmpty(name))
                        Debug.Log($"Scope finished in {elapsed.TotalSeconds}s");
                    else
                        Debug.Log($"Scope ({name}) finished in {elapsed.TotalSeconds}ms");
                }
                else
                {
                    if (string.IsNullOrEmpty(name))
                        Debug.Log($"Scope finished in {elapsed.TotalMilliseconds}s");
                    else
                        Debug.Log($"Scope ({name}) finished in {elapsed.TotalMilliseconds}ms");
                }
            }
        }

        private class ProfilingScope
            : IDisposable
        {
            private DateTime m_startTime;
            string m_scopeName;
            private ITimingResultReceiver m_receiver;

            public void Start(string name, ITimingResultReceiver receiver)
            {
                m_startTime = DateTime.Now;
                m_scopeName = name;
                m_receiver = receiver;
            }

            public void Dispose()
            {
                if (null != m_receiver)
                    End();

                m_scopeName = null;
                m_receiver = null;
            }

            public void End()
            {
                var endTime = DateTime.Now;
                m_receiver.OnTimingResult(m_scopeName, endTime - m_startTime);

                m_freeScopes.Add(this);
            }
        }

        private static DefaultTimingResultReceiver m_defaultReceiver = new();
        private static List<ProfilingScope> m_freeScopes = new();

        public static IDisposable Profile(string name = "", ITimingResultReceiver receiver = null)
        {
            if (0 == m_freeScopes.Count)
                m_freeScopes.Add(new ProfilingScope());

            var scope = m_freeScopes.PopUnordered();

            if (null == receiver)
                receiver = m_defaultReceiver;

            scope.Start(name, receiver);
            return scope;
        }
    }
}

