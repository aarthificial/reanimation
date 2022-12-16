namespace Aarthificial.Reanimation.Editor.GraphView
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using System.Linq;
    using UnityEditor;
    using Aarthificial.Reanimation.Nodes;
    using Aarthificial.Reanimation.Cels;
    using Aarthificial.Reanimation.Common;

    public class ReanimatorGraphView : GraphView
    {
        public new class UxmlFactory
            : UxmlFactory<ReanimatorGraphView, ReanimatorGraphView.UxmlTraits> { }

        SwitchNode _rootNode;
        public Action<ReanimatorNodeView> OnNodeSelected;
        private bool isAnimationNodesHidden = false;

        public ReanimatorGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphView"));

            // Enable common graph interactions
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Setup Zoom
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Setup Grid
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            VisualElement contentViewContainer = ElementAt(1);
            Vector3 screenMousePosition = evt.localMousePosition;
            Vector2 worldMousePosition =
                screenMousePosition - contentViewContainer.transform.position;
            worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
            evt.menu.AppendAction(
                "Create Switch Node",
                actionEvent => CreateNode(worldMousePosition)
            );
            evt.menu.AppendAction("Create Group", actionEvent => CreateGroup(worldMousePosition));

            base.BuildContextualMenu(evt);
        }

        public void CreateGroup(Vector2 position)
        {
            var group = new Group() { title = "New Group", autoUpdateGeometry = true };
            group.SetPosition(new Rect(position, new Vector2(0, 0)));
            AddElement(group);
        }

        public void CreateNode(Vector2 position)
        {
            var node = new SwitchNode();
            node.position = position;
            AddNode(node);
        }

        public void Generate(ReanimatorNode rootNode = null)
        {
            RemoveAllGraphElements();
            ReanimatorNodeView.levelCounts = new Dictionary<int, int>();
            if (rootNode == null)
                rootNode = _rootNode;
            if (rootNode is SwitchNode switchNode)
                _rootNode = switchNode;
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

        public void ToggleAnimationNodes()
        {
            isAnimationNodesHidden = !isAnimationNodesHidden;
            foreach (var node in nodes.ToList())
            {
                if (node is ReanimatorNodeView nodeView)
                {
                    if (nodeView.node is TerminationNode)
                    {
                        nodeView.visible = !isAnimationNodesHidden;
                    }
                    if (nodeView.node is SwitchNode)
                    {
                        nodeView.expanded = !isAnimationNodesHidden;
                    }
                }
            }
            foreach (var edge in edges)
            {
                if (edge.input.node is ReanimatorNodeView inpotNodeView)
                {
                    if (inpotNodeView.node is TerminationNode)
                    {
                        edge.visible = !isAnimationNodesHidden;
                    }
                }
            }
        }

        void populateAllNodes(
            ReanimatorNode[] nodes,
            int level = 0,
            ReanimatorNodeView prevNode = null
        )
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

        ReanimatorNodeView AddNode(
            ReanimatorNode node,
            int level = 0,
            ReanimatorNodeView prevNodeView = null
        )
        {
            var nodeView = new ReanimatorNodeView(node, level);

            if (prevNodeView != null)
            {
                // only Switch nodes can have outputs
                // meaning it was the previous node
                var prevSwitchNode = prevNodeView.node as SwitchNode;
                var myIndex = prevSwitchNode.Nodes.ToList().IndexOf(node);
                var prevOutput = prevNodeView.outputContainer[myIndex] as Port;

                var inputPort = GeneratePort(nodeView, Direction.Input, Port.Capacity.Multi);
                inputPort.portName = prevNodeView.node.name.ToString();
                var edge = inputPort.ConnectTo(prevOutput);

                inputPort.edgeConnector.target.Add(edge);

                nodeView.inputContainer.Add(inputPort);
            }
            var switchNode = node as SwitchNode;
            if (switchNode != null)
            {
                foreach (var tempNode in switchNode.Nodes)
                {
                    var outputPort = GeneratePort(nodeView, Direction.Output, Port.Capacity.Multi);
                    outputPort.portName = tempNode.name.ToString();
                    nodeView.outputContainer.Add(outputPort);
                }
            }
            if (switchNode == null || switchNode.Nodes.Length == 0)
            {
                nodeView.AddToClassList("end-node");
            }
            nodeView.OnNodeSelected += (ReanimatorNodeView nodeView) =>
                OnNodeSelected?.Invoke(nodeView);
            AddElement(nodeView);
            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();
            return nodeView;
        }

        private Port GeneratePort(
            ReanimatorNodeView nodeView,
            Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single
        )
        {
            return nodeView.InstantiatePort(
                Orientation.Horizontal,
                portDirection,
                capacity,
                typeof(float)
            ); //Arbitrary type
        }

        private void RemoveAllGraphElements()
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
    }
}
