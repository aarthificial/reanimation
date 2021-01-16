using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

namespace Aarthificial.Reanimation
{
    public delegate void ReanimatorListener();

    public class Reanimator : MonoBehaviour
    {
        public event ReanimatorListener Ticked;
        public SpriteRenderer Renderer => renderer;
        public ReanimatorNode root;
        public IReadOnlyReanimatorState State => _previousState;

        public bool Flip
        {
            set => _previousState.Set(ReanimatorState.FlipDriver, value);
        }

        [SerializeField] private new SpriteRenderer renderer;
        [SerializeField] private float secondsPerFrame = 0.1f;

        private readonly Dictionary<string, ReanimatorListener> _listeners =
            new Dictionary<string, ReanimatorListener>();

        private readonly ReanimatorState _previousState = new ReanimatorState();
        private readonly ReanimatorState _nextState = new ReanimatorState();
        private readonly HashSet<string> _oneOffDrivers = new HashSet<string>();
        private float _clock;

        private void Awake()
        {
            if (renderer == null)
                renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _clock += Time.deltaTime;
            while (_clock >= secondsPerFrame)
            {
                _clock -= secondsPerFrame;
                UpdateFrame();
            }
        }

        private void UpdateFrame()
        {
            _previousState.Merge(_nextState);
            _nextState.Clear();
            root.Resolve(_previousState, _nextState)
                .ResolveKeyframe(_previousState, _nextState)
                .ApplyToRenderer(_previousState, _nextState, renderer);
            foreach (string driver in _oneOffDrivers)
                _previousState.Remove(driver);

            foreach (var listener in _listeners)
                if (_nextState.Contains(listener.Key))
                    listener.Value.Invoke();

            Ticked?.Invoke();
        }

        public void Set(string key, int value)
        {
            _nextState.Set(key, value);
        }

        public void Set(string key, float value)
        {
            _nextState.Set(key, value);
        }

        public void Set(string key, bool value)
        {
            _nextState.Set(key, value);
        }

        public bool WillChange(string key)
        {
            return _nextState.Contains(key) && _nextState.Get(key) != _previousState.Get(key);
        }

        public bool WillChange(string key, int toValue)
        {
            if (!_nextState.Contains(key)) return false;
            int nextValue = _nextState.Get(key);

            return nextValue == toValue && _previousState.Get(key) != nextValue;
        }

        public void ForceRerender()
        {
            _clock = 0;
            UpdateFrame();
        }

        public void AddListener(string driverName, ReanimatorListener listener)
        {
            if (_listeners.ContainsKey(driverName))
                _listeners[driverName] += listener;
            else
                _listeners[driverName] = listener;
        }

        public void RemoveListener(string driverName, ReanimatorListener listener)
        {
            if (!_listeners.ContainsKey(driverName)) return;
            
            _listeners[driverName] -= listener;
            if (_listeners[driverName] == null)
                _listeners.Remove(driverName);
        }

        public void AddOneOffDriver(string driverName)
        {
            _oneOffDrivers.Add(driverName);
        }

        public void RemoveOneOffDriver(string driverName)
        {
            _oneOffDrivers.Remove(driverName);
        }
    }
}