namespace Aarthificial.Reanimation.Editor.GraphView
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using UnityEditor;
    using Aarthificial.Reanimation.Nodes;

    public class ReanimatorNodeView : Node
    {
        public static Dictionary<int, int> levelCounts = new Dictionary<int, int>();
        public Action<ReanimatorNodeView> OnNodeSelected;
        public Action<ReanimatorNodeView> OnNodeUnSelected;

        public string GUID;
        public ReanimatorNode node;

        public ReanimatorNodeView(ReanimatorNode node, int level)
        {
            this.node = node;
            GUID = Guid.NewGuid().ToString();
            title = node.name;
            if(levelCounts.ContainsKey(level))
                levelCounts[level]++;
            else
                levelCounts[level] = 0;
            RefreshPorts();
            RefreshExpandedState();
            var position = node.position == Vector2.zero ? new Vector2(250 * level, 150 * levelCounts[level]) : node.position;
            SetPosition(new Rect(position, Vector2.zero));
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

        public void UpdateView(ReanimatorNode node)
        {
            this.node = node;
            titleContainer.Q<Label>().text = $"{node.name}";
        }
    }
}
