using UnityEngine;

namespace SZUtilities
{
    [DefaultExecutionOrder(-999)]
    public class Singleton<SingletonType> : LocalSingleton<SingletonType>
        where SingletonType : Singleton<SingletonType>
    {
        protected override bool MoveToDontDestroy => true;
    }
}