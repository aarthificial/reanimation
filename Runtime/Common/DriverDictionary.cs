using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Reanimation.Common
{
    [Serializable]
    public class DriverDictionary
    {
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<int> values = new List<int>();
    }
}