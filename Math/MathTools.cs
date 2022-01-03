namespace AugustEngine.Math
{


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.Vectors;
    public static class MathTools
    {
        /// <summary>
        /// returns the mean of all the values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Mean(this float[] values)
        {
            return values.Sum() / (float)values.Length;
        }
        public static float Mean(this IEnumerable<float> values)
        {
            //get the length first
            var _length = 0f;
            foreach (float f in values) _length++;
            return values.Sum() * _length;
        }
        /// <summary>
        /// returns the sum of all the provided values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Sum(this IEnumerable<float> values)
        {


            float sum = 0f;
            foreach (float f in values)
            {
                sum += f;
            }
            return sum;
        }
        public static bool FuzzyEquals(this float f1, float f2, float tol)
        {
            return (Mathf.Abs(f1 - f2) < tol);

        }

        public static bool FuzzyEquals(this Vector3 _v1, Vector3 _v2, float tol)
        {
            return (_v1.x.FuzzyEquals(_v2.x, tol) && _v1.y.FuzzyEquals(_v2.y, tol) && _v1.z.FuzzyEquals(_v2.z, tol));
        }
        public static bool FuzzyEquals(this Quaternion _q1, Quaternion _q2, float tol)
        {
            return (_q1.x.FuzzyEquals(_q2.x, tol) && _q1.y.FuzzyEquals(_q2.y, tol) && _q1.z.FuzzyEquals(_q2.z, tol) &&
               _q1.w.FuzzyEquals(_q2.w, tol));
        }

    }
}
