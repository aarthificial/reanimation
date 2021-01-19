using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    [CustomEditor(typeof(Reanimator))]
    public class ReanimatorEditor : UnityEditor.Editor
    {
        private SerializedProperty _root;
        private SerializedProperty _renderer;
        private SerializedProperty _fps;
        private SerializedProperty _temporary;
        private Reanimator _reanimator;
        private bool _isExpanded;

        private void OnEnable()
        {
            _root = serializedObject.FindProperty(nameof(Reanimator.root));
            _renderer = serializedObject.FindProperty("renderer");
            _fps = serializedObject.FindProperty("fps");
            _temporary = serializedObject.FindProperty("temporaryDrivers");
            _reanimator = target as Reanimator;
        }

        public override bool RequiresConstantRepaint()
        {
            return _isExpanded && _reanimator != null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_root);
            EditorGUILayout.PropertyField(_renderer);
            EditorGUILayout.PropertyField(_fps);
            EditorGUILayout.PropertyField(_temporary);

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            _isExpanded = EditorGUILayout.Foldout(_isExpanded, "State", true);
            EditorGUI.EndDisabledGroup();

            if (Application.isPlaying && _isExpanded && _reanimator != null)
            {
                var boxStyle = new GUIStyle("Box")
                {
                    padding = new RectOffset(8, 8, 8, 8),
                    margin = new RectOffset(8, 8, 8, 8)
                };

                EditorGUIUtility.labelWidth /= 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.LabelField("Previous", EditorStyles.boldLabel);
                EditorGUILayout.Separator();
                foreach (var pair in _reanimator.State)
                    EditorGUILayout.IntField(pair.Key, pair.Value);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.LabelField("Next", EditorStyles.boldLabel);
                EditorGUILayout.Separator();
                foreach (var pair in _reanimator.NextState)
                    EditorGUILayout.IntField(pair.Key, pair.Value);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 0;
            }
            else
            {
                _isExpanded = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}