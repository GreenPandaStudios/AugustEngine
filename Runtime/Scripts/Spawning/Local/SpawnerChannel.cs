using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AugustEngine.Spawning.Local
{
    /// <summary>
    /// A spawner channel is meant to broadcast spawn and despawn requests for spawner of a single type
    /// </summary>
    [CreateAssetMenu(fileName = "Spawner Channel", menuName = "August Engine/Spawning/Spawner Channel", order = 1)]
    public class SpawnerChannel : ScriptableObject
    {
        private Func<TransformData, SpawnableObject> spawnAction = null;
        private Action<SpawnableObject> despawnAction = delegate { };


        internal void Subscribe(Func<TransformData, SpawnableObject> SpawnAction, Action<SpawnableObject> DespawnAction)
        {
            spawnAction = SpawnAction;
            despawnAction = DespawnAction;
        }

        internal void Unsubscribe(Func<TransformData, SpawnableObject> SpawnAction, Action<SpawnableObject> DespawnAction)
        {
            spawnAction = null;
            despawnAction = null;
        }


        public T Spawn<T>(Vector3 postion, Quaternion rotation = new Quaternion()) where T : SpawnableObject
        {
            if (spawnAction == null)
            {
                return null;
            }
            return (T)spawnAction(new TransformData() { position = postion, rotation = rotation });
        }

        public void Despawn(SpawnableObject spawnableObject)
        {
            if (despawnAction == null)
            {
                return;
            }
            despawnAction(spawnableObject);
        }

        public struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
        }
    }
}