using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class Singleton<SingletonType> : MonoBehaviourEx
    where SingletonType : Singleton<SingletonType>
{
    #region Singleton

    private static Singleton<SingletonType> s_earlyInstance = null;
    public static SingletonType Instance { get; private set; }

    private void InitializeSingleton()
    {
        if (s_earlyInstance)
        {
            Destroy(gameObject);
            return;
        }

        s_earlyInstance = this;
        DontDestroyOnLoad(gameObject);

        var init = Initialize();
        if(null != init)
        {
            init = Routines.Concat(init, Routines.Action(() => { Instance = this as SingletonType; }));
            StartCoroutine(init);
        }
        else
        {
            Instance = this as SingletonType;
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
