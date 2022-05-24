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
        public static Vector2 RemoveX(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }
        public static Vector2 RemoveZ(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        /// <summary>
        /// Clamps a vector to the maximum provided magnitude
        /// </summary>
        /// <param name="v"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public static Vector2 ClampMagnitude(this Vector2 v, float magnitude)
        {
            if (v.magnitude <= magnitude) return v;
            return v.normalized * magnitude;
        }
        /// <summary>
        /// Clamps a vector to the maximum provided magnitude
        /// </summary>
        /// <param name="v"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public static Vector3 ClampMagnitude(this Vector3 v, float magnitude)
        {
            if (v.magnitude <= magnitude) return v;
            return v.normalized * magnitude;
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
        public static Vector3 ToLocalCoordinates(this Vector3 v3, Transform localSpace)
        {
            return localSpace.InverseTransformPoint(v3);

        }
        /// <summary>
        /// Returns the vector 3 in world space in
        /// terms of the provided transform
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3 ToLocalDirection(this Vector3 v3, Transform localSpace)
        {

            return localSpace.InverseTransformDirection(v3);
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
        /// <summary>
        /// Returns the vector 3 as a direction according to the cameraSpace
        /// As projected on the up plane
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3 ToCameraSpace(this Vector3 v3, Transform cameraSpace)
        {
            return  v3.x * Vector3.ProjectOnPlane(cameraSpace.right, Vector3.up).normalized +
                    v3.y * Vector3.up +
                     v3.z * Vector3.ProjectOnPlane(cameraSpace.forward, Vector3.up).normalized
                    ;
        }
    }

   /// <summary>
   /// A double precision Vector3
   /// </summary>
    public class Vector3Double
    {
       
        public double x, y, z;
        public Vector3Double(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
           
        }
        public override int GetHashCode()
        {
            return (int) Procedural.NoiseGeneration.SquirrelNoise((int)x + (int)y * (int)y * ((int)z + 31) * (int)x,
                (long)x+(long)y+(long)z);
        }
        public static Vector3Double Zero
        {
            get=> new Vector3Double(0,0,0);
        }
        public static Vector3Double One
        {
            get => new Vector3Double(1, 1, 1);
        }
        public static Vector3Double Up
        {
            get => new Vector3Double(1, 0, 0);
        }
        public static Vector3Double Right
        {
            get => new Vector3Double(1, 0, 0);
        }
        public static Vector3Double Forward
        {
            get => new Vector3Double(0, 0, 1);
        }

        /// <summary>
        /// Returs the squared magnitude of the vector
        /// </summary>
        public double SqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }

        }
        /// <summary>
        /// Returs the  magnitude of the vector
        /// </summary>
        public double Magnitude
        {
            get
            {
                return System.Math.Sqrt(SqrMagnitude);
            }

        }

        /// <summary>
        /// Returs the  magnitude of the vector
        /// </summary>
        public Vector3Double Normalized
        {
            get
            {
                return this / Magnitude;
            }

        }
        /// <summary>
        /// Returns the dot product of the two vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Dot(Vector3Double v1, Vector3Double v2)
        {
            return  v1.x * v2.x + 
                    v1.y * v2.y + 
                    v1.z * v2.z;
        }
        //OPERATOR OVERLOADS
        public static Vector3Double operator *(Vector3Double v1, double v2)
        {
            return new Vector3Double(v1.x * v2, v1.y * v2, v1.z * v2);
        }
        public static Vector3Double operator /(Vector3Double v1, double v2)
        {
            return new Vector3Double(v1.x / v2, v1.y / v2, v1.z / v2);
        }
        public static Vector3Double operator *(double v2, Vector3Double v1)
        {
            return new Vector3Double(v1.x * v2, v1.y * v2, v1.z * v2);
        }
        public static Vector3Double operator +(Vector3Double v1, Vector3Double v2)
        {
            return new Vector3Double(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3Double operator -(Vector3Double v1)
        {
            return new Vector3Double(-v1.x, -v1.y, -v1.z);
        }
        public static Vector3Double operator -(Vector3Double v1, Vector3Double v2)
        {
            return v1 + -v2;
        }
        public static implicit operator Vector3Double(Vector3 v1)
        {
            return new Vector3Double(v1.x, v1.y, v1.z);
        }
        public static bool operator ==(Vector3Double v1, Vector3Double v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector3Double v1, Vector3Double v2)
        {
            return !v1.Equals(v2);
        }
        public override bool Equals(object v)
        {
            if (v is Vector3Double)
            {
                return x == ((Vector3Double)v).x && y == ((Vector3Double)v).y && z == ((Vector3Double)v).z;
            }
            if (v is Vector3)
            {
                return (float)x == ((Vector3)v).x && (float)y == ((Vector3)v).y && (float)z == ((Vector3)v).z;
            }
            return false;

        }

    }


    /// <summary>
    /// A double precision Vector2
    /// </summary>
    public class Vector2Double
    {
        public double x, y;

        public Vector2Double(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static Vector2Double Zero
        {
            get => new Vector2Double(0, 0);
        }
        public static Vector2Double One
        {
            get => new Vector2Double(1, 1);
        }
        public static Vector2Double Up
        {
            get => new Vector2Double(1, 0);
        }
        public static Vector2Double Right
        {
            get => new Vector2Double(1, 0);
        }


        /// <summary>
        /// Returs the squared magnitude of the vector
        /// </summary>
        public double SqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }

        }
        /// <summary>
        /// Returs the  magnitude of the vector
        /// </summary>
        public double Magnitude
        {
            get
            {
                return System.Math.Sqrt(SqrMagnitude);
            }

        }
        /// <summary>
        /// Returs the  magnitude of the vector
        /// </summary>
        public Vector2Double Normalized
        {
            get
            {
                return this / Magnitude;
            }

        }
        /// <summary>
        /// Returns the dot product of the two vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Dot(Vector2Double v1, Vector2Double v2)
        {
            return v1.x * v2.x +
                    v1.y * v2.y;
        }
        //OPERATOR OVERLOADS
        public static Vector2Double operator *(Vector2Double v1, double v2)
        {
            return new Vector2Double(v1.x * v2, v1.y * v2);
        }
        public static Vector2Double operator /(Vector2Double v1, double v2)
        {
            return new Vector2Double(v1.x / v2, v1.y / v2);
        }
        public static Vector2Double operator *(double v2, Vector2Double v1)
        {
            return new Vector2Double(v1.x * v2, v1.y * v2);
        }
        public static Vector2Double operator +(Vector2Double v1, Vector2Double v2)
        {
            return new Vector2Double(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2Double operator -(Vector2Double v1)
        {
            return new Vector2Double(-v1.x, -v1.y);
        }
        public static Vector2Double operator -(Vector2Double v1, Vector2Double v2)
        {
            return v1 + -v2;
        }
        public static bool operator ==(Vector2Double v1, Vector2Double v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector2Double v1, Vector2Double v2)
        {
            return !v1.Equals(v2);
        }
        public static implicit operator Vector2Double(UnityEngine.Vector2 v1)
        {
            return new Vector2Double(v1.x, v1.y);
        }
        public override bool Equals(object v)
        {
            if (v is Vector2Double)
            {
                return x == ((Vector2Double)v).x && y == ((Vector2Double)v).y;
            }
            if (v is UnityEngine.Vector2)
            {
                return (float)x == ((UnityEngine.Vector2)v).x && (float)y == ((UnityEngine.Vector2)v).y;
            }
            return false;

        }
        public override int GetHashCode()
        {
            return (int)Procedural.NoiseGeneration.SquirrelNoise((int)x + (int)y * (int)y + 31 * (int)x,
                (long)x + (long)y);
        }
    
    }

}

