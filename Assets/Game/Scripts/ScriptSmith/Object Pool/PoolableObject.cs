using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    public class PoolableObject : MonoBehaviour
    {
        public bool isInUse;
        public bool InitialInit { get; private set; }

        

        public void OnEnable()
        {
            isInUse = true;
        }
        public void OnDisable()
        {
            isInUse = false;
        }

        public virtual void Init()
        {
            InitialInit = true;
        }
    }
}