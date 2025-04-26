
namespace SZUtilities
{
    public class ReuseableCancellationTokenSource
    {
        private long m_lastCancelIndex = 0;
        public long CurrentCancelIndex { get; protected set; }

        public ReuseableCancellationToken Token
        {
            get
            {
                Arm();
                return new ReuseableCancellationToken(this, CurrentCancelIndex);
            }
        }

        public void Arm()
        {
            if (CurrentCancelIndex < 0)
                CurrentCancelIndex = ++m_lastCancelIndex;
        }

        public void Cancel()
        {
            CurrentCancelIndex = -1;
        }

        public void Cancel(long cancellableIndex)
        {
            if (CurrentCancelIndex == cancellableIndex)
                Cancel();
        }
    }

    public readonly struct ReuseableCancellationToken
    {
        public readonly ReuseableCancellationTokenSource CancelSource;
        public readonly long CancelIndex;

        internal ReuseableCancellationToken(ReuseableCancellationTokenSource cancelSource, long cancelIndex)
        {
            CancelSource = cancelSource;
            CancelIndex = cancelIndex;
        }

        public readonly bool IsSetup => null != CancelSource;

        public readonly bool IsCancellationRequested => null != CancelSource 
            && CancelSource.CurrentCancelIndex != CancelIndex;
    }
}

