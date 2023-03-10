﻿using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class ReanimatorNode : ScriptableObject
    {
        public Vector2 position;
        public abstract TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState);

        protected void AddTrace(ReanimatorState nextState)
        {
#if UNITY_EDITOR
            nextState.AddTrace(this);
#endif
        }
    }
}