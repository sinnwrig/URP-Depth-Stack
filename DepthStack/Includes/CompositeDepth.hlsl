#ifndef COMPOSITE_DEPTH_INCLUDED
#define COMPOSITE_DEPTH_INCLUDED

#include "Common.hlsl"

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D(_PrevCameraDepth);
SAMPLER(sampler_PrevCameraDepth);

int _RenderOverlay;


float SampleDepth(float2 uv)
{
    return SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
}


float4 SampleCompositeDepth(float2 uv) {
	float rawDepth = SampleDepth(uv);
	float4 compositeDepth = 0;

	if (_RenderOverlay == 1 && !(rawDepth > 0.0)) {
		// If rendering an overlay and end of depth is reached:
		compositeDepth = SAMPLE_TEXTURE2D(_PrevCameraDepth, sampler_PrevCameraDepth, uv);
	} else {
		// Normal scene depth
		compositeDepth.x = rawDepth;
		compositeDepth.y = 0;
		compositeDepth.zw = _ZBufferParams.zw;
	}

	return compositeDepth;
}


float CompositeDepthRaw(float2 uv) 
{
	return SampleCompositeDepth(uv).x;
}


float CompositeDepth01(float2 uv) 
{
	float4 compositeDepth = SampleCompositeDepth(uv);
	return Linear01Depth(compositeDepth.x, compositeDepth);
}


float CompositeDepthEye(float2 uv) 
{
    float4 compositeDepth = SampleCompositeDepth(uv);
	return LinearEyeDepth(compositeDepth.x, compositeDepth);
}


float CompositeDepthScaled(float2 uv, float viewLength) 
{
	float rawDepth = SampleDepth(uv);
	float compositeDepth = 0;

	if (_RenderOverlay == 1 && !(rawDepth > 0.0)) {
		// If rendering an overlay and end of depth is reached:
		float4 encInfo = SAMPLE_TEXTURE2D(_PrevCameraDepth, sampler_PrevCameraDepth, uv);
		compositeDepth = LinearEyeDepth(encInfo.x, encInfo) * encInfo.y;
	} else {
		// Normal scene depth
		compositeDepth = LinearEyeDepth(rawDepth, _ZBufferParams) * viewLength;
	}

	return compositeDepth;
}

#endif


