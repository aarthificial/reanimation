using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "switch", menuName = "Reanimator/Switch", order = 400)]
    public class SwitchNode : ControlNode
    {
        public ReanimatorNode[] nodes;

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            return nodes[ResolveDriver(previousState, nextState, nodes.Length)].Resolve(previousState, nextState);
        }
    }
}