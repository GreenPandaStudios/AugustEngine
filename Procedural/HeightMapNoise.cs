using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.Procedural
{
    public class HeightMapNoise : NoiseGeneration.NoiseGenerator
    {
        const float FEEL_TO_SCALE_RATIO = 50f;
        const int FEEL_RECURSIONS = 4;
        const float BIOME_TO_SCALE_RATIO = 250f;
        const int BIOME_RECURSIONS = 2;
        const float FEEL_HEIGHT_RATIO = .75f;

        public enum HeightType
        {
            Land,
            Water,
            Temperature,
            Tree,
        }

        public static float EvaluatePoint(NoiseGeneration.NoiseData noiseData, float x, float y)
        {
            if (noiseData.useGlobalOffset)
            {
                x += (float) InfiniteWorldTransform.GlobalOffset.x;
                y += (float) InfiniteWorldTransform.GlobalOffset.z;
            }
            var _offsX = NoiseGeneration.SquirrelNoise(noiseData.seed, noiseData.seed) % 12345;
            var _offsY = NoiseGeneration.SquirrelNoise(noiseData.seed, noiseData.seed) % 13245;

            x += _offsX;
            y -= _offsY;

            if (noiseData.noiseType == HeightType.Land)
                return EvaluateLand(noiseData, x, y);
            else if (noiseData.noiseType == HeightType.Water)
                return EvaluateWater(noiseData, x, y);

            return 0;



        }
        
        private static float EvaluateLand(NoiseGeneration.NoiseData noiseData, float x, float y)
        {
            //check if we have this stored as an override height
            if (overrideHeights.TryGetValue(new Vector2(x, y), out var _h))
            {
                return _h;
            }



            //first, generate the overall "feel" of the land, this should be 10 times less than the scale
            float feel1 = NoiseGeneration.WarpedNoise(x, y, noiseData.scale / FEEL_TO_SCALE_RATIO / 20f, 100, 100f);
            float feel2 = NoiseGeneration.WarpedNoise(y, x, noiseData.scale / FEEL_TO_SCALE_RATIO / 10f, 50, 5f);
            float feel3 = Mathf.Pow(NoiseGeneration.WarpedNoise(x, y, noiseData.scale / FEEL_TO_SCALE_RATIO / 5f, 50, 5f), 2f);
            float feel4 = NoiseGeneration.WarpedNoise(y, x, noiseData.scale / FEEL_TO_SCALE_RATIO / 2f, 50, 5f);

            //float feel2



            return (noiseData.heightScale * (0.5f - (feel1 * .95f + feel2 * .3f + feel3 * .1f + feel4 * .1f))) -
                (noiseData.useGlobalOffset ?
               (float)InfiniteWorldTransform.GlobalOffset.y : 0f);
        }
        
        private static float EvaluateWater(NoiseGeneration.NoiseData noiseData, float x, float y)
        {
            return noiseData.heightScale +
                NoiseGeneration.WarpedNoise(x, y, noiseData.scale / FEEL_TO_SCALE_RATIO / 20f, 100, 100f);
        }
        
        public override float EvaluatePoint(float x, float y) => EvaluatePoint(data, x, y);



        private static Dictionary<Vector2, float> overrideHeights = new Dictionary<Vector2, float>();
        /// <summary>
        /// Adds an overriden height to this heightmap
        /// An overriden height will return instead of the base noise function at
        /// that point
        /// </summary>
        /// <param name="point_loc"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static bool AddHeightOverride(Vector2 point_loc, float newHeight)
        {
            overrideHeights[point_loc] = newHeight;
            return true;
        }
        /// <summary>
        /// Removes an overriden height on this heightmap
        /// An overriden height will return instead of the base noise function at
        /// that point
        /// </summary>
        /// <param name="point_loc"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static bool RemoveHeightOverride(Vector2 point_loc)
        {
            overrideHeights.Remove(point_loc);
            return true;
        }

    }
}