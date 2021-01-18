using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Cels
{
    public abstract class CelPropertyDrawer : PropertyDrawer
    {
        private const float Size = 60;
        private const float Padding = 6;
        private const float Spacing = 2;
        private const float Margin = 4;

        protected virtual IEnumerable<string> PropertiesToDraw => new string[] { };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float childPropertiesHeight = PropertiesToDraw.Sum(
                name => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name))
            );

            return Mathf.Max(childPropertiesHeight + Spacing * 6, Size) + Margin * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.y += Margin;
            position = DrawPreview(position, property);

            foreach (string name in PropertiesToDraw)
            {
                var childProperty = property.FindPropertyRelative(name);
                position.height = EditorGUI.GetPropertyHeight(childProperty);
                position.y += Spacing;
                EditorGUI.PropertyField(position, childProperty);
                position.y += position.height + Spacing;
            }

            EditorGUI.EndProperty();
        }

        protected virtual Rect DrawPreview(Rect position, SerializedProperty property)
        {
            return DrawSpritePreview(position, property, "sprite");
        }

        protected Rect DrawSpritePreview(Rect position, SerializedProperty property, string name)
        {
            var spriteProp = property.FindPropertyRelative(name);
            var sprite = spriteProp.objectReferenceValue as Sprite;
            if (sprite == null || position.width < 300) return position;

            var spritePosition = position;
            spritePosition.width = Size - 2 * Padding;
            spritePosition.height = Size - 2 * Padding;
            position.width -= Size;
            spritePosition.x += position.width + Padding;
            spritePosition.y += Padding;
            Helpers.DrawTexturePreview(spritePosition, sprite);

            return position;
        }
    }
}