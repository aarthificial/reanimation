using Aarthificial.Reanimation.KeyFrames;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class TerminationNode : ControlNode
    {
        public sealed override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return this;
        }

        public abstract KeyFrame ResolveKeyframe(IReadOnlyReanimatorState previousState, ReanimatorState nextState);
    }
}