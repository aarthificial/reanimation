namespace Aarthificial.Reanimation.Editor.GraphView
{
    using System.Collections;
    using System.Collections.Generic;
    using Aarthificial.Reanimation.Nodes;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using UnityEditor.Experimental.GraphView;

    public class ReanimatorEditorWindow : EditorWindow
    {
        private ReanimatorGraphView graphView;
        ReanimatorInspectorView inspectorView;

        private SwitchNode rootNode;
        private bool isAnimationNodesHidden = false;
        VisualElement root => rootVisualElement;

        [MenuItem("Window/Reanimator/Reanimator Graph")]
        public static void OpenReanimatorGraph()
        {
            var window = GetWindow<ReanimatorEditorWindow>();
            window.titleContent = new GUIContent(text: "Reanimator Graph");
        }

        public void CreateGUI()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("GraphEditorWindow");
            visualTree.CloneTree(root);

            GenerateToolbar();
            GetGraphView();
            OnSelectionChange();
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var toggleCollapse = new Button(clickEvent: ToggleHideAnimationNodes);
            toggleCollapse.text = "Hide Animation Nodes";
            toggleCollapse.name = "toggle-button";
            toolbar.Add(toggleCollapse);

            root.Add(toolbar);
        }

        private void ToggleHideAnimationNodes()
        {
            isAnimationNodesHidden = !isAnimationNodesHidden;
            root.Q<Button>("toggle-button").text = isAnimationNodesHidden
                ? "Hide Animation Nodes"
                : "Show Animation Nodes";
            graphView.ToggleAnimationNodes();
        }

        private void OnSelectionChange()
        {
            var node = Selection.activeObject as ReanimatorNode;
            if (node != null)
            {
                graphView.Generate(node);
            }
            else if (Selection.activeGameObject)
            {
                Reanimator reanimator = Selection.activeGameObject.GetComponent<Reanimator>();
                if (reanimator != null)
                {
                    node = reanimator.root;
                    graphView.Generate(node);
                }
            }
            inspectorView.UpdateSelection(node);
        }

        void HandleInspectorChanged()
        {
            graphView.Generate();
        }

        void HandleNodeSelectionChanged(ReanimatorNodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView.node);
        }

        GraphViewChange HandleGraphViewChange(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                foreach (var graphElement in graphViewChange.movedElements)
                {
                    var nodeView = graphElement as ReanimatorNodeView;
                    if (nodeView == null)
                        continue;
                    var node = nodeView.node;
                    node.position = nodeView.GetPosition().position;
                }
            }
            AssetDatabase.SaveAssets();
            return graphViewChange;
        }

        void GetGraphView()
        {
            graphView = root.Q<ReanimatorGraphView>();
            inspectorView = root.Q<ReanimatorInspectorView>();

            Subscribe();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Re-establish Graph View to work in Edit Mode
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                GetGraphView();
            }
        }

        private void OnEnable()
        {
            GenerateToolbar();
            GetGraphView();
            if (graphView != null)
            {
                graphView.Generate(rootNode);
            }
        }

        void OnDisable()
        {
            Unsubscribe();
        }

        public void Subscribe()
        {
            // Play it safe!
            Unsubscribe();

            // Nobody else than us should listen to these evens (therefor =)
            if (graphView != null)
            {
                graphView.OnNodeSelected = HandleNodeSelectionChanged;
                graphView.graphViewChanged = HandleGraphViewChange;
            }

            // Nobody else than us should listen to these evens (therefor =)
            if (inspectorView != null)
            {
                inspectorView.OnChange = HandleInspectorChanged;
            }
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        // Unsubcribe from event
        public void Unsubscribe()
        {
            if (graphView != null)
            {
                graphView.OnNodeSelected -= HandleNodeSelectionChanged;
                graphView.graphViewChanged -= HandleGraphViewChange;
            }
            if (inspectorView != null)
            {
                inspectorView.OnChange -= HandleInspectorChanged;
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }
}
