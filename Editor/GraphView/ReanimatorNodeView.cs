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
        #region Properties and Fields
        public static Dictionary<int, List<ReanimatorNodeView>> Grid = new Dictionary<int, List<ReanimatorNodeView>>();
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

        public Vector2Int GridPosition { get; set; }

        public string Name { get; set; }

        public ReanimatorNodeView Parent { get; set; }

        protected TextField nodeName = new TextField();

        protected VisualElement textInput;
        #endregion

        #region Styles
        private void SetStyles()
        {
            if (Parent != null)
                Parent.RegisterCallback<GeometryChangedEvent>(SetPositionOnChanges);

            //Node Name 
            {
                nodeName.AddToClassList("nodeName");
                nodeName.value = Node.name;
                textInput = nodeName.Q("unity-text-input");
                DisableEditName(null);
                nodeName.RegisterCallback<FocusOutEvent>(ChangeName);
                textInput.style.fontSize = 24;
                textInput.style.backgroundColor = StylesUtility.Colors.Transparent;
                StylesUtility.SetBorderWidth(textInput, 0);
                nodeName.style.paddingLeft = 10;
                nodeName.style.paddingRight = 10;
                textInput.style.color = StylesUtility.Colors.Dark;
            }

            //Tittle Container
            {
                titleContainer.Clear(); //removing expand button
                titleContainer.Add(nodeName);
                
            }

            //Node border
            {
                RemoveAt(1); //remove border on hover, then we add our effect
                VisualElement border = this.Q("node-border");
                StylesUtility.SetBorderRadius(border, 14);
                StylesUtility.SetBorderWidth(border, 2);
                StylesUtility.SetBorderColor(border, StylesUtility.Colors.Light);
            }

            //Hover effect
            {
                RegisterCallback((PointerEnterEvent ev) => style.scale = new StyleScale(new Scale(new Vector3(1.05f,1.05f,1.05f))));
                RegisterCallback((PointerOutEvent ev) => style.scale = new StyleScale(new Scale(Vector3.one)));
                RegisterCallback((PointerUpEvent ev) => style.scale = new StyleScale(new Scale(new Vector3(1.05f, 1.05f, 1.05f))));
                RegisterCallback((PointerDownEvent ev) => style.scale = new StyleScale(new Scale(Vector3.one)));
            }
        }

        private void SetStylesToPort(ref Port port)
        {
            port.portName = "";
            port.pickingMode = PickingMode.Ignore;
            port.portColor = StylesUtility.Colors.Light;
            port.style.opacity = 0;
            StylesUtility.SetPadding(port, 0);
            StylesUtility.SetMargin(port, 0);
            port.style.width = 0;
            port.style.top = 5;
            port.style.left = -5;
        }

        private void SetPositionOnChanges(GeometryChangedEvent ev)
        {
            Vector2 pos = LevelToPosition();
            if (pos == GetPosition().position) return;

            SetPosition(new Rect(pos, Vector2.zero));

            //Next Code work because its executes on second frame(or later), when all nodes already generated
            if(Parent.Node is SwitchNode switchNode)
            {
                if (!(Grid.Count > 0 && GridPosition.y - 1 >= 0 
                    && Grid[GridPosition.x][GridPosition.y - 1].Parent != Parent))
                    return;
                int offset = GridPosition.y + (int)((switchNode.Nodes.Count() - 1) / 2);
                offset -= Parent.GridPosition.y;
                offset = offset < 0 ? 0 : offset;
                offset = Parent.GridPosition.y >= GridPosition.y ? 0 : offset;
                foreach (ReanimatorNodeView node in Grid[Parent.GridPosition.x])
                {
                    if (node.GridPosition.y > Parent.GridPosition.y)
                    {
                        node.GridPosition = new Vector2Int(node.GridPosition.x, node.GridPosition.y + offset);
                    }
                }
                Parent.GridPosition = new Vector2Int(Parent.GridPosition.x, offset + Parent.GridPosition.y);
            }
        }

        private Vector2 LevelToPosition()
        {
            var position = new Vector2();
            if(Parent != null)
            {
                position = new Vector2(Parent.GetPosition().x + GetMaxWidth(Grid[Parent.GridPosition.x]) + 100, (Parent.titleContainer.resolvedStyle.height + 50) * GridPosition.y);
            }
            
            return position;
        }
        private float GetMaxWidth(List<ReanimatorNodeView> nodes)
        {
            float maxWidth = (from node in nodes
                             orderby node.titleContainer.resolvedStyle.width
                             select node.titleContainer.resolvedStyle.width).Last();
            return maxWidth;
        }

        #endregion

        public ReanimatorNodeView(ReanimatorNode node, int xPos, ReanimatorNodeView previousNodeView = null)
        {
            GUID = Guid.NewGuid().ToString();
            if (Grid.ContainsKey(xPos))
            {
                Grid[xPos].Add(this);
            }
            else
            {
                Grid[xPos] = new List<ReanimatorNodeView>
                {
                    this
                };
            }
            GridPosition = new Vector2Int(xPos, Grid[xPos].Count - 1);

            Node = node;
            Name = node.name;
            Parent = previousNodeView; 
            SetStyles();
            GeneratePorts();

            RegisterCallback<MouseDownEvent>(EnableEditName);
            textInput.RegisterCallback<FocusOutEvent>(DisableEditName);
        }

        public void UnregisterAllCallbacks()
        {
            if (Parent != null)
                Parent.UnregisterCallback<GeometryChangedEvent>(SetPositionOnChanges);
        }

        public void CreateChildAsset<T>() where T : ReanimatorNode
        {
            var node = ScriptableObject.CreateInstance<T>();
            SwitchNode parentNode = Node as SwitchNode;
            node.name = "New Node";
            List<ReanimatorNode> nodes = parentNode.Nodes.ToList();
            nodes.Add(node);
            parentNode.Nodes = nodes.ToArray();
            RGVIOUtility.CreateNode(GetRootNode(this), node);
        }
        private ReanimatorNode GetRootNode(ReanimatorNodeView nodeView)
        {
            if(nodeView.Parent != null) return GetRootNode(nodeView.Parent);
            return nodeView.Node;
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
            SwitchNode switchNode = Parent.Node as SwitchNode;
            List<ReanimatorNode> nodes = switchNode.Nodes.ToList();
            nodes.Remove(Node);
            switchNode.Nodes = nodes.ToArray();
            OnDetached.Invoke();
        }

        

        public void EnableEditName(MouseDownEvent ev = null)
        {
            if (ev == null || ev.clickCount == 2)
            {
                textInput.pickingMode = PickingMode.Position;
                textInput.Focus();
                textInput.SendEvent(new PointerMoveEvent());
                OnSelected();
            }
            if(ev != null)
            {
                ev.StopPropagation();
            }
                
        }
        public void DisableEditName(FocusOutEvent ev = null)
        {
            
            textInput.pickingMode = PickingMode.Ignore;
        }

        
        
        private void ChangeName(FocusOutEvent evt)
        {
            TextField nodeName = evt.target as TextField;
            switch (Node.GetType().Name)
            {
                case nameof(SwitchNode):
                    if (!RGVIOUtility.RenameNode(GetRootNode(this), Node, nodeName.value)) nodeName.value = Name;
                    break;
                case nameof(SimpleAnimationNode):
                    if (!RGVIOUtility.RenameNode(GetRootNode(this), Node, nodeName.value)) nodeName.value = Name;
                    break;
            }
            Name = nodeName.value;
        }

        #region Ports
        private void GeneratePorts()
        {
            inputContainer.Clear();
            outputContainer.Clear();
            if (Parent != null)
            {
                // only Switch nodes can have outputs
                // meaning it was the previous node
                var prevOutput = Parent.titleContainer.Children().Last() as Port;

                var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
                var edge = inputPort.ConnectTo(prevOutput);
                edge.pickingMode = PickingMode.Ignore;
                inputPort.edgeConnector.target.Add(edge);
                SetStylesToPort(ref inputPort);
                titleContainer.Insert(0, inputPort);
            }
            var switchNode = Node as SwitchNode;
            if (switchNode != null)
            {
                foreach (var tempNode in switchNode.Nodes)
                {
                    var outputPort = GeneratePort(Direction.Output, Port.Capacity.Multi);
                    SetStylesToPort(ref outputPort);
                    titleContainer.Add(outputPort);
                }
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
