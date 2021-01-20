using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

namespace Aarthificial.Reanimation
{
    public delegate void ReanimatorListener();

    public class Reanimator : MonoBehaviour
    {
        /// <summary>
        /// Event triggered each time the graph is resolved.
        /// 
        /// Frequency depends on the <see cref="Reanimator.fps">framerate</see>.
        /// </summary>
        public event ReanimatorListener Ticked;

        /// <summary>
        /// SpriteRenderer being used to display the animation.
        /// </summary>
        public SpriteRenderer Renderer => renderer;

        /// <summary>
        /// State used during the previous resolution.
        /// 
        /// Should not be modified.
        /// </summary>
        public IReadOnlyReanimatorState State => _previousState;

        /// <summary>
        /// State that will be used during the next resolution.
        /// 
        /// Should not be modified directly.<br/>
        /// Use the <see cref="Reanimator.Set(string, int)">Set() method</see> instead
        /// </summary>
        public IReadOnlyReanimatorState NextState => _nextState;

        /// <summary>
        /// Shortcut for setting the <see cref="ReanimatorState.FlipDriver"/>
        /// </summary>
        public bool Flip
        {
            set => _previousState.Set(ReanimatorState.FlipDriver, value);
        }

        /// <summary>
        /// Root of the graph being used to resolve cels.
        /// </summary>
        [Tooltip("Root of the graph being used to resolve cels.")]
        public ReanimatorNode root;

        [Tooltip("SpriteRenderer being used to display the animation.")] 
        [SerializeField]
        private new SpriteRenderer renderer;

        [Tooltip("Framerate of the displayed animation.")] 
        [SerializeField]
        private int fps = 12;

        [Tooltip("Drivers marked as temporary are removed from the state if they were not set during the previous resolution.")]
        [SerializeField]
        private string[] temporaryDrivers = new string[0];

        private readonly Dictionary<string, ReanimatorListener> _listeners =
            new Dictionary<string, ReanimatorListener>();

        private readonly ReanimatorState _previousState = new ReanimatorState();
        private readonly ReanimatorState _nextState = new ReanimatorState();
        private readonly HashSet<string> _temporaryDrivers = new HashSet<string>();
        private float _clock;

        private void Awake()
        {
            if (renderer == null)
                renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            AddTemporaryDriver(temporaryDrivers);
        }

        private void OnDisable()
        {
            RemoveTemporaryDriver(temporaryDrivers);
        }

        private void Update()
        {
            _clock += Time.deltaTime;
            float secondsPerFrame = 1 / (float) fps;
            while (_clock >= secondsPerFrame)
            {
                _clock -= secondsPerFrame;
                UpdateFrame();
            }
        }

        private void UpdateFrame()
        {
            foreach (string driver in _temporaryDrivers)
                _previousState.Remove(driver);
            _previousState.Merge(_nextState);
            _nextState.Clear();

#if UNITY_EDITOR
            try
            {
                root
                    .Resolve(_previousState, _nextState)
                    .ResolveCel(_previousState, _nextState)
                    .ApplyToRenderer(_previousState, _nextState, renderer);
            }
            catch (Exception e)
            {
                Debug.LogException(e, _nextState.LastTracedNode);
            }
#else
            root
                .Resolve(_previousState, _nextState)
                .ResolveCel(_previousState, _nextState)
                .ApplyToRenderer(_previousState, _nextState, renderer);
#endif

            foreach (var listener in _listeners)
                if (_nextState.Contains(listener.Key))
                    listener.Value.Invoke();

            Ticked?.Invoke();
        }

        /// <summary>
        /// Modify driver in the next state.
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <param name="value">New value</param>
        public void Set(string key, int value)
        {
            _nextState.Set(key, value);
        }

        /// <summary>
        /// Modify driver in the next state.
        ///
        /// Used in conjunction with <see cref="ControlDriver.percentageBased"/>
        /// to control the completion of animation using a percentage-based value:
        /// <code>
        /// 0.0f = 0%
        /// 0.5f = 50%
        /// 1.0f = 100%
        /// </code>
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <param name="value">New value</param>
        public void Set(string key, float value)
        {
            _nextState.Set(key, value);
        }

        /// <summary>
        /// Modify driver in the next state.
        ///
        /// Shortcut for:
        /// <code>value ? 1 : 0</code>
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <param name="value">New value</param>
        public void Set(string key, bool value)
        {
            _nextState.Set(key, value);
        }

        /// <summary>
        /// Return true if the value of the driver will change during the next resolution. 
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <returns></returns>
        public bool WillChange(string key)
        {
            return _nextState.Contains(key) && _nextState.Get(key) != _previousState.Get(key);
        }

        /// <summary>
        /// Return true if the value of the driver will change to the specified value during the next resolution. 
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <param name="toValue">Expected new value</param>
        /// <returns></returns>
        public bool WillChange(string key, int toValue)
        {
            if (!_nextState.Contains(key)) return false;
            int nextValue = _nextState.Get(key);

            return nextValue == toValue && _previousState.Get(key) != nextValue;
        }

        /// <summary>
        /// Return true if the value of the driver will change to the specified value during the next resolution.
        ///
        /// Shortcut for:
        /// <code>toValue ? 1 : 0</code>
        /// </summary>
        /// <param name="key">Name of the driver</param>
        /// <param name="toValue">Expected new value</param>
        /// <returns></returns>
        public bool WillChange(string key, bool toValue)
        {
            if (!_nextState.Contains(key)) return false;
            bool nextValue = _nextState.GetBool(key);

            return nextValue == toValue && _previousState.GetBool(key) != nextValue;
        }

        /// <summary>
        /// Immediately force the next resolution process to happen.
        /// </summary>
        public void ForceRerender()
        {
            _clock = 0;
            UpdateFrame();
        }

        /// <summary>
        /// Add listener for the given driver.
        ///
        /// Listeners are invoked whenever the driver is set during a resolution process.<br/>
        /// The value does not matter. 
        /// </summary>
        /// <param name="driverName"></param>
        /// <param name="listener"></param>
        public void AddListener(string driverName, ReanimatorListener listener)
        {
            if (_listeners.ContainsKey(driverName))
                _listeners[driverName] += listener;
            else
                _listeners[driverName] = listener;
        }

        /// <summary>
        /// Remove listener for the given driver.
        /// </summary>
        /// <param name="driverName"></param>
        /// <param name="listener"></param>
        public void RemoveListener(string driverName, ReanimatorListener listener)
        {
            if (!_listeners.ContainsKey(driverName)) return;

            _listeners[driverName] -= listener;
            if (_listeners[driverName] == null)
                _listeners.Remove(driverName);
        }

        /// <summary>
        /// Define additional temporary drivers.
        /// </summary>
        /// <param name="driverName"></param>
        public void AddTemporaryDriver(params string[] driverName)
        {
            _temporaryDrivers.UnionWith(driverName);
        }

        /// <summary>
        /// Remove temporary drivers.
        /// </summary>
        /// <param name="driverName"></param>
        public void RemoveTemporaryDriver(params string[] driverName)
        {
            _temporaryDrivers.ExceptWith(driverName);
        }
    }
}