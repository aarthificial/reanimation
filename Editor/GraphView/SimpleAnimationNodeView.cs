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
    using UnityEngine.EventSystems;

    public class SimpleAnimationNodeView : ReanimatorNodeView
    {
        public new SimpleAnimationNode Node { get; set; }
        public SimpleAnimationNodeView(SimpleAnimationNode node, int level, ReanimatorNodeView previousNodeView = null) : base(node, level, previousNodeView)
        {
            Node = node;
            StylesSet();
        }

        private void StylesSet()
        {
            textInput.style.color = StylesUtility.Colors.Light;
            titleContainer.style.backgroundColor = StylesUtility.Colors.Dark;
        }
    }
}
