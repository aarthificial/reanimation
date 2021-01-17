using System.Linq;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Nodes;
using NUnit.Framework;
using UnityEngine;

namespace Aarthificial.Reanimation.Tests
{
    public class SimpleAnimationNodeTests
    {
        private const string DriverName = "testDriver";

        private readonly ReanimatorState _previousState = new ReanimatorState();
        private readonly ReanimatorState _nextState = new ReanimatorState();
        private SimpleAnimationNode _testedNode;
        private SimpleCel[] _keyframes;
        private ControlDriver _controlDriver;

        [SetUp]
        public void SetUp()
        {
            _controlDriver = new ControlDriver(DriverName, true);
            _keyframes = new int[3].Select(_ => new SimpleCel()).ToArray();
            _testedNode = SimpleAnimationNode.Create<SimpleAnimationNode>(
                _controlDriver,
                _keyframes
            );
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_testedNode);
        }

        [Test]
        public void ShouldResolveToItself()
        {
            _previousState.Clear();
            _nextState.Clear();

            var terminationNode = _testedNode.Resolve(_previousState, _nextState);

            Assert.AreEqual(terminationNode, _testedNode);
        }

        [Test]
        public void ShouldAutoIncrementAnimation()
        {
            _previousState.Clear();
            _nextState.Clear();

            var firstFrame = _testedNode.ResolveCel(_previousState, _nextState);
            _previousState.Merge(_nextState);
            _nextState.Clear();
            var secondFrame = _testedNode.ResolveCel(_previousState, _nextState);

            Assert.AreEqual(firstFrame, _keyframes[0]);
            Assert.AreEqual(secondFrame, _keyframes[1]);
        }

        [Test]
        public void ShouldUseCorrectDriver()
        {
            _previousState.Clear();
            _nextState.Clear();
            _previousState.Set(DriverName, 2);

            var frame = _testedNode.ResolveCel(_previousState, _nextState);

            Assert.AreEqual(frame, _keyframes[2]);
        }
    }
}