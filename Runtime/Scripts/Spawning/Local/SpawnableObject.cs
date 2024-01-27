using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace AugustEngine.Spawning.Local
{
    public class SpawnableObject : MonoBehaviour
    {
        public Spawner Spawner = null;
        public GameObject Prefab
        {
            get => gameObject;
        }
        public void Despawn()
        {
            if (Spawner != null)
            {
                Spawner.Channel.Despawn(this);
            }
        }
    }
}