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


        public static float EvaluatePoint(NoiseGeneration.NoiseData noiseData, float x, float y)
        {
            if (noiseData.useGlobalOffset)
            {
                x += (float) InfiniteWorldTransform.GlobalOffset.x;
                y += (float) InfiniteWorldTransform.GlobalOffset.z;
            }

            x += noiseData.seed;
            y -= noiseData.seed;
            
            
            float landmass = (Mathf.Cos(x * noiseData.scale /FEEL_TO_SCALE_RATIO) + Mathf.Cos(y * noiseData.scale / FEEL_TO_SCALE_RATIO))/2;

            //first, generate the overall "feel" of the land, this should be 10 times less than the scale
            float feel1 = NoiseGeneration.WarpedNoise(x, y, noiseData.scale / FEEL_TO_SCALE_RATIO/10f, 1000, .95f);
            float feel2 = NoiseGeneration.HillyNoise(x, y, noiseData.scale /10f, 50, .95f);

            //float feel2



            return (noiseData.heightScale * (0.5f - (feel1 * .95f + feel2*.05f))) -
                (noiseData.useGlobalOffset ?
               (float)InfiniteWorldTransform.GlobalOffset.y : 0f);
        }
        public override float EvaluatePoint(float x, float y) => EvaluatePoint(data, x, y);
       
    }
}