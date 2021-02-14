using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PostprocessObject : MonoBehaviourEx
{
    private static new DebugUtilities.DebugEx Debug = new DebugUtilities.DebugEx(nameof(PostprocessObject));

    public enum PostprocessAction
    {
        DeleteObject,
        DisableObject,
        DeleteIfNotEditor,
    }

    public PostprocessAction m_action = PostprocessAction.DeleteObject;

#if UNITY_EDITOR
    [UnityEditor.Callbacks.PostProcessScene]
    private static void PostprocessScene()
    {
        var objectsToPostprocess = Resources.FindObjectsOfTypeAll<PostprocessObject>().Where(x => !UnityEditor.EditorUtility.IsPersistent(x)).ToArray();

        for (int sceneId = 0; sceneId < UnityEngine.SceneManagement.SceneManager.sceneCount; ++sceneId)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneId);
            var sceneName = null != scene.name ? scene.name : "<null-name>";
            Debug.Log($"Running PostprocessScene() on scene {sceneName}");
        }

        foreach (var obj in objectsToPostprocess)
        {
            if (obj == null)
                continue;

            PostprocessSceneObject(obj);
        }
    }

    private static void PostprocessSceneObject(PostprocessObject obj)
    {
        string sceneName = "NoScene";
        if (!obj || !obj.gameObject)
        {
            Debug.LogError($"Trying to resolve null obj!");
            return;
        }
        else
        {
            sceneName = obj.gameObject.scene.name;
        }

        switch (obj.m_action)
        {
            case PostprocessAction.DisableObject:
                obj.gameObject.SetActive(false);
                Debug.Log($"Disabling {obj.name}: {sceneName} because PostprocessAction.DisableObject");
                DestroyImmediate(obj);
                break;

            case PostprocessAction.DeleteObject:
                Debug.Log($"Destroying {obj.name}: {sceneName} because PostprocessAction.DeleteObject");
                DestroyImmediate(obj.gameObject);
                break;

            case PostprocessAction.DeleteIfNotEditor:
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Debug.Log($"Destroying {obj.name}: {sceneName} because PostprocessAction.DeleteIfNotEditor");
                    DestroyImmediate(obj.gameObject);
                }
                break;

            default:
                throw new System.NotSupportedException(obj.m_action.ToString());
        }
    }
#endif

    protected override void Awake()
    {
        base.Awake();

        var toDestroy = new List<Object>();
        switch (m_action)
        {
            case PostprocessAction.DisableObject:
                gameObject.SetActive(false);
                toDestroy.Add(this);
                break;

            case PostprocessAction.DeleteObject:
                toDestroy.Add(gameObject);
                break;

            default:
                return;
        }

        Debug.LogWarning($"PostProcess {m_action} {gameObject.scene.name}:{gameObject.name}: executed in game mode!");

        foreach (var item in toDestroy)
        {
            if (Application.isPlaying)
                Destroy(item);
            else
                DestroyImmediate(item);
        }
    }
}