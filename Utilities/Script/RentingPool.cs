using System;
using System.Collections.Generic;
using System.Diagnostics;
using SZUtilities.Extensions;

namespace SZUtilities
{
    public interface IRentIdentifier
    {
        long RentID { get; }
        void UpdateRent(long id);
    }

    public struct RentedRef<TargetType>
        where TargetType : IRentIdentifier
    {
        private TargetType m_target;
        private long m_rentID;

        public RentedRef(TargetType target)
        {
            m_target = target;
            m_rentID = null != target
                ? target.RentID
                : default;
        }

        public void Set(TargetType target)
        {
            m_target = target;
            m_rentID = null != target
                ? target.RentID
                : default;
        }

        public TargetType Get()
        {
            if (null != m_target && m_target.RentID != m_rentID)
            {
                m_target = default;
                m_rentID = default;
            }

            return m_target;
        }

        public override bool Equals(object obj)
        {
            return obj is RentedRef<TargetType> @ref &&
                   EqualityComparer<TargetType>.Default.Equals(m_target, @ref.m_target) &&
                   m_rentID == @ref.m_rentID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_target, m_rentID);
        }

        public static bool operator ==(RentedRef<TargetType> lhs, RentedRef<TargetType> rhs)
        {
            return !(lhs != rhs);
        }

        public static bool operator !=(RentedRef<TargetType> lhs, RentedRef<TargetType> rhs)
        {
            if (lhs.m_rentID != rhs.m_rentID)
                return true;
            if (!ReferenceEquals(lhs.m_target, rhs.m_target))
                return true;

            return false;
        }
    }


    internal class RentingHandle
        : IDisposable
    {
        private static List<RentingHandle> s_freeHandles = new();

        private Action<object> m_returnAction;
        private object m_element;

        [DebuggerStepThrough]
        public static IDisposable GetHandle(Action<object> returnAction, object element)
        {
            RentingHandle handle;

            lock (s_freeHandles)
            {
                if (0 == s_freeHandles.Count)
                    s_freeHandles.Add(new RentingHandle());

                handle = s_freeHandles.PopUnordered();
            }

            handle.m_returnAction = returnAction;
            handle.m_element = element;

            return handle;
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
            if (null == m_returnAction)
                return;

            m_returnAction.Invoke(m_element);
            m_returnAction = null;
            m_element = null;

            lock(s_freeHandles)
            {
                s_freeHandles.Add(this);
            }
        }
    }

    public static class RentingPool<RentingType>
        where RentingType : class, new()
    {
        private static long s_currentRentID;

        private static Action<object> s_returnAction = (element) => 
        {
            var rentElement = (RentingType)element;
            lock (s_rentingPool)
                s_rentingPool.Add(rentElement);

            if (rentElement is IRentIdentifier rentIdentifier)
                rentIdentifier.UpdateRent(default);
        };

        private static List<RentingType> s_rentingPool = new();

        [DebuggerStepThrough]
        public static IDisposable Rent(out RentingType result)
        {
            lock (s_rentingPool)
            {
                if (0 == s_rentingPool.Count)
                    s_rentingPool.Add(new RentingType());
                
                result = s_rentingPool.PopUnordered();

                if (result is IRentIdentifier rentIdentifier)
                {
                    if (++s_currentRentID >= long.MaxValue)
                        s_currentRentID = 1;

                    rentIdentifier.UpdateRent(s_currentRentID);
                }
            }

            return RentingHandle.GetHandle(s_returnAction, result);
        }
    }
}
