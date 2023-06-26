using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "switch", menuName = "Reanimator/Switch", order = 400)]
    public class SwitchNode : ReanimatorNode
    {
        [SerializeField] protected ReanimatorNode[] nodes = new ReanimatorNode[0];
        public ReanimatorNode[] Nodes { get => nodes; set => nodes = value; }
        [SerializeField] protected ControlDriver controlDriver = new ControlDriver();
        public ControlDriver ControlDriver { get => controlDriver; set => controlDriver = value; }
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