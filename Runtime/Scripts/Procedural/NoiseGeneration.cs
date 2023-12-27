namespace AugustEngine.Procedural
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NoiseGeneration : MonoBehaviour
    {
        // Width and height of the texture in pixels.
        public int pixWidth;
        public int pixHeight;

        // The origin of the sampled area in the plane.
        public float xOrg;
        public float yOrg;

        // The number of cycles of the basic noise pattern that are repeated
        // over the width and height of the texture.
        public float scale = 1.0F;

        [SerializeField] Texture2D noiseTex;
        [SerializeField] int rec = 3;

        private Renderer rend;

        void CalcNoise(int recursions = 1)
        {
            // For each pixel in the texture...
            float y = 0.0F;
            Color[] pix = new Color[pixWidth * pixHeight];

            while (y < noiseTex.height)
            {
                float x = 0.0F;
                while (x < noiseTex.width)
                {

                    float xCoord = xOrg + x / noiseTex.width;
                    float yCoord = yOrg + y / noiseTex.height;
                    float sample = WarpedNoise(xCoord, yCoord, scale, rec);
                    sample = Mathf.Lerp(sample, WarpedNoise(xCoord, yCoord, scale / 2), HillyNoise(yCoord, xCoord));
                    pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                    x++;
                }
                y++;
            }

            // Copy the pixel data to the texture and load it into the GPU.
            noiseTex.SetPixels(pix);
            noiseTex.Apply();
        }
        public static float WarpedNoise(float x, float y, float scale = 1, float warpSize = 0, float warpStrength = 0)
        {
            if (warpSize <=0 || warpStrength <= 0)
            {
                return Mathf.PerlinNoise(x * scale, y * scale);
            }
            else
            {
                var _str = warpStrength * WarpedNoise(warpSize * x, warpSize * y, scale);
                return WarpedNoise(x+ _str, y + _str, scale);
            }
        }
        public static float GridNoise(float x, float y, float scale = 1, float warpSize = 0, float warpStrength = 0)
        {
            float width = 20f;
            float gap = 4f;
            if (warpSize <= 0 || warpStrength <= 0)
            {
                return Mathf.Lerp(
                        1f-Mathf.Max(gap*Mathf.Cos(x/ width) + 1f -gap, 0f),
 
                    WarpedNoise(x, y, scale), 0.5f
                    );
            }
            else
            {
                var _str = warpStrength * WarpedNoise(warpSize * x, warpSize * y, scale);
                return GridNoise(x + _str, y + _str, scale);
            }
        }
        public static float HillyNoise(float x, float y, float scale = 1, float warpSize = 0, float warpStrength = 0)
        {
            if (warpSize <= 0 || warpStrength <= 0)
            {
                return Mathf.Abs(Mathf.PerlinNoise(x * scale, y * scale));
            }
            else
            {
                var _str = warpStrength * HillyNoise(warpSize * x, warpSize * y, scale);
                return HillyNoise(x + _str, y + _str, scale);
            }
        }
        public static float RidgeNoise(float x, float y, float scale = 1, float warpSize = 0, float warpStrength = 0)
        {

            if (warpSize <= 0 || warpStrength <= 0)
            {
                return 1 - HillyNoise(x, y, scale);
            }
            else
            {
                var _str = warpStrength * RidgeNoise(warpSize * x, warpSize * y, scale);
                return RidgeNoise(x + _str, y + _str, scale);
             
            }
        }
        /// <summary>
        /// A fast and reliable way for generating
        /// seeded and deterministic noise
        /// </summary>
        public static long SquirrelNoise(int position, long seed = 0)
        {
            const long BIT_NOISE1 = 0xB5297A4D;
            const long BIT_NOISE2 = 0x68E31DA4;
            const long BIT_NOISE3 = 0x1B56C4E9;

            long mangled = position;
            mangled *= BIT_NOISE1;
            mangled += seed;
            mangled ^= seed;
            mangled += BIT_NOISE2;
            mangled ^= (mangled << 8);
            mangled *= BIT_NOISE3;
            mangled ^= (mangled >> 8);
            return mangled;
        }



        void OnEnable()
        {
            CalcNoise(rec);
        }
        public abstract class NoiseGenerator : MonoBehaviour
        {
            public NoiseData data;
            public abstract float EvaluatePoint(float x, float y);
        }
        [System.Serializable]
        public struct NoiseData
        {
            public float scale;
            public float heightScale;
            public int seed;
            public bool useGlobalOffset;
            public HeightMapNoise.HeightType noiseType;
        }


    }
}
