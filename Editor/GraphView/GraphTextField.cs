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
    using UnityEngine.EventSystems;

    public class GraphTextField
    {
        public TextField TextField { get; set; }
        public Label Label { get; set; }
        public GraphTextField(TextField textField, Label label) 
        {
            TextField = textField;
            Label = label;
            HideTextField();
        }
        private void HideTextField()
        {
            TextField.style.opacity = 0;
        }
    }
}