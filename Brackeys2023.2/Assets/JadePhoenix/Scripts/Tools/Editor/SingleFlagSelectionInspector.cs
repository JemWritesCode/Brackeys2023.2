#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace JadePhoenix.Tools
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SingleFlagSelectionAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SingleFlagSelectionAttribute))]
    public class SingleFlagSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUI.Popup(position, label.text, property.intValue, property.enumNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }
    }
#endif
}