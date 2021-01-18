using System.Collections;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Aarthificial.Reanimation.Tests
{
    public class ReanimatorTestRunner : MonoBehaviour, IMonoBehaviourTest
    {
        protected const string EventDriver = "eventDriver";

        protected Reanimator Reanimator;
        protected SpriteRenderer Renderer;
        protected SimpleAnimationNode Root;
        protected SimpleCel[] Frames;

        public bool IsTestFinished { get; protected set; }

        protected void Awake()
        {
            var drivers = new DriverDictionary();
            drivers.keys.Add(EventDriver);
            drivers.values.Add(0);
            var frame = new SimpleCel(drivers: drivers);
            Frames = new[] {frame};
            Root = SimpleAnimationNode.Create<SimpleAnimationNode>(cels: Frames);

            Renderer = gameObject.AddComponent<SpriteRenderer>();
            Reanimator = gameObject.AddComponent<Reanimator>();
            Reanimator.root = Root;
        }
    }

    public class ReanimatorTests
    {
        [UnityTest]
        public IEnumerator ShouldTriggerTickEvents()
        {
            yield return new MonoBehaviourTest<ShouldTriggerTickEvents>();
        }

        [UnityTest]
        public IEnumerator ShouldRerender()
        {
            yield return new MonoBehaviourTest<ShouldRerender>();
        }

        [UnityTest]
        public IEnumerator ShouldTriggerCustomEvents()
        {
            yield return new MonoBehaviourTest<ShouldTriggerCustomEvents>();
        }

        [UnityTest]
        public IEnumerator ShouldSetFlip()
        {
            yield return new MonoBehaviourTest<ShouldSetFlip>();
        }

        [UnityTest]
        public IEnumerator ShouldSetDriverValues()
        {
            yield return new MonoBehaviourTest<ShouldSetDriverValues>();
        }

        [UnityTest]
        public IEnumerator ShouldAutomaticallyFindRenderer()
        {
            yield return new MonoBehaviourTest<ShouldAutomaticallyFindRenderer>();
        }
    }

    public class ShouldTriggerTickEvents : ReanimatorTestRunner
    {
        private bool _wasEventTriggered;

        private IEnumerator Start()
        {
            Reanimator.Ticked += HandleEvent;
            yield return new WaitForSeconds(1f);
            Reanimator.Ticked -= HandleEvent;

            IsTestFinished = true;
            Assert.IsTrue(_wasEventTriggered);
        }

        private void HandleEvent()
        {
            _wasEventTriggered = true;
        }
    }

    public class ShouldRerender : ReanimatorTestRunner
    {
        private bool _wasEventTriggered;

        private void Start()
        {
            Reanimator.Ticked += HandleEvent;
            Reanimator.ForceRerender();
            Reanimator.Ticked -= HandleEvent;

            IsTestFinished = true;
            Assert.IsTrue(_wasEventTriggered);
        }

        private void HandleEvent()
        {
            _wasEventTriggered = true;
        }
    }

    public class ShouldTriggerCustomEvents : ReanimatorTestRunner
    {
        private bool _wasEventTriggered;

        private IEnumerator Start()
        {
            Reanimator.AddListener(EventDriver, HandleEvent);
            yield return new WaitForSeconds(1f);
            Reanimator.RemoveListener(EventDriver, HandleEvent);

            IsTestFinished = true;
            Assert.IsTrue(_wasEventTriggered);
        }

        private void HandleEvent()
        {
            _wasEventTriggered = true;
        }
    }

    public class ShouldSetFlip : ReanimatorTestRunner
    {
        private void Start()
        {
            Reanimator.Flip = true;

            IsTestFinished = true;
            Assert.IsTrue(Reanimator.State.ShouldFlip());
        }
    }

    public class ShouldSetDriverValues : ReanimatorTestRunner
    {
        private void Start()
        {
            const string driverName = "testDriver";
            const string boolDriverName = "boolTestDriver";
            const string floatDriverName = "floatTestDriver";
            const int driverValue = 1;
            const bool boolDriverValue = true;
            const float floatDriverValue = 17.435f;

            Reanimator.Set(driverName, driverValue);
            Reanimator.Set(floatDriverName, floatDriverValue);
            Reanimator.Set(boolDriverName, boolDriverValue);

            bool willChange = Reanimator.WillChange(driverName, driverValue);
            bool boolWillChange = Reanimator.WillChange(boolDriverName);
            bool floatWillChange = Reanimator.WillChange(floatDriverName);
            
            Reanimator.ForceRerender();
            
            int receivedValue = Reanimator.State.Get(driverName);
            bool receivedBoolValue = Reanimator.State.GetBool(boolDriverName);
            float receivedFloatValue = Reanimator.State.GetFloat(floatDriverName);

            IsTestFinished = true;
            Assert.IsTrue(willChange);
            Assert.IsTrue(boolWillChange);
            Assert.IsTrue(floatWillChange);
            Assert.AreEqual(driverValue, receivedValue);
            Assert.AreEqual(boolDriverValue, receivedBoolValue);
            Assert.AreEqual(floatDriverValue, receivedFloatValue, 0.00001);
        }
    }

    public class ShouldAutomaticallyFindRenderer : ReanimatorTestRunner
    {
        private void Start()
        {
            IsTestFinished = true;
            Assert.AreEqual(Renderer, Reanimator.Renderer);
        }
    }
}