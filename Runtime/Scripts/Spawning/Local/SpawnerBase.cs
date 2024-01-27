namespace AugustEngine.Spawning.Local
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public abstract class SpawnerBase : MonoBehaviour
    {
        //get a refereence to the thing we want to spawn
        [SerializeField] protected SpawnableObject spawnedObject;
        [SerializeField] protected Stack<SpawnableObject> objectPool = new Stack<SpawnableObject>();
        [SerializeField] protected int poolSize = 5;
        [SerializeField] protected SpawnerChannel channel;
        public SpawnerChannel Channel { get => channel; }

        //get an accessor to the spawned Object
        public SpawnableObject SpawnedObject
        {
            get => spawnedObject;

        }

        protected abstract void Load();

        protected SpawnableObject Spawn(SpawnerChannel.TransformData transformData) => Spawn(transformData.position, transformData.rotation);
        protected abstract SpawnableObject Spawn(Vector3 postition, Quaternion rotation);
        protected abstract void Despawn(SpawnableObject spawnable);
        //create an event that is sent out when we spawn the object
        public Action<SpawnableObject> OnSpawn = delegate { };

        //create an event that is sent out when we despawn the object
        public Action<SpawnableObject> OnDespawn = delegate { };

        private void OnEnable()
        {
            channel.Subscribe(Spawn, Despawn);
            if (objectPool.Count == 0) Load();
        }
        private void OnDisable()
        {
            channel.Unsubscribe(Spawn, Despawn);
        }
    }
}

