using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    [CustomPropertyDrawer(typeof(OverridePair))]
    public class OverridePairPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fromProp = property.FindPropertyRelative("fromNode");
            return EditorGUI.GetPropertyHeight(fromProp);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fromProp = property.FindPropertyRelative("fromNode");
            var toProp = property.FindPropertyRelative("toNode");

            position.width /= 2;
            EditorGUI.PropertyField(position, fromProp, GUIContent.none);
            position.x += position.width;
            EditorGUI.PropertyField(position, toProp, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}