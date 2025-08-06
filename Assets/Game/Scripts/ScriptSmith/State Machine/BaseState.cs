using System;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    [System.Serializable]
    public abstract class BaseState<TEState> where TEState : Enum
    {
        protected BaseState(TEState stateName)
        {
            StateName = stateName;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();

        public TEState StateName { get; private set; }

        public abstract TEState GetNextState();

        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);


        public abstract void OnDrawGizmos();
    }
}