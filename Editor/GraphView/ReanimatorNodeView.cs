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
    using UnityEngine.UI;

    public class ReanimatorNodeView : Node
    {
        public static Dictionary<int, int> levelCounts = new Dictionary<int, int>();
        public Action<ReanimatorNodeView> OnNodeSelected = delegate { };
        public Action<ReanimatorNodeView> OnNodeUnSelected = delegate { };
        public Action OnDetached = delegate { };

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

        private TextField nodeName = new TextField();
        private VisualElement textInput;

        public ReanimatorNodeView(ReanimatorNode node, int level, ReanimatorNodeView previousNodeView = null)
        {
            Level = level;
            Node = node;
            GUID = Guid.NewGuid().ToString();
            Name = node.name;
            PreviousNodeView = previousNodeView;
            SetPosition(new Rect(LevelToPosition(level), Vector2.zero));
            RegisterCallback<PointerDownEvent>(EnableEditName);
            nodeName.RegisterCallback<PointerOutEvent>(DisableEditName);
            TuneUIChildrens();
            GeneratePorts();
        }
        
        ~ReanimatorNodeView()
        {
            nodeName.UnregisterCallback<FocusOutEvent>(ChangeName);
            UnregisterCallback<PointerDownEvent>(EnableEditName);
            nodeName.UnregisterCallback<PointerOutEvent>(DisableEditName);
        }

        public void CreateChildAsset<T>() where T : ReanimatorNode
        {
            var node = ScriptableObject.CreateInstance<T>();
            SwitchNode parentNode = Node as SwitchNode;
            node.name = "New Node";
            List<ReanimatorNode> nodes = parentNode.Nodes.ToList();
            nodes.Add(node);
            parentNode.Nodes = nodes.ToArray();
            RGVIOUtility.SaveNode<T>(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeUnSelected?.Invoke(this);
        }

        public void DetachFromParent()
        {
            SwitchNode switchNode = PreviousNodeView.Node as SwitchNode;
            List<ReanimatorNode> nodes = switchNode.Nodes.ToList();
            nodes.Remove(Node);
            switchNode.Nodes = nodes.ToArray();
            OnDetached.Invoke();
        }

        private void TuneUIChildrens()
        {
            nodeName.AddToClassList("nodeName");
            nodeName.value = Node.name;
            textInput = nodeName.Q("unity-text-input");
            DisableEditName(null);
            titleContainer.Clear(); //removing expand button
            titleContainer.Add(nodeName);
            nodeName.RegisterCallback<FocusOutEvent>(ChangeName);
        }

        public void EnableEditName(PointerDownEvent ev = null)
        {
            if (ev == null || ev.clickCount == 2)
            {
                textInput.pickingMode = PickingMode.Position;
                textInput.Focus();
            }
        }
        public void DisableEditName(PointerOutEvent ev = null)
        {
            textInput.pickingMode = PickingMode.Ignore;
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
                    if (!RGVIOUtility.RenameNode<SwitchNode>(Node, nodeName.value)) nodeName.value = Name;
                    break;
                case nameof(SimpleAnimationNode):
                    if (!RGVIOUtility.RenameNode<SimpleAnimationNode>(Node, nodeName.value)) nodeName.value = Name;
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
                inputPort.pickingMode = PickingMode.Ignore;
                var edge = inputPort.ConnectTo(prevOutput);
                edge.pickingMode = PickingMode.Ignore;
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
                    outputPort.pickingMode = PickingMode.Ignore;
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
    }
}
