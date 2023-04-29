#ifndef OUTLINE_INCLUDE
#define OUTLINE_INCLUDE

static float2 samplePoints[9] = {
    float2(-1,1), float2(0,1), float2(1,1),
    float2(-1,0), float2(0,0), float2(1,1),
    float2(-1,-1), float2(0,-1),float2(1,1),
};

void DepthSobel_float(float2 UV, float Threshold, float Thickness, out float Out) {
    float diff = 0;

    float c_depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
    // Sample depth
    [unroll] for (int i = 0; i < 9; i++) {

        float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + (samplePoints[i]/_ScreenParams) * Thickness);
        diff = max(diff,abs(c_depth - depth));
    }
    if (diff > Threshold) {
        Out = 1.0;
        return;
    }

    // Now, we're going to sample the scene normal's z axis and use that for outlines too
    float3 c_normal = normalize(SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV));
    [unroll] for (int i = 0; i < 9; i++) {
        float3 normal = normalize(SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV + (samplePoints[i]/_ScreenParams) * Thickness));
        diff = max(diff,1.0-abs(dot(c_normal,normal)));
    }
    if (diff > Threshold) {
        Out = 1.0;
        return;
    }
    Out = 0.0;
}

#endif