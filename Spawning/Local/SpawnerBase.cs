namespace AugustEngine.Spawning.Local
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public abstract class SpawnerBase : MonoBehaviour
    {
        //get a refereence to the thing we want to spawn
        [SerializeField] protected GameObject spawnedObject;

        //get an accessor to the spawned Object
        public GameObject SpawnedObject
        {
            get => spawnedObject;

        }
        public abstract void Load();
        public abstract void Spawn();
        public abstract void Despawn();
        //create an event that is sent out when we spawn the object
        public Action OnSpawn = delegate { };

        //create an event that is sent out when we despawn the object
        public Action OnDespawn = delegate { };
    }
}

