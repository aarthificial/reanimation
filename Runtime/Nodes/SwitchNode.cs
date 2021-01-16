using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "Switch", menuName = "Reanimator/Switch", order = 0)]
    public class SwitchNode : ControlNode
    {
        public ReanimatorNode[] nodes;

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return nodes[ProcessDriver(previousState, nextState, nodes.Length)].Resolve(previousState, nextState);
        }
    }
}