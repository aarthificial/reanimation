namespace Aarthificial.Reanimation.Editor.GraphView
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using UnityEditor;
    using Aarthificial.Reanimation.Nodes;
    using System.Drawing;
    using System.Linq;

    public class ReanimatorNodeView : Node
    {
        public static Dictionary<int, int> levelCounts = new Dictionary<int, int>();
        public Action<ReanimatorNodeView> OnNodeSelected;
        public Action<ReanimatorNodeView> OnNodeUnSelected;

        public string GUID;

        private ReanimatorNode node;
        public ReanimatorNode Node
        {
            get { return node; }
            set 
            { 
                node = value;
                RefreshPorts();
                RefreshExpandedState();
            }
        }
        public int Level { get; set; }
        public string Name { get; set; }
        public ReanimatorNodeView PreviousNodeView { get; set; }

        public ReanimatorNodeView(ReanimatorNode node, int level, ReanimatorNodeView previousNodeView = null)
        {
            Level = level;
            Node = node;
            GUID = Guid.NewGuid().ToString();
            Name = node.name;
            PreviousNodeView = previousNodeView;
            
            SetPosition(new Rect(LevelToPosition(level), Vector2.zero));
            Draw();
            GeneratePorts();
        }

        public void CreateChildAsset<T>() where T : ReanimatorNode
        {
            var node = ScriptableObject.CreateInstance<T>();
            SwitchNode parentNode = Node as SwitchNode;//parent node always SwitchNode, cause AnimationNode cannot have other nodes
            node.name = "New Node";
            List<ReanimatorNode> nodes = parentNode.Nodes.ToList();
            nodes.Add(node);
            parentNode.Nodes = nodes.ToArray();
            RGVIOUtility.SaveNode<T>(node);
        }

        private void Draw()
        {
            TextField nodeName = new TextField();
            nodeName.AddToClassList("nodeName");
            nodeName.value = Node.name;
            titleContainer.Clear(); //removing expand button
            titleContainer.Add(nodeName);
            nodeName.RegisterCallback<FocusOutEvent>(ChangeName);
        }

        private Vector2 LevelToPosition(int level)
        {
            if (levelCounts.ContainsKey(level))
                levelCounts[level]++;
            else
                levelCounts[level] = 0;
            var position = node.Position == Vector2.zero ? new Vector2(250 * level, 150 * levelCounts[level]) : node.Position;
            return position;
        }
        
        private void ChangeName(FocusOutEvent evt)
        {
            TextField nodeName = evt.target as TextField;
            switch (Node.GetType().Name)
            {
                case nameof(SwitchNode):
                    if (!RGVIOUtility.RenameNode<SwitchNode>(Name, nodeName.value)) nodeName.value = Name;
                    break;
                case nameof(SimpleAnimationNode):
                    if (!RGVIOUtility.RenameNode<SimpleAnimationNode>(Name, nodeName.value)) nodeName.value = Name;
                    break;
            }
            Name = nodeName.value;
        }

        #region Ports
        private void GeneratePorts()
        {
            inputContainer.Clear();
            outputContainer.Clear();
            if (PreviousNodeView != null)
            {
                // only Switch nodes can have outputs
                // meaning it was the previous node
                var prevSwitchNode = PreviousNodeView.Node as SwitchNode;
                var myIndex = prevSwitchNode.Nodes.ToList().IndexOf(Node);
                var prevOutput = PreviousNodeView.outputContainer[myIndex] as Port;

                var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = PreviousNodeView.Node.name.ToString();
                var edge = inputPort.ConnectTo(prevOutput);

                inputPort.edgeConnector.target.Add(edge);

                inputContainer.Add(inputPort);
            }
            var switchNode = Node as SwitchNode;
            if (switchNode != null)
            {
                foreach (var tempNode in switchNode.Nodes)
                {
                    var outputPort = GeneratePort(Direction.Output, Port.Capacity.Multi);
                    outputPort.portName = (tempNode == null) ? "None" : tempNode.name.ToString();
                    outputContainer.Add(outputPort);
                }
            }
            if (switchNode == null || switchNode.Nodes.Length == 0)
            {
                AddToClassList("end-node");
            }
            RefreshExpandedState();
            RefreshPorts();
        }
        private Port GeneratePort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return this.InstantiatePort(
                Orientation.Horizontal,
                portDirection,
                capacity,
                typeof(float)
            ); //Arbitrary type
        }
        #endregion

        /*public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeUnSelected?.Invoke(this);
        }*/
    }
}
