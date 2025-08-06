using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    public class MonoGenericLazySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //flag to avoid creating singleton in some cases. Like when closing the game
        private static bool _singletonDestroyed = false;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_singletonDestroyed)
                {
                    Debug.LogError("Singleton is already destroyed. Returning null");
                    return null;
                }

                if (_instance) return _instance;

                _instance = FindFirstObjectByType<T>();

                if (!_instance)
                {
                    new GameObject(typeof(T).ToString()).AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_instance && !_singletonDestroyed)
            {
                _instance = this as T;
            }
            else if (_instance != this)
                Destroy(this);
        }

        protected virtual void OnDestroy()
        {
            if (_instance != this)
                return;

            _singletonDestroyed = true;
            _instance = null;
        }
    }
}