using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    public class AnimationNode<TCel> : TerminationNode
        where TCel : ICel
    {
        public static TNode Create<TNode>(
            ControlDriver driver = null,
            TCel[] cels = null
        ) where TNode : AnimationNode<TCel>
        {
            var instance = CreateInstance<TNode>();

            if (driver != null)
                instance.controlDriver = driver;
            if (cels != null)
                instance.cels = cels;

            return instance;
        }

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once InconsistentNaming
        [SerializeField] protected TCel[] cels;
        [SerializeField] protected ControlDriver controlDriver = new ControlDriver();
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            nextState.Merge(drivers);
            return this;
        }

        public override ICel ResolveCel(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            nextState.Merge(drivers);
            return cels[controlDriver.ResolveDriver(previousState, nextState, cels.Length)];
        }
    }
}