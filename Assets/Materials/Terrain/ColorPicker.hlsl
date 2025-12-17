#ifndef TERRAIN_COLOR_PICKER_H
#define TERRAIN_COLOR_PICKER_H

void ColorPicker_float(
    float3 sand, 
    float3 grass, 
    float3 light_grass,
    float3 rock,
    float3 light_rock,
    float3 snow, 
    float height, 
    float snowHeightThreshold,
    float angle, 
    out float3 color)
{
    if (angle > 45)
    {
        float progress = clamp((angle - 45) / 10.0, 0.0, 1.0);
        color = rock * progress + light_rock * (1 - progress);
        return;
    }
    
    
    if (height < 10)
    {
        color = sand;
        return;
    }


    float progress = clamp(angle / 30.0, 0.0, 1.0);
    color = grass * progress + light_grass * (1 - progress);

    progress = clamp((height - snowHeightThreshold) / 25, 0.0, 1.0);
    color = snow * progress + color * (1 - progress);
}

#endif