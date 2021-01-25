using UnityEngine;

namespace Aarthificial.Reanimation.Cels
{
    public interface ICel
    {
        void ApplyToRenderer(
            ReanimatorState previousState,
            ReanimatorState nextState,
            SpriteRenderer renderer
        );
    }
}
