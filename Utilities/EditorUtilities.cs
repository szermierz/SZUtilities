using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EditorUtilities
{
    public static class ApplicationStatus
    {
        public static bool IsQuitting { get; private set; }
        
        [RuntimeInitializeOnLoadMethod]
        static void RegisterQuitting()
        {
            IsQuitting = false;
            Application.quitting -= OnQuit;
            Application.quitting += OnQuit;
        }

        private static void OnQuit()
        {
            IsQuitting = true;
        }
    }
#if UNITY_EDITOR
    public static class _EditorDirty
    {
        public static void Mark(ScriptableObject scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);
        }
        public static void Mark(Component component)
        {
            if(!component)
                return;

            Mark(component.gameObject);
        }
        public static void Mark(GameObject gameObject)
        {
            if (Application.isPlaying)
                return;

            if(!gameObject)
                return;

            if(gameObject.scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            else
                EditorUtility.SetDirty(gameObject);
        }
    }
#endif
}
