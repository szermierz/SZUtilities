using System;
using System.Collections.Generic;
using SZUtilities.Extensions;

namespace SZUtilities
{
    public sealed class DisposableAction : IDisposable
    {
        private static List<DisposableAction> s_actionsPool = new();

        public static IDisposable Create(Action callback)
        {
            if (0 == s_actionsPool.Count)
                s_actionsPool.Add(new DisposableAction());

            var action = s_actionsPool.PopUnordered();
            
            if (null != action.m_callback)
                throw new Exception();

            action.m_callback = callback;
            
            return action;
        }

        private Action m_callback;

        private DisposableAction()
        { }

        public void Dispose()
        {
            m_callback?.Invoke();
            m_callback = null;

            s_actionsPool.Add(this);
        }
    }
}

