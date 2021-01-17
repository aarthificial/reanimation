using Aarthificial.Reanimation.Cels;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class TerminationNode : ReanimatorNode
    {
        public abstract ICel ResolveCel(IReadOnlyReanimatorState previousState, ReanimatorState nextState);
    }
}