namespace Aarthificial.Reanimation.Editor.GraphView
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor;
    using Aarthificial.Reanimation.Nodes;

    public class ReanimatorInspectorView : VisualElement
    {
        Editor editor;

        public new class UxmlFactory : UxmlFactory<ReanimatorInspectorView, ReanimatorInspectorView.UxmlTraits> { }
        public Action OnChange;

        internal void UpdateSelection(ReanimatorNode node)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            if (node == null)
                return;
            editor = Editor.CreateEditor(node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                EditorGUI.BeginChangeCheck();
                {
                    editor.DrawDefaultInspector();
                }
                if(EditorGUI.EndChangeCheck()){
                    OnChange?.Invoke();
                    editor.serializedObject.ApplyModifiedProperties();
                }
            });
            Add(container);
        }


    }
}

