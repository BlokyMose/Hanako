#ifndef JETTELLY_COMMON_INCLUDED
#define JETTELLY_COMMON_INCLUDED

#include "UnityShaderVariables.cginc"

inline half4 Jettelly_RainbowColor(float2 uv, float speed, float frequency)
{
    half4 r = sin((uv.y + _Time.y * speed) * float4(0.45, 0.3245, 0.345, 1) * frequency) * 0.5 + 0.5;
    return saturate(r);
}

inline half4 Jettelly_ThreeColorGradient(float4 colorTop, float4 colorMiddle, float4 colorBottom, float v)
{
    half4 r = lerp(colorBottom, colorMiddle, v / 0.5) * step(v, 0.5);
    r += lerp(colorMiddle, colorTop, (v - 0.5) / (1 - 0.5)) * step(0.5, v);
    return saturate(r);
}

inline half Jettelly_SpecularShading(float3 colorRefl, float specularInt, float3 normal, float3 lightDir, float3 viewDir, float specularPow)
{
    float3 h = normalize(lightDir + viewDir);
    half r = colorRefl * specularInt * pow(max(0, dot(normal, h)), specularPow);
    return saturate(r);
}

inline half3 Jettelly_Glitter(float3 tangentDir, float3 bitangentDir, float3 normalDir, float3 viewDir, float2 uv, Texture2D glitterMap, SamplerState sampler_glitterMap, float glitPow)
{
    float pi = UNITY_PI;

    float3x3 matrixTBN = float3x3(tangentDir, bitangentDir, normalDir);
    float2 uv_g01 = ((0.05 * mul(matrixTBN, viewDir).xy + uv).rg * ((1.0 / 2.0) + 1.0));
    float4 glitCol01 = glitterMap.Sample(sampler_glitterMap, uv_g01);    

    float2 v = (mul((-0.05 *5* mul(matrixTBN, viewDir).xy + uv).rg - float2(0.5,0.5), float2x2(cos(pi), -sin(pi), sin(pi), cos(pi))) + float2(0.5,0.5));
    float2 uv_g02 = (v * (1.0 - (1.0 / pi)));
    float4 glitCol02 = glitterMap.Sample(sampler_glitterMap,uv_g02);
                    
    float3 col = (lerp(pow((3 * glitCol01.rgb), 3), float3(0, 0, 0), max((1.0 - glitCol02.rgb), float3(0, 0, 0))));
    float3 r = pow(col, 2) * glitPow;
    return saturate(r);
}

#endif