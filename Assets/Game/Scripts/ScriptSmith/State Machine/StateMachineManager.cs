using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    public abstract class StateMachineManager<TEState> : MonoBehaviour where TEState : System.Enum
    {
        protected SerializableDictionary<TEState, BaseState<TEState>> States =
            new SerializableDictionary<TEState, BaseState<TEState>>();

        protected BaseState<TEState> CurrentState;

        public TEState StateName => CurrentState.StateName;

        private bool isTransitioningState = false;

        private void Start()
        {
            CurrentState.OnEnter();
        }

        private void Update()
        {
            if (isTransitioningState)
                return;
            TEState nextStateName = CurrentState.GetNextState();
            if (!nextStateName.Equals(CurrentState.StateName))
            {
                CurrentState.OnUpdate();
            }
            else
            {
                TransitionToState(nextStateName);
            }
        }

        private void TransitionToState(TEState nextStateName)
        {
            isTransitioningState = true;
            CurrentState.OnExit();
            CurrentState = States[nextStateName];
            CurrentState.OnEnter();
            isTransitioningState = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        private void OnDrawGizmos()
        {
            CurrentState?.OnDrawGizmos();
        }
    }
}