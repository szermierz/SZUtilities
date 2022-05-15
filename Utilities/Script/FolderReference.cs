using UnityEngine;

namespace SZUtilities
{
    [System.Serializable]
    public class FolderReference
    {
        [SerializeField]
        private string m_guid;
        public string Guid => m_guid;

#if UNITY_EDITOR
        public string _Path => UnityEditor.AssetDatabase.GUIDToAssetPath(Guid);
#endif
    }
}

