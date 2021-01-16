using System;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class ControlNode : ReanimatorNode
    {
        [SerializeField] protected bool autoIncrement;
        [SerializeField] protected bool percentageBased;
        [SerializeField] protected string driver;
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();
        [HideInInspector] [SerializeField] protected string guid = Guid.NewGuid().ToString();

        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(driver))
                driver = guid;
        }

        protected int ProcessDriver(IReadOnlyReanimatorState previousState, ReanimatorState nextState, int size)
        {
            nextState.Merge(drivers);

            if (percentageBased)
            {
                float floatDriverValue = Mathf.Clamp01(previousState.GetFloat(driver));
                if (floatDriverValue < 1)
                    return Mathf.FloorToInt(floatDriverValue * size);

                return size - 1;
            }

            int driverValue = previousState.Get(driver) % size;
            if (autoIncrement)
                nextState.Set(driver, (driverValue + 1) % size);

            return driverValue;
        }
    }
}