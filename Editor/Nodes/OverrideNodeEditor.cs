using System;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    [CustomEditor(typeof(OverrideNode))]
    public class OverrideNodeEditor : UnityEditor.Editor
    {
        private SerializedProperty _next;
        private SerializedProperty _override;

        private void OnEnable()
        {
            _next = serializedObject.FindProperty("next");
            _override = serializedObject.FindProperty("overrides");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_next);
            EditorGUILayout.PropertyField(_override);

            serializedObject.ApplyModifiedProperties();
        }
    }
}