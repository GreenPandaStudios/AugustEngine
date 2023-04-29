#ifndef PIXEL_SHADER_INCLUDE
#define PIXEL_SHADER_INCLUDE


void Pixelate_float(float2 UV, float pixels, float width, float height, out float3 Out) {

    UV.x *= width;
    UV.y *= height;

    UV.x = trunc((UV.x / pixels)) * pixels;
    UV.y = trunc((UV.y / pixels)) * pixels;

    UV.x /= width;
    UV.y /= height;

    Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV);
}

#endif