using System.Collections;
using UnityEngine;

namespace SZUtilities
{
    [DefaultExecutionOrder(-999)]
    public abstract class LocalSingleton<SingletonType> : MonoBehaviourEx
        where SingletonType : LocalSingleton<SingletonType>
    {
        #region Singleton

        protected virtual bool WaitForInitialize => true;

        private static LocalSingleton<SingletonType> s_earlyInstance = null;
        public static SingletonType Instance { get; private set; }

        public bool Initialized { get; private set; }

        protected virtual bool MoveToDontDestroy => false;

        private void InitializeSingleton()
        {
            if (s_earlyInstance)
            {
                Destroy(gameObject);
                return;
            }

            s_earlyInstance = this;

            if (!WaitForInitialize)
                Instance = this as SingletonType;

            if (MoveToDontDestroy)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            var init = Initialize();
            if (null != init)
            {
                init = Routines.Concat(init, Routines.Action(() =>
                {
                    Instance = this as SingletonType;
                    Initialized = true;
                }));
                StartCoroutine(init);
            }
            else
            {
                Instance = this as SingletonType;
                Initialized = true;
            }
        }

        #endregion

        protected sealed override void Awake()
        {
            base.Awake();

            InitializeSingleton();
        }

        protected virtual IEnumerator Initialize() => null;
    }
}