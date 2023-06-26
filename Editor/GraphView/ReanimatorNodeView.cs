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

        public ReanimatorNodeView(ReanimatorNode node, int level)
        {
            Level = level;
            Node = node;
            GUID = Guid.NewGuid().ToString();
            Name = node.name;
            SetPosition(new Rect(LevelToPosition(level), Vector2.zero));
            Draw();
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
        public void Draw()
        {
            TextField nodeName = new TextField();
            VisualElement textInput = nodeName.Q("unity-text-input");
            textInput.style.backgroundColor = new UnityEngine.Color(0, 0, 0, 0);
            textInput.style.borderBottomWidth = 0;
            textInput.style.borderLeftWidth = 1;
            textInput.style.borderTopWidth = 0;
            textInput.style.borderRightWidth = 0;
            textInput.style.paddingLeft = 10;
            textInput.style.paddingRight = 10;
            nodeName.value = Node.name;
            nodeName.StretchToParentSize();
            nodeName.style.marginLeft = 30;
            nodeName.style.minWidth = 50;
            titleContainer.Add(nodeName);
            titleContainer.style.minWidth = 50 + 30;


            nodeName.RegisterCallback<FocusOutEvent>(ChangeName);
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
    }
}
