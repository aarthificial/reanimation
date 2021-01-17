using System;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Cels
{
    [Serializable]
    public class SimpleCel : ICel
    {
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();

        public SimpleCel()
        {
        }

        public SimpleCel(Sprite sprite = null, DriverDictionary drivers = null)
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