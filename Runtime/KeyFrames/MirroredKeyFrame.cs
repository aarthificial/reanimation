using System;
using UnityEngine;

namespace Aarthificial.Reanimation.KeyFrames
{
    [Serializable]
    public class MirroredKeyFrame : KeyFrame
    {
        [SerializeField] private Sprite spriteLeft;

        public override void ApplyToRenderer(
            ReanimatorState previousState,
            ReanimatorState nextState,
            SpriteRenderer renderer
        )
        {
            renderer.sprite = previousState.ShouldFlip() ? spriteLeft : sprite;
            renderer.flipX = false;
        }
    }
}