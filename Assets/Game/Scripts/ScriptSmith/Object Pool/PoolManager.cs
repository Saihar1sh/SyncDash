using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    public static class PoolManager
    {
        private static Dictionary<int, List<PoolableObject>> pools = new Dictionary<int, List<PoolableObject>>();

        public static T GetPoolableObject<T>(T poolablePrefab)
            where T : PoolableObject
        {
            return GetPoolableObject<T>(poolablePrefab, Vector3.negativeInfinity, default);
        }
        
        public static T GetPoolableObject<T>(T poolablePrefab, Vector3 position, Quaternion rotation)
            where T : PoolableObject
        {
            int objHash = poolablePrefab.name.GetHashCode();
            PoolableObject usablePoolableObject;
            if (!pools.ContainsKey(objHash))
            {
                pools.Add(objHash, new List<PoolableObject>());
            }

            pools.TryGetValue(objHash, out var poolList);
            var poolObj = poolList.Find(_poolObj => _poolObj.isInUse == false);
            if (poolObj == null)
            {
                if(position != Vector3.negativeInfinity && rotation != default)
                {
                    usablePoolableObject = CreatePoolableObject(poolablePrefab);
                }
                else
                {
                    usablePoolableObject = CreatePoolableObject<T>(poolablePrefab, position, rotation);
                }
                usablePoolableObject.isInUse = true;
            }
            else
            {
                usablePoolableObject = poolObj;
            }

            return usablePoolableObject as T;
        }

        public static void PoolObjects<T>(T poolablePrefab) where T : PoolableObject
        {
            int objHash = poolablePrefab.name.GetHashCode();
            pools.TryGetValue(objHash, out var poolList);
            poolList.ForEach(obj => PoolObject(obj));
        }

        public static void PoolObjects<T>(T poolablePrefab, List<T> poolableList) where T : PoolableObject
        {
            int objHash = poolablePrefab.name.GetHashCode();
            pools.TryGetValue(objHash, out var poolList);
            var usablePoolableList = new List<PoolableObject>();
            foreach (var poolableObject in poolableList) usablePoolableList.Add(poolableObject);
            poolList.FindAll(obj=> usablePoolableList.Contains(obj)).ForEach(obj => PoolObject(obj));
        }

        private static void PoolObject(PoolableObject poolable)
        {
            if (!poolable.InitialInit)
            {
                poolable.Init();
            }

            poolable.isInUse = false;

            poolable.gameObject.SetActive(false);
        }

        private static PoolableObject CreatePoolableObject<T>(T poolablePrefab, Vector3 position, Quaternion rotation)
            where T : PoolableObject
        {
            return GameObject.Instantiate<T>(poolablePrefab,position,rotation);
        }
        private static PoolableObject CreatePoolableObject<T>(T poolablePrefab)
            where T : PoolableObject
        {
            return GameObject.Instantiate<T>(poolablePrefab);
        }
    }
}