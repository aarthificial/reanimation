using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Common
{
    [CustomPropertyDrawer(typeof(OverridePair))]
    public class OverridePairPropertyDrawer : PropertyDrawer
    {
        private static class Styles
        {
            public static readonly GUIContent RemoveIcon = EditorGUIUtility.IconContent("d_tab_next");
            public static readonly GUIStyle IconButton = new GUIStyle("IconButton");
        }
        
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

            float width = (position.width - 20) / 2;
            position.width = width;
            EditorGUI.PropertyField(position, fromProp, GUIContent.none);
            position.x += width + 2;
            position.width = 10;
            EditorGUI.LabelField(position, Styles.RemoveIcon, Styles.IconButton);
            position.x += 18;
            position.width = width;
            EditorGUI.PropertyField(position, toProp, GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}