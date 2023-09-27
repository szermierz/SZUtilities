using System;
using System.Collections.Generic;
using System.Linq;
using SZUtilities.Extensions;

namespace SZUtilities
{
    public class DisposablesGroup : IDisposable
    {
        private static List<DisposablesGroup> s_freeGroups = new();

        private IDisposable m_listHandle = null;
        private List<IDisposable> m_list = null;

        private DisposablesGroup()
        { }

        public static DisposablesGroup Rent()
        {
            if(s_freeGroups.Any())
                return s_freeGroups.PopUnordered();

            var group = new DisposablesGroup();
            return group;
        }

        public void Add(IDisposable disposable)
        {
            if(null == m_list)
            {
                if (null != m_listHandle)
                    throw new Exception();

                m_listHandle = ListRenting.Rent(out m_list);
            }

            m_list.Add(disposable);
        }

        public void Dispose()
        {
            foreach (var disposable in m_list)
                disposable.Dispose();

            m_list.Clear();
            m_listHandle.Dispose();

            m_list = null;
            m_listHandle = null;

            s_freeGroups.Add(this);
        }
    }
}

