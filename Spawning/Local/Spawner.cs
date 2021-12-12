namespace AugustEngine.Spawning.Local
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class Spawner : SpawnerBase
    {


        public override void Load()
        {
            //load the object into memory
            spawnedObject = Instantiate(spawnedObject, transform.position, Quaternion.identity, null);
            spawnedObject.SetActive(false);
        }

        /// <summary>
        /// Spawns an instantiated game object
        /// </summary>
        public override void Spawn()
        {
            //(re)activate the object and move it back to the spawner
            spawnedObject.transform.position = transform.position;
            spawnedObject.transform.rotation = Quaternion.identity;

            spawnedObject.SetActive(true);

            //send out the OnSpawn event if it is not empty
            OnSpawn?.Invoke();
        }
        /// <summary>
        /// despawns an instantiated game object
        /// </summary>
        public override void Despawn()
        {
            //deactivate the object and send out the despawn event
            spawnedObject.SetActive(false);

            OnDespawn?.Invoke();
        }
    }
}
