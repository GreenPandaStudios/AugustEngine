namespace AugustEngine.LowLevel
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Singleton implementation
    /// </summary>
    /// <typeparam name="T">type of singelton</typeparam>
    public class Singleton<T> : MonoBehaviour where T : UnityEngine.Component
    {

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //intialize a new one
                    var g = new GameObject(typeof(T).ToString() + "_singelton");
                    DontDestroyOnLoad(g);
                    instance = g.AddComponent<T>();
                }
                return instance;
            }
        }
        private static T instance = null;
    }
}


