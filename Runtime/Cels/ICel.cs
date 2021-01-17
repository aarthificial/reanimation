using UnityEngine;

namespace Aarthificial.Reanimation.Cels
{
    public interface ICel
    {
        public void ApplyToRenderer(
            ReanimatorState previousState,
            ReanimatorState nextState,
            SpriteRenderer renderer
        );
    }
}