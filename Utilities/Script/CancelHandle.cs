using System;
using System.Collections.Generic;
using SZUtilities.Extensions;

namespace SZUtilities.Cancellables
{
    public interface ICancellable
    {
        void Cancel(long cancellableIndex);
    }

    public static class CancelHandlePool
    {
        private static long s_cancellableIndex;
        private static List<CancelHandle> m_freeHandles = new();

        public static (IDisposable handle, long cancellableIndex) Rent(ICancellable cancellable)
        {
            lock(m_freeHandles)
            {
                if (0 == m_freeHandles.Count)
                    m_freeHandles.Add(new CancelHandle());

                var handle = m_freeHandles[0];
                m_freeHandles.RemoveUnorderedAt(0);

                handle.Cancellable = cancellable;
                handle.CancellableIndex = ++s_cancellableIndex;

                return (handle, handle.CancellableIndex);
            }
        }

        private class CancelHandle : IDisposable
        {
            public long CancellableIndex;
            public ICancellable Cancellable;

            public void Dispose()
            {
                lock(m_freeHandles)
                {
                    Cancellable?.Cancel(CancellableIndex);
                    Cancellable = null;
                    m_freeHandles.Add(this);
                }
            }
        }
    }



}