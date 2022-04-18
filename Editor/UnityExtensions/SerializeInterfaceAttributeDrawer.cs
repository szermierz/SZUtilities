using UnityEngine;
using UnityEditor;

namespace SZUtilities._Editor
{
    [CustomPropertyDrawer(typeof(SerializeInterfaceAttribute))]
    public class SerializeInterfaceAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var requiredAttribute = (SerializeInterfaceAttribute) attribute;
                EditorGUI.BeginProperty(position, label, property);
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue,
                    requiredAttribute.requiredType, true);
                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
                GUI.color = previousColor;
            }
        }
    }
}