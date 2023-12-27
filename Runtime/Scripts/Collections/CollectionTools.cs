namespace AugustEngine.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public static class CollectionTools
    {
        public static IEnumerable<float> AsEnumarable(this Vector3 v)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
        }
        public static IEnumerable<float> AsEnumarable(this Quaternion v)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
            yield return v.w;
        }
        public static IEnumerable<float> AsEnumarable(this Vector4 v)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
            yield return v.w;
        }
        public static T GetRandomMember<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new NullReferenceException();
            return list[UnityEngine.Random.Range(0, list.Count)];

        }
        /// <summary>
        /// Get the desired component as a list recursively visiting ALL childern of a gameobject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="g"></param>
        /// <returns></returns>

        public static List<T> GetComponentsInAllChildren<T>(this GameObject g)
        {
            var l = new List<T>();
            GetComponenetsInChildrenHelper<T>(l, g);
            return l;
        }
        private static void GetComponenetsInChildrenHelper<T>(List<T> list, GameObject g)
        {
            if (g.TryGetComponent<T>(out var t))
            {
                list.Add(t);
            }
            for (int i = 0; i < g.transform.childCount; i++) GetComponenetsInChildrenHelper<T>(list, g.transform.GetChild(i).gameObject);
        }

        public static List<Transform> GetAllChildren(this GameObject g)
        {
            var l = new List<Transform>();
            GetAllnChildrenHelper(l, g);
            return l;
        }
        private static void GetAllnChildrenHelper(List<Transform> list, GameObject g)
        {
            list.Add(g.transform);
            for (int i = 0; i < g.transform.childCount; i++)
            {
                var child = g.transform.GetChild(i);
                list.Add(child);
                GetAllnChildrenHelper(list, child.gameObject);
            }

        }
    }
}