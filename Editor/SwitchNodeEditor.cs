using Aarthificial.Reanimation.Nodes;
using UnityEditor;

namespace Aarthificial.Reanimation.Editor
{
    [CustomEditor(typeof(SwitchNode))]
    public class SwitchNodeEditor : ControlNodeEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            PropertiesToDraw.Add(serializedObject.FindProperty("nodes"));
        }
    }
}