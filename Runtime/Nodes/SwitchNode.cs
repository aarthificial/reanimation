using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "switch", menuName = "Reanimator/Switch", order = 400)]
    public class SwitchNode : ReanimatorNode
    {
        [SerializeField] protected ReanimatorNode[] nodes;
        [SerializeField] protected ControlDriver controlDriver = new ControlDriver();
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            nextState.Merge(drivers);
            return nodes[controlDriver.ResolveDriver(previousState, nextState, nodes.Length)]
                .Resolve(previousState, nextState);
        }
    }
}