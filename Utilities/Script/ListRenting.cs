using System;
using System.Collections.Generic;
using System.Linq;

namespace SZUtilities
{
    public static class ListRenting
    {
        private static Dictionary<Type, List<object>> s_listsPool = new Dictionary<Type, List<object>>();

        public static IDisposable Rent<ElementType>(out List<ElementType> list)
        {
            lock(s_listsPool)
            {
                var type = typeof(ElementType);

                if (!s_listsPool.ContainsKey(type))
                    s_listsPool.Add(type, new List<List<ElementType>>() as List<object>);

                var lists = s_listsPool[type];

                if (!lists.Any())
                    lists.Add(new List<ElementType>());

                var lastIndex = lists.Count - 1;
                list = lists[lastIndex] as List<ElementType>;
                lists.RemoveAt(lastIndex);

                return GetHandle(list);
            }
        }

        public static void Return<ElementType>(List<ElementType> list)
        {
            lock(s_listsPool)
            {
                var type = typeof(ElementType);

                if (!s_listsPool.ContainsKey(type))
                    s_listsPool.Add(type, new List<List<ElementType>>() as List<object>);

                var lists = s_listsPool[type];

                list.Clear();
                lists.Add(list);
            }
        }

        private static Dictionary<Type, List<object>> s_handles = new Dictionary<Type, List<object>>();

        private static ListHandle<ElementType> GetHandle<ElementType>(List<ElementType> list)
        {
            lock(s_handles)
            {
                var type = typeof(ElementType);
                if (!s_handles.ContainsKey(type))
                    s_handles.Add(type, new List<ListHandle<ElementType>>() as List<object>);

                var handles = s_handles[type] as List<ListHandle<ElementType>>;
                if (!handles.Any())
                    handles.Add(new ListHandle<ElementType>());

                var lastIndex = handles.Count - 1;
                var handle = handles[lastIndex];
                handles.RemoveAt(lastIndex);

                handle.Assign(list);
                return handle;
            }
        }

        private static void ReturnHandle<ElementType>(ListHandle<ElementType> handle)
        {
            lock(s_handles)
            {
                var type = typeof(ElementType);
                if (!s_handles.ContainsKey(type))
                    s_handles.Add(type, new List<ListHandle<ElementType>>() as List<object>);

                var handles = s_handles[type] as List<ListHandle<ElementType>>;
                handles.Add(handle);
            }
        }

        private class ListHandle<ElementType> : IDisposable
        {
            private List<ElementType> m_list;

            public void Assign(List<ElementType> list)
            {
                if (null != m_list)
                    throw new Exception();

                m_list = list;
            }

            public void Dispose()
            {
                if (null == m_list)
                    return;

                Return(m_list);
                m_list = null;

                ReturnHandle(this);
            }
        }
    }
}
