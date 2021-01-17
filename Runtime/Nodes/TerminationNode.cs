using Aarthificial.Reanimation.Cels;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class TerminationNode : ControlNode
    {
        public sealed override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return this;
        }

        public virtual ICel ResolveCel(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            return new InvalidCel();
        }
    }
}