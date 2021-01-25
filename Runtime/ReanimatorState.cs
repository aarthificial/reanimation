using System;
using System.Collections;
using System.Collections.Generic;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;

namespace Aarthificial.Reanimation
{
    public interface IReadOnlyReanimatorState : IEnumerable<KeyValuePair<string, int>>
    {
        void Set(string name, bool value);

        int Get(string name, int fallback = 0);

        float GetFloat(string name, float fallback = 0);

        bool GetBool(string name, bool fallback = false);

        bool ShouldFlip();
    }

    [Serializable]
    public class ReanimatorState : IReadOnlyReanimatorState
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
#if UNITY_EDITOR
            _trace.Clear();
#endif
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

#if UNITY_EDITOR
        private readonly List<ReanimatorNode> _trace = new List<ReanimatorNode>();
        public ReanimatorNode LastTracedNode => _trace[_trace.Count - 1];

        public void AddTrace(ReanimatorNode value)
        {
            _trace.Add(value);
        }
#endif
    }
}
