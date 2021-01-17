using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Aarthificial.Reanimation.Editor
{
    public abstract class ControlNodeEditor : UnityEditor.Editor
    {
        protected SerializedProperty AI;
        protected SerializedProperty Percentage;

        private readonly string[] _propertiesToDraw = {"driverName", "drivers"};
        protected List<SerializedProperty> PropertiesToDraw;

        protected virtual void OnEnable()
        {
            PropertiesToDraw = _propertiesToDraw
                .Select(propertyName => serializedObject.FindProperty(propertyName))
                .ToList();

            AI = serializedObject.FindProperty("autoIncrement");
            Percentage = serializedObject.FindProperty("percentageBased");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCustomProperties();
            EditorGUILayout.Separator();
            DrawControlOptions();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomProperties()
        {
            foreach (var property in PropertiesToDraw)
                EditorGUILayout.PropertyField(property);
        }

        protected void DrawControlOptions()
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            if (AI.boolValue)
                Percentage.boolValue = false;
            EditorGUI.BeginDisabledGroup(Percentage.boolValue);
            EditorGUILayout.PropertyField(AI);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(AI.boolValue);
            EditorGUILayout.PropertyField(Percentage);
            EditorGUI.EndDisabledGroup();
        }
    }
}