using System;
using System.Collections;
using System.Collections.Generic;
using Aarthificial.Reanimation.Common;

namespace Aarthificial.Reanimation
{
    public interface IReadOnlyReanimatorState
    {
        public void Set(string name, bool value);

        public int Get(string name, int fallback = 0);

        public float GetFloat(string name, float fallback = 0);

        public bool GetBool(string name, bool fallback = false);

        public bool ShouldFlip();
    }

    [Serializable]
    public class ReanimatorState : IReadOnlyReanimatorState, IEnumerable<KeyValuePair<string, int>>
    {
        public static readonly string FlipDriver = "_flip";
        public static readonly int FloatPrecision = 100000;

        private readonly Dictionary<string, int> _drivers = new Dictionary<string, int>();

        public void Set(string name, int value)
        {
            _drivers[name] = value;
        }

        public void Set(string name, float value)
        {
            // TODO I'm not sure about storing floats as ints.
            // For now they serve only as a percentage-like completion value so the loss of precision shouldn't be an issue tho
            // #FamousLastWords
            _drivers[name] = (int) (value * FloatPrecision);
        }

        public void Set(string name, bool value)
        {
            _drivers[name] = value ? 1 : 0;
        }

        public int Get(string name, int fallback = 0)
        {
            return _drivers.ContainsKey(name) ? _drivers[name] : fallback;
        }

        public float GetFloat(string name, float fallback = 0)
        {
            return _drivers.ContainsKey(name) ? (float) _drivers[name] / FloatPrecision : fallback;
        }

        public bool GetBool(string name, bool fallback = false)
        {
            return _drivers.ContainsKey(name) ? _drivers[name] != 0 : fallback;
        }

        public bool Contains(string name)
        {
            return _drivers.ContainsKey(name);
        }

        public void Remove(string name)
        {
            _drivers.Remove(name);
        }

        public bool ShouldFlip()
        {
            return GetBool(FlipDriver);
        }

        public void Clear()
        {
            _drivers.Clear();
        }

        public void Merge(ReanimatorState state)
        {
            foreach (var driver in state)
                _drivers[driver.Key] = driver.Value;
        }

        public void Merge(DriverDictionary drivers)
        {
            for (var i = 0; i < drivers.keys.Count; i++)
                _drivers[drivers.keys[i]] = drivers.values[i];
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return _drivers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}