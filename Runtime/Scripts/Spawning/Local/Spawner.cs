namespace AugustEngine.Spawning.Local
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class Spawner : SpawnerBase
    {
        [SerializeField] ParticleSystem despawnParticles;
        protected override void Load()
        {
            // load all of the objects into memory
            for (int i = 0; i < poolSize; i++)
            {
                SpawnableObject _spawnedObject = Instantiate(spawnedObject.Prefab, transform.position, Quaternion.identity, transform).GetComponent<SpawnableObject>();
                _spawnedObject.gameObject.SetActive(false);
                _spawnedObject.gameObject.transform.position = Vector3.zero;
                _spawnedObject.gameObject.transform.rotation = Quaternion.identity;
                _spawnedObject.Spawner = this;
                objectPool.Push(_spawnedObject);
               
            }
        }

        /// <summary>
        /// Spawns an instantiated game object
        /// </summary>
        protected override SpawnableObject Spawn(Vector3 postion, Quaternion rotation = new Quaternion())
        {
            // See if we need to instantiate more
            if (objectPool.Count == 0)
            {
                Load();
            }

            SpawnableObject spawnable = objectPool.Pop();



            //(re)activate the object and move it back to the spawner
            spawnable.gameObject.transform.position = postion;
            spawnable.gameObject.transform.rotation = rotation;

            spawnable.gameObject.SetActive(true);

            //send out the OnSpawn event if it is not empty
            OnSpawn?.Invoke(spawnable);
            return spawnable;
        }

        /// <summary>
        /// despawns an instantiated game object
        /// </summary>
        protected override void Despawn(SpawnableObject spawnable)
        {
            // deactivate the object and send out the despawn event
            spawnable.gameObject.SetActive(false);

            OnDespawn?.Invoke(spawnable);

            if (despawnParticles != null)
            {
                despawnParticles.transform.position = spawnable.transform.position;
                despawnParticles.Play();
            }

            spawnable.gameObject.transform.position = Vector3.zero;
            spawnable.gameObject.transform.rotation = Quaternion.identity;

            // Just in case we got mixed up
            spawnable.Spawner = this;

           
        }
    }
}
