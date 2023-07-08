namespace Aarthificial.Reanimation.Editor.GraphView
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using System.Linq;
    using Aarthificial.Reanimation.Nodes;
    using UnityEngine.EventSystems;
    using UnityEditor;
    using System.Reflection;

    public class ReanimatorGraphView : GraphView
    {
        public new class UxmlFactory
            : UxmlFactory<ReanimatorGraphView, ReanimatorGraphView.UxmlTraits> { }

        public Action<ReanimatorNodeView> OnNodeSelected;
        public Action<ReanimatorNodeView> OnNodeUnSelected;
        public Reanimator SelectedReanimator { get; set; }

        //private bool isAnimationNodesHidden = false;
        private SwitchNode _rootNode;

        public ReanimatorGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphView"));

            // Enable common graph interactions
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new SelectionDragger());
            //this.AddManipulator(new RectangleSelector());

            // Setup Zoom
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            RegisterCallback<PointerDownEvent>(SelectGameObject);

            style.backgroundColor = StylesUtility.Colors.Dark;
        }
        public override EventPropagation DeleteSelection()
        {
            foreach (ReanimatorNodeView node in selection)
            {
                node.DetachFromParent();
            }
            return EventPropagation.Stop;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            VisualElement contentViewContainer = ElementAt(0);
            Vector3 screenMousePosition = evt.localMousePosition;
            Vector2 worldMousePosition =
                screenMousePosition - contentViewContainer.transform.position;
            worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
            if (evt.target is ReanimatorNodeView)
            {
                ReanimatorNodeView reanimatorNodeView = evt.target as ReanimatorNodeView;
                switch (reanimatorNodeView.GetType().Name)
                {
                    case nameof(SwitchNodeView):
                        evt.menu.AppendAction("Create Switch Node", actionEvent =>
                        {
                            SwitchNode node = reanimatorNodeView.Node as SwitchNode;
                            reanimatorNodeView.CreateChildAsset<SwitchNode>();
                            Generate();
                        });
                        evt.menu.AppendAction("Create Animation Node", actionEvent =>
                        {
                            SwitchNode node = reanimatorNodeView.Node as SwitchNode;
                            reanimatorNodeView.CreateChildAsset<SimpleAnimationNode>();
                            Generate();
                        });
                        break;
                }
                evt.menu.AppendAction("Edit Name", actionEvent =>
                {
                    reanimatorNodeView.EnableEditName();
                });
            }

        }

        private void SelectGameObject(PointerDownEvent ev)
        {
            Selection.activeGameObject = SelectedReanimator.gameObject;
        }

        #region Generate Graph
        public void Generate(ReanimatorNode rootNode = null)
        {
            RemoveAllGraphElements();
            if (SelectedReanimator != null)
            {
                rootNode = SelectedReanimator.root;
            }
            ReanimatorNodeView.Grid = new Dictionary<int, List<ReanimatorNodeView>>();
            if (rootNode == null)
                rootNode = _rootNode;
            if (rootNode is SwitchNode switchNode)
                _rootNode = switchNode;
            if (rootNode == null) return;
            if (!(rootNode is SwitchNode))
            {
                AddNode(rootNode, 0);
            }
            else
            {
                var rootNodeView = AddNode(_rootNode, 0);
                populateAllNodes(_rootNode.Nodes, 1, rootNodeView);
            }
        }
        public void RemoveAllGraphElements()
        {
            ports.ForEach(
                (port) =>
                {
                    
                    port.Clear();
                }
            );

            nodes.ForEach(
                (node) =>
                {
                    if(node is ReanimatorNodeView reanimatorView)
                        reanimatorView.UnregisterAllCallbacks();
                    node.Clear();
                    
                }
            );

            edges.ForEach(
                (edge) =>
                {
                    edge.Clear();
                }
            );
        }
        private void populateAllNodes(
            ReanimatorNode[] nodes,
            int level = 0,
            ReanimatorNodeView prevNode = null)
        {
            foreach (var node in nodes)
            {
                if (node == null)
                    continue;
                var nodeView = AddNode(node, level, prevNode);

                if (node is SwitchNode switchNode)
                {
                    populateAllNodes(switchNode.Nodes, level + 1, nodeView);
                }
            }
        }
        private ReanimatorNodeView AddNode(
            ReanimatorNode node,
            int level = 0,
            ReanimatorNodeView prevNodeView = null
        )
        {
            ReanimatorNodeView nodeView;
            if (node is SwitchNode switchNode)
                nodeView = new SwitchNodeView(switchNode, level, prevNodeView);
            else if (node is SimpleAnimationNode simpleAnimationNode)
                nodeView = new SimpleAnimationNodeView(simpleAnimationNode, level, prevNodeView);
            else
                nodeView = new ReanimatorNodeView(node, level, prevNodeView);
            node.OnValidated += () => OnNodeValidated(nodeView);
            nodeView.OnNodeSelected += (ReanimatorNodeView nodeView) =>
                OnNodeSelected?.Invoke(nodeView);
            nodeView.OnNodeUnSelected += (ReanimatorNodeView nodeView) =>
                OnNodeUnSelected?.Invoke(nodeView);
            nodeView.OnDetached += () => Generate();
            AddElement(nodeView);
            return nodeView;
        }
        private void OnNodeValidated(ReanimatorNodeView reanimatorNodeView)
        {
            if (reanimatorNodeView is SwitchNodeView switchNodeView)
                if (switchNodeView.NodesChanged()) Generate();
        }
        #endregion
        
        

        /*public void ToggleAnimationNodes()
        {
            isAnimationNodesHidden = !isAnimationNodesHidden;
            foreach (var node in nodes.ToList())
            {
                foreach(VisualElement visualElement in node.outputContainer.Children())
                {
                    if (isAnimationNodesHidden)
                    {
                        visualElement.style.opacity = 0;
                        visualElement.style.height = 0;
                    }
                    else
                    {
                        visualElement.style.opacity = 1;
                        visualElement.style.position = Position.Relative;

                    }
                }
            }
        }*/

        /*public void CreateGroup(Vector2 position)
       {
           var group = new Group() { title = "New Group", autoUpdateGeometry = true };
           group.SetPosition(new Rect(position, new Vector2(0, 0)));
           AddElement(group);
       }*/
    }
}
