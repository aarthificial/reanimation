using Aarthificial.Reanimation.KeyFrames;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    public class AnimationNode<TKeyFrame> : TerminationNode
        where TKeyFrame : KeyFrame
    {
        public static TNode Create<TNode>(
            bool autoIncrement = false,
            bool percentageBased = false,
            string driver = null,
            TKeyFrame[] frames = null
        ) where TNode : AnimationNode<TKeyFrame>
        {
            var instance = CreateInstance<TNode>();
            instance.autoIncrement = autoIncrement;
            instance.percentageBased = percentageBased;

            if (driver != null)
                instance.driver = driver;
            if (frames != null)
                instance.frames = frames;

            return instance;
        }

        [SerializeField] protected TKeyFrame[] frames;

        public override KeyFrame ResolveKeyframe(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return frames[ProcessDriver(previousState, nextState, frames.Length)];
        }
    }
}