using System.IO;
using UnityEditor;
using UnityEngine;

namespace SZUtilities._Editor
{
    [CustomPropertyDrawer(typeof(FolderReference))]
    public class FolderReferencePropertyDrawer : PropertyDrawer
    {
        private bool m_initialized;
        private SerializedProperty m_guid;
        private Object m_obj;

        private void Init(SerializedProperty property)
        {
            m_initialized = true;
            m_guid = property.FindPropertyRelative("m_guid");
            m_obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(m_guid.stringValue));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_initialized) Init(property);

            GUIContent guiContent = EditorGUIUtility.ObjectContent(m_obj, typeof(DefaultAsset));

            Rect r = EditorGUI.PrefixLabel(position, label);

            Rect textFieldRect = r;
            textFieldRect.width -= 19f;

            GUIStyle textFieldStyle = new GUIStyle("TextField")
            {
                imagePosition = m_obj ? ImagePosition.ImageLeft : ImagePosition.TextOnly
            };

            if (GUI.Button(textFieldRect, guiContent, textFieldStyle) && m_obj)
                EditorGUIUtility.PingObject(m_obj);

            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    DragAndDrop.visualMode = Directory.Exists(path) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    if (Directory.Exists(path))
                    {
                        m_obj = reference;
                        m_guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                    }
                    Event.current.Use();
                }
            }

            Rect objectFieldRect = r;
            objectFieldRect.x = textFieldRect.xMax + 1f;
            objectFieldRect.width = 19f;

            if (GUI.Button(objectFieldRect, "", GUI.skin.GetStyle("IN ObjectField")))
            {
                string path = EditorUtility.OpenFolderPanel("Select a folder", "Assets", "");
                if (path.Contains(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    m_obj = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                    m_guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_obj));
                }
                else Debug.LogError("The path must be in the Assets folder");
            }
        }
    }
}