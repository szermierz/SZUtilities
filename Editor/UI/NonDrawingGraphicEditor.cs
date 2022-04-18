using SZUtilities.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace SZUtilities._Editor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
	public class NonDrawingGraphicEditor : GraphicEditor
	{
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[] { });
			base.RaycastControlsGUI();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}