using UnityEngine;
using System.Linq;

namespace SZUtilities
{
    public abstract class MonoBehaviourEx : MonoBehaviour, IValidable
    {
        #region Accessors

        public bool Valid => null != this;

        #endregion

        #region Construction

        public MonoBehaviourEx()
        {
            Debug = new DebugEx(this);
        }

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            Verify();
#endif
        }

        protected virtual void Verify()
        { }

        #endregion

        #region DebugEx

        protected readonly DebugEx Debug;

        #endregion

        #region Components helpers

        protected T OwnComponent<T>(ref T m_cache)
            where T : class
        {
            if (null != m_cache)
            {
                if (m_cache is IValidable validableObject && validableObject.Valid)
                    return m_cache;

                if (m_cache is Component component && component)
                    return m_cache;
            }

            var query = GetComponents<Component>()
                .Select(_component => _component as T)
                .Where(_t => null != _t);

            if (typeof(IValidable).IsAssignableFrom(typeof(T)))
            {
                m_cache = query
                    .FirstOrDefault(_t => ((IValidable)_t).Valid);
            }
            else if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                m_cache = query.FirstOrDefault();
            }

            return m_cache;
        }

        protected T ParentComponent<T>(ref T m_cache, bool skipSelf = false)
            where T : class
        {
            if (null != m_cache)
            {
                if (m_cache is IValidable validableObject && validableObject.Valid)
                    return m_cache;

                if (m_cache is Component component && component)
                    return m_cache;
            }

            if (typeof(IValidable).IsAssignableFrom(typeof(T)))
            {
                MonoBehaviourEx parentIt = null;
                if (skipSelf)
                {
                    if (transform.parent && transform.parent.GetComponentInParent<MonoBehaviourEx>() is MonoBehaviourEx parent)
                        parentIt = parent;
                }
                else
                {
                    parentIt = this;
                }

                while (parentIt)
                {
                    if (parentIt.OwnComponent(ref m_cache) is IValidable validable && validable.Valid)
                        return m_cache;

                    if (parentIt.transform.parent is Transform parent)
                        parentIt = parent.GetComponentInParent<MonoBehaviourEx>();
                    else
                        parentIt = null;
                }
            }
            else if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                m_cache = GetComponentInParent<T>();
                return m_cache;
            }

            return m_cache;
        }

        #endregion
    }

    public interface IValidable
    {
        bool Valid { get; }
    }
}