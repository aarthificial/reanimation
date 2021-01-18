using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Common
{
    [CustomPropertyDrawer(typeof(ControlDriver))]
    public class ControlDriverPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 1;
        private const float Margin = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float name = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("name"));
            float ai = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("autoIncrement"));
            float percentage = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("percentageBased"));

            return property.isExpanded
                ? name + name + ai + percentage + Spacing * 8 + Margin * 2
                : name + Spacing * 2 + Margin * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var name = property.FindPropertyRelative("name");
            var ai = property.FindPropertyRelative("autoIncrement");
            var percentage = property.FindPropertyRelative("percentageBased");

            if (ai.boolValue)
                percentage.boolValue = false;

            var labelText = "Control driver";
            if (!string.IsNullOrEmpty(name.stringValue))
                labelText += $" ({name.stringValue})";
            
            position.y += Margin;
            position.y += Spacing;
            position.height = EditorGUI.GetPropertyHeight(name);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, labelText, true);
            position.y += position.height + Spacing;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += Spacing;
                EditorGUI.PropertyField(position, name);
                position.y += position.height + Spacing;
            
                EditorGUI.BeginDisabledGroup(percentage.boolValue);
                position.y += Spacing;
                position.height = EditorGUI.GetPropertyHeight(ai);
                EditorGUI.PropertyField(position, ai);
                position.y += position.height + Spacing;
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(ai.boolValue);
                position.y += Spacing;
                position.height = EditorGUI.GetPropertyHeight(percentage);
                EditorGUI.PropertyField(position, percentage);
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}