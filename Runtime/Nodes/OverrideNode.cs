using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [Serializable]
    public struct OverridePair
    {
        public TerminationNode fromNode;
        public TerminationNode toNode;
    }

    [CreateAssetMenu(fileName = "override", menuName = "Reanimator/Override", order = 400)]
    public class OverrideNode : ReanimatorNode
    {
        [SerializeField] private ReanimatorNode next;
        [SerializeField] private List<OverridePair> overrides = new List<OverridePair>();

        private readonly Dictionary<TerminationNode, TerminationNode> _map =
            new Dictionary<TerminationNode, TerminationNode>();

        private void OnEnable()
        {
            _map.Clear();
            foreach (var pair in overrides)
            {
                if (pair.fromNode == null || pair.toNode == null) continue;
                _map[pair.fromNode] = pair.toNode;
            }
        }

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            var node = next.Resolve(previousState, nextState);
            if (!_map.ContainsKey(node))
                return node;

            var overrideNode = _map[node];
#if UNITY_EDITOR
            nextState.AddTrace(overrideNode);
#endif
            return overrideNode;
        }
    }
}