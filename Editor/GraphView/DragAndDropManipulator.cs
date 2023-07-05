namespace Aarthificial.Reanimation.Editor.GraphView
{
    using Aarthificial.Reanimation.Nodes;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class ReanimatorEditorWindow : EditorWindow
    {
        class DragAndDropManipulator : PointerManipulator
        {
            public DragAndDropManipulator(VisualElement root)
            {
                target = root.Q<VisualElement>("Graph");
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<DragEnterEvent>(OnDragEnter);
                target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                target.RegisterCallback<DragPerformEvent>(OnDragPerform);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
                target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
            }
            void OnDragEnter(DragEnterEvent _)
            {
            }
            void OnDragUpdate(DragUpdatedEvent ev)
            {
                if(ev.target is SwitchNodeView)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
            }
            void OnDragPerform(DragPerformEvent ev)
            {
                if(ev.target is SwitchNodeView switchNodeView)
                {
                    foreach (ReanimatorNode i in DragAndDrop.objectReferences)
                    {
                        List<ReanimatorNode> nodes = switchNodeView.Node.Nodes.ToList();
                        nodes.Add(i);
                        switchNodeView.Node.Nodes = nodes.ToArray();
                        if (ev.currentTarget is ReanimatorGraphView reanimatorGraphView)
                            reanimatorGraphView.Generate();
                    }
                }
            }
        }
    }
}
