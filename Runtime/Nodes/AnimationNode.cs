using Aarthificial.Reanimation.Cels;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    public class AnimationNode<TCel> : TerminationNode
        where TCel : ICel
    {
        public static TNode Create<TNode>(
            bool autoIncrement = false,
            bool percentageBased = false,
            string driver = null,
            TCel[] frames = null
        ) where TNode : AnimationNode<TCel>
        {
            var instance = CreateInstance<TNode>();
            instance.autoIncrement = autoIncrement;
            instance.percentageBased = percentageBased;

            if (driver != null)
                instance.Driver = driver;
            if (frames != null)
                instance.frames = frames;

            return instance;
        }

        [SerializeField] protected TCel[] frames;

        public override ICel ResolveCel(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            return frames[ResolveDriver(previousState, nextState, frames.Length)];
        }
    }
}