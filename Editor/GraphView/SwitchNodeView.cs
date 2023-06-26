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

    public class SwitchNodeView : ReanimatorNodeView
    {
        private ReanimatorNode[] reanimatorNodes;
        public new SwitchNode Node { get; set; }
        public SwitchNodeView(SwitchNode node, int level) : base(node, level)
        {
            Node = node;
            Update();
        }
        public bool NodesChanged()
        {
            if (reanimatorNodes != Node.Nodes) return true;
            return false;
        }
        public void Update()
        {
            reanimatorNodes = Node.Nodes;
        }


        /*public List<ReanimatorNode> GetDeletedNodes()
        {
            List<ReanimatorNode> nodes = new List<ReanimatorNode>();
            foreach (var node in reanimatorNodes)
            {
                if(!Node.Nodes.Contains(node)) nodes.Add(node);
            }
            return nodes;
        }
        public List<ReanimatorNode> GetAddedNodes()
        {
            List<ReanimatorNode> nodes = reanimatorNodes.ToList();
            foreach(var node in Node.Nodes)
            {
                nodes.Remove(node);
            }
            return nodes;
        }*/
    }
}
