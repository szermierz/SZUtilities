using System;
using System.Collections.Generic;
using SZUtilities.Extensions;

namespace SZUtilities
{
    /**
     The purpose of this class is to solve the following case:

    using var someHandle = RentSomeResource();
    someHandle.Dispose(); // Cannot do this earlier, because someHandle is using var

    Solution:

    var someHandle = RentSomeResource();
    using var sharedHandle = SharedDisposable.Rent(someHandle);
    sharedHandle.SoftDispose(); // Possible, then later it will only Dispose shared handle
     */

    public class SharedDisposable 
        : IDisposable
    {
        private static List<SharedDisposable> s_freeSharedDisposables = new();

        private IDisposable m_sharedWith;

        public static SharedDisposable Rent(IDisposable toShareWith)
        {
            if (s_freeSharedDisposables.Count == 0)
                s_freeSharedDisposables.Add(new SharedDisposable());

            var sharedDisposable = s_freeSharedDisposables.PopUnordered();

            if (null != sharedDisposable.m_sharedWith)
                throw new Exception();

            sharedDisposable.m_sharedWith = toShareWith;
            return sharedDisposable;
        }

        public void SoftDispose()
        {
            m_sharedWith?.Dispose();
            m_sharedWith = null;
        }

        public void Dispose()
        {
            SoftDispose();
            s_freeSharedDisposables.Add(this);
        }
    }
}

