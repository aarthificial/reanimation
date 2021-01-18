using System.Collections.Generic;
using UnityEditor;

namespace Aarthificial.Reanimation.Editor.Nodes
{
    public abstract class ReanimatorNodeEditor : UnityEditor.Editor
    {
        private readonly List<SerializedProperty> _propertiesToDraw = new List<SerializedProperty>();

        protected SerializedProperty AddCustomProperty(string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            _propertiesToDraw.Add(property);
            return property;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCustomProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawCustomProperties()
        {
            foreach (var property in _propertiesToDraw)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}