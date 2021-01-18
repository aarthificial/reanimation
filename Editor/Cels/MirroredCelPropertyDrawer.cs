using System.Collections.Generic;
using Aarthificial.Reanimation.Cels;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Cels
{
    [CustomPropertyDrawer(typeof(MirroredCel))]
    public class MirroredCelPropertyDrawer: CelPropertyDrawer
    {
        protected override IEnumerable<string> PropertiesToDraw => new []{"sprite", "spriteLeft", "drivers"};

        protected override Rect DrawPreview(Rect position, SerializedProperty property)
        {
            position = DrawSpritePreview(position, property, "sprite");
            position = DrawSpritePreview(position, property, "spriteLeft");

            return position;
        }
    }
}