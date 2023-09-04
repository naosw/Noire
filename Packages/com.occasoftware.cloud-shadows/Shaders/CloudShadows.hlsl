#ifndef CLOUD_SHADOWS_INCLUDED
#define CLOUD_SHADOWS_INCLUDED

#pragma warning (disable : 4068 )

half InverseLerp(half a, half b, half v)
{
	return (v - a) / (b - a);
}

half RemapUnclamped(half iMin, half iMax, half oMin, half oMax, half v)
{
	half t = InverseLerp(iMin, iMax, v);
	return lerp(oMin, oMax, t);
}

half Remap(half iMin, half iMax, half oMin, half oMax, half v)
{
	v = clamp(v, iMin, iMax);
	return RemapUnclamped(iMin, iMax, oMin, oMax, v);
}

float2 _CloudShadowOffsets[3];
SamplerState linear_repeat_sampler;
void GetCloudData_float(float2 UV, Texture2D CloudTexture, float Cloudiness, float TextureOrientation, float TilingDomain, out float Out)
{
	float frequency = 1.0;
	float amplitude = 1.0;
	float value = 0.0;
	float sum = 0;
	float globalScale = 1.0 / TilingDomain;
	
	float2x2 RM = float2x2(cos(TextureOrientation), -sin(TextureOrientation), sin(TextureOrientation), cos(TextureOrientation));
	
    for(int i = 0; i < 3; i++)
	{
		float2 temporaryUV = (UV + _CloudShadowOffsets[i] * globalScale) * frequency;
		temporaryUV = mul(temporaryUV, RM);
		value += amplitude * CloudTexture.Sample(linear_repeat_sampler, temporaryUV).r;
		sum += amplitude;
		frequency *= 2.0;
		amplitude *= 0.5;
	}
	
	value /= sum;
	
	value = Remap(1.0 - Cloudiness, 1.0, 0.0, 1.0, value);
	Out = saturate(value);
}


float IGN(uint pixelX, uint pixelY)
{
    return fmod(52.9829189f * fmod(0.06711056f * float(pixelX) + 0.00583715f * float(pixelY), 1.0f), 1.0f);
}

int _NoiseOption;
void DitherCloudShadowMap_float(float In, float2 InputUV, float NoiseScale, float FadeInExtents, float MaximumOpacity, Texture2D DitherTex, out float Out)
{
    if (FadeInExtents < 1e-5)
    {
        Out = In;
        return;
    }
	
    float2 shadowmapSize = float2(4096.0, 4096.0);
	#define TexSampleScale 1.0 / 64.0
    float2 uv = InputUV.xy * shadowmapSize * 10.0 * NoiseScale;
    
    float ign = saturate(IGN(uv.x, uv.y));
    if (_NoiseOption == 1)
    {
        ign = DitherTex.SampleLevel(linear_repeat_sampler, uv * TexSampleScale, 0).a;
    }
	
    float c = Remap(0, FadeInExtents, 0, MaximumOpacity, In);
    Out = step(ign, c);
}
#endif