#ifndef LAYRED_NOISE_H
#define LAYRED_NOISE_H

// Taken from https://docs.unity3d.com/Packages/com.unity.shadergraph@7.1/manual/Gradient-Noise-Node.html
float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

// Taken from https://docs.unity3d.com/Packages/com.unity.shadergraph@7.1/manual/Gradient-Noise-Node.html
float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

void LayredNoise_float(
    float frequency, 
    float frequencyGain, 
    float amplitudeGain,
    float octaves,
    float2 UV,
    out float sample)
{
    float weight = 0;
    float f = frequency;
    float a = 1;
    sample = 0;
    
    for (float i = 0; i < octaves; i++)
    {
        sample += unity_gradientNoise(UV * f) * a;
        
        weight += a;
        
        f *= frequencyGain;
        a *= amplitudeGain;
    }
    
    sample /= weight;
}

#endif