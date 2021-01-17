using System;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Cels
{
    [Serializable]
    public class MirroredCel : ICel
    {
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();
        [SerializeField] private Sprite spriteLeft;

        public void ApplyToRenderer(
            ReanimatorState previousState,
            ReanimatorState nextState,
            SpriteRenderer renderer
        )
        {
            nextState.Merge(drivers);
            renderer.sprite = previousState.ShouldFlip() ? spriteLeft : sprite;
            renderer.flipX = false;
        }
    }
}