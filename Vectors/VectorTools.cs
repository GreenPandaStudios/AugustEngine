namespace AugustEngine.Vectors
{


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class VectorTools
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
        public static Vector2 RemoveY(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        /// <summary>
        /// creates a new random vector between the x and y components of the provided
        /// </summary>
        /// <param name="v"></param>
        public static Vector2 RandomV2(this Vector2 v)
        {
            return new Vector2(UnityEngine.Random.Range(v.x, v.y),
                UnityEngine.Random.Range(v.x, v.y));
        }
        /// <summary>
        /// creates a new random vector between the x and y components of the provided
        /// </summary>
        /// <param name="v"></param>
        public static Vector3 RandomV3(this Vector2 v)
        {
            return new Vector3(UnityEngine.Random.Range(v.x, v.y),
                UnityEngine.Random.Range(v.x, v.y),
               UnityEngine.Random.Range(v.x, v.y));
        }
        public static Vector3 ToXZPlane(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }
        public static Color AsColor(this Vector3 v)
        {
            return new Color(v.x, v.y, v.z);
        }
        public static Vector3 AsVector(this Color c)
        {
            return new Vector3(c.r, c.g, c.b);
        }
        /// <summary>
        /// Returns the vector 3 in world space in
        /// terms of the provided transform
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3 ToLocalSpace(this Vector3 v3, Transform localSpace)
        {
            return (v3.x * localSpace.right + v3.y * localSpace.up + v3.z * localSpace.forward);
        }
        /// <summary>
        /// Returns the vector 2 in world space in
        /// terms of the provided transform
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3 ToWorldSpace(this Vector2 v2, Transform localSpace)
        {


            return (v2.x * localSpace.right + v2.y * localSpace.forward);
        }

        /// <summary>
        /// The vector from this vector to the provided
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 To(this Vector3 v3, Vector3 to)
        {
            return to - v3;
        }
        /// <summary>
        /// The vector to this vector from the provided
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 From(this Vector3 v3, Vector3 from)
        {
            return v3 - from;
        }
    }
}
