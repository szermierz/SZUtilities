using UnityEngine;

namespace SZUtilities
{
    public class ScriptableObjectEx : ScriptableObject
    {
        #region Construction

        public ScriptableObjectEx()
        {
            Debug = new DebugEx(this);

        }

        #endregion

        #region DebugEx

        protected readonly DebugEx Debug;

        #endregion
    }
}