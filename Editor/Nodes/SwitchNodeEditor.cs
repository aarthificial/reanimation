using Aarthificial.Reanimation.Nodes;
using UnityEditor;

namespace Aarthificial.Reanimation.Editor.Nodes
{
    [CustomEditor(typeof(SwitchNode))]
    public class SwitchNodeEditor : ReanimatorNodeEditor
    {
        protected void OnEnable()
        {
            AddCustomProperty("controlDriver");
            AddCustomProperty("drivers");
            AddCustomProperty("nodes");
        }
    }
}