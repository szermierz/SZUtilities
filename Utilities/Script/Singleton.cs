
using System.Collections;

public class Singleton<SingletonType> : MonoBehaviourEx
    where SingletonType : Singleton<SingletonType>
{
    #region Singleton

    public static SingletonType Instance { get; private set; }

    private void InitializeSingleton()
    {
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

    protected override void Awake()
    {
        base.Awake();

        InitializeSingleton();
    }

    protected virtual IEnumerator Initialize() => null;
}
