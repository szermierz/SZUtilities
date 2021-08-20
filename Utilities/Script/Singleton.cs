using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class Singleton<SingletonType> : MonoBehaviourEx
    where SingletonType : Singleton<SingletonType>
{
    #region Singleton

    protected virtual bool WaitForInitialize => true;
    
    private static Singleton<SingletonType> s_earlyInstance = null;
    public static SingletonType Instance { get; private set; }
    
    public bool Initialized { get; private set; }

    private void InitializeSingleton()
    {
        if (s_earlyInstance)
        {
            Destroy(gameObject);
            return;
        }

        s_earlyInstance = this;
        
        if(!WaitForInitialize)
            Instance = this as SingletonType;
        
        DontDestroyOnLoad(gameObject);

        var init = Initialize();
        if(null != init)
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
