using System;
using Aarthificial.Reanimation.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class ControlNode : ReanimatorNode
    {
        [SerializeField] protected bool autoIncrement;
        [SerializeField] protected bool percentageBased;
        [SerializeField] [FormerlySerializedAs("driver")] protected string driverName;
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();
        [HideInInspector] [SerializeField] protected string guid = Guid.NewGuid().ToString();
        
        protected string Driver;

        protected virtual void OnEnable()
        {
            Driver = string.IsNullOrEmpty(driverName) ? guid : driverName;
        }

        protected int ResolveDriver(IReadOnlyReanimatorState previousState, ReanimatorState nextState, int size)
        {
            if (size == 0) return 0;
            
            nextState.Merge(drivers);

            if (percentageBased)
            {
                float floatDriverValue = Mathf.Clamp01(previousState.GetFloat(Driver));
                if (floatDriverValue < 1)
                    return Mathf.FloorToInt(floatDriverValue * size);

                return size - 1;
            }

            int driverValue = previousState.Get(Driver) % size;
            if (autoIncrement)
                nextState.Set(Driver, (driverValue + 1) % size);

            return driverValue;
        }
    }
}