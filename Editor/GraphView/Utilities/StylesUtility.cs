using Aarthificial.Reanimation.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.Editor.GraphView
{
    public static class StylesUtility
    {
        public static class Colors
        {
            public static Color Dark { get; private set; } = new Color(0.1254f, 0.1254f, 0.1254f);
            public static Color Light { get; private set; } = new Color(0.9922f, 1, 0.9608f);
            public static Color Transparent { get; private set; } = new Color(0,0,0,0);
        }
        

        public static void SetBorderWidth(VisualElement visualElement, float width)
        {
            visualElement.style.borderBottomWidth = width;
            visualElement.style.borderLeftWidth = width;
            visualElement.style.borderRightWidth = width;
            visualElement.style.borderTopWidth = width;
        }
        public static void SetBorderColor(VisualElement visualElement, UnityEngine.Color color)
        {
            visualElement.style.borderBottomColor = color;
            visualElement.style.borderLeftColor = color;
            visualElement.style.borderRightColor = color;
            visualElement.style.borderTopColor = color;
        }
        public static void SetBorderRadius(VisualElement visualElement, float radius)
        {
            visualElement.style.borderTopLeftRadius = radius;
            visualElement.style.borderTopRightRadius = radius;
            visualElement.style.borderBottomRightRadius = radius;
            visualElement.style.borderBottomLeftRadius = radius;
        }
        public static void SetPadding(VisualElement visualElement, float padding)
        {
            visualElement.style.paddingBottom = padding;
            visualElement.style.paddingLeft = padding;
            visualElement.style.paddingRight = padding;
            visualElement.style.paddingTop = padding;
        }
        public static void SetMargin(VisualElement visualElement, float margin)
        {
            visualElement.style.marginBottom = margin;
            visualElement.style.marginLeft = margin;
            visualElement.style.marginRight = margin;
            visualElement.style.marginTop = margin;
        }
    }
}