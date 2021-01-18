using Aarthificial.Reanimation.Common;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Common
{
    [CustomPropertyDrawer(typeof(DriverDictionary))]
    public class DriverDictionaryPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keysProp = property.FindPropertyRelative("keys");
            return property.isExpanded ? keysProp.arraySize * 20 + 40 : 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = 20;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20;
            var buttonPosition = position;
            position.y += 20;
            
            if (property.isExpanded)
            {
                int removeIndex = -1;
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");
                for (var i = 0; i < keysProp.arraySize; i++)
                {
                    var fieldPosition = position;
                    fieldPosition.width /= 2;
                    EditorGUI.PropertyField(fieldPosition, keysProp.GetArrayElementAtIndex(i), GUIContent.none);
                    fieldPosition.x += fieldPosition.width;
                    fieldPosition.width -= 20;
                    EditorGUI.PropertyField(fieldPosition, valuesProp.GetArrayElementAtIndex(i), GUIContent.none);
                    fieldPosition.x += fieldPosition.width;
                    fieldPosition.width = 20;

                    if (GUI.Button(fieldPosition, "-"))
                        removeIndex = i;

                    position.y += 20;
                }

                buttonPosition.width /= 2;
                if (GUI.Button(buttonPosition, "Add"))
                {
                    valuesProp.InsertArrayElementAtIndex(0);
                    keysProp.InsertArrayElementAtIndex(0);

                    var keyProp = keysProp.GetArrayElementAtIndex(0);
                    keyProp.stringValue = "driver" + keysProp.arraySize;

                }

                buttonPosition.x += buttonPosition.width;
                if (GUI.Button(buttonPosition, "Clear"))
                {
                    keysProp.ClearArray();
                    valuesProp.ClearArray();
                }
                
                if (removeIndex > -1)
                {
                    valuesProp.DeleteArrayElementAtIndex(removeIndex);
                    keysProp.DeleteArrayElementAtIndex(removeIndex);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}