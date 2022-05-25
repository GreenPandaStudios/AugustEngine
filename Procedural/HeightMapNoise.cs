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
            var _offsX = NoiseGeneration.SquirrelNoise(noiseData.seed, noiseData.seed)%12345;
            var _offsY = NoiseGeneration.SquirrelNoise(noiseData.seed, noiseData.seed) % 13245;

            x += _offsX;
            y -= _offsY;
            
            
           

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
        public override float EvaluatePoint(float x, float y) => EvaluatePoint(data, x, y);
       
    }
}