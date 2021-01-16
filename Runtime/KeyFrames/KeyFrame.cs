using System;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.KeyFrames
{
    [Serializable]
    public class KeyFrame
    {
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();

        public KeyFrame()
        {
        }

        public KeyFrame(Sprite sprite = null, DriverDictionary drivers = null)
        {
            if (sprite != null)
                this.sprite = sprite;
            if (drivers != null)
                this.drivers = drivers;
        }

        public virtual void ApplyToRenderer(
            ReanimatorState previousState,
            ReanimatorState nextState,
            SpriteRenderer renderer
        )
        {
            nextState.Merge(drivers);
            renderer.sprite = sprite;
            renderer.flipX = previousState.ShouldFlip();
        }
    }
}