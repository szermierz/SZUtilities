using DebugUtilities;
using UnityEngine;

public class ScriptableObjectEx : ScriptableObject, IValidable
{
    #region IValidable

    public bool Valid => null != this;

    #endregion
    
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
