#ifndef TERRAIN_COLOR_PICKER_H
#define TERRAIN_COLOR_PICKER_H

void ColorPicker_float(float3 sand, float3 grass, float3 rock, float3 snow, float height, bool steep, out float3 color)
{
    if (steep)
    {
        color = rock;
        return;
    }
    
    if (height < 10)
    {
        color = sand;
        return;
    }

    if (height < 500)
    {
        color = grass;
        return;
    }
    color = snow;
}

#endif