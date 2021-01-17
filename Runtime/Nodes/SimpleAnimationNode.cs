using Aarthificial.Reanimation.Cels;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "simple_animation", menuName = "Reanimator/Simple Animation", order = 400)]
    public class SimpleAnimationNode : AnimationNode<SimpleCel>
    {
    }
}