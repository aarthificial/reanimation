using Aarthificial.Reanimation.KeyFrames;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    [CustomPropertyDrawer(typeof(KeyFrame))]
    public class KeyFramePropertyDrawer : PropertyDrawer
    {
        private const float Size = 60;
        private const float Padding = 6;
        private const float Spacing = 2;
        private const float Margin = 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var spriteProp = property.FindPropertyRelative("sprite");
            var driversProp = property.FindPropertyRelative("drivers");

            return Mathf.Max(
                       EditorGUI.GetPropertyHeight(spriteProp)
                       + EditorGUI.GetPropertyHeight(driversProp)
                       + Spacing * 6,
                       Size
                   )
                   + Margin * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.y += Margin;

            var spriteProp = property.FindPropertyRelative("sprite");
            var sprite = spriteProp.objectReferenceValue as Sprite;
            var spritePosition = position;
            spritePosition.width = Size - 2 * Padding;
            spritePosition.height = Size - 2 * Padding;
            position.width -= Size;
            spritePosition.x += position.width + Padding;
            spritePosition.y += Padding;
            Helpers.DrawTexturePreview(spritePosition, sprite);

            position.height = EditorGUI.GetPropertyHeight(spriteProp);
            position.y += Spacing;
            EditorGUI.PropertyField(position, spriteProp);
            position.y += position.height + Spacing;

            var driversProp = property.FindPropertyRelative("drivers");
            position.height = EditorGUI.GetPropertyHeight(driversProp);
            position.y += Spacing;
            EditorGUI.PropertyField(position, driversProp);
            position.y += position.height + Spacing;

            EditorGUI.EndProperty();
        }
    }
}