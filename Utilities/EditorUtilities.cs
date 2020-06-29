﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace _EditorUtilities
{
    public static class _EditorDirty
    {
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
}

#endif