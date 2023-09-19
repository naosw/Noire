Shader "Hidden/Sobel"
{
Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

        Pass
        {
			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
			
			struct appdata
            {
            	float4 positionOS : Position;
            	float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
            	float4 positionCS : SV_Position;
            	float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
            	v2f o;
            	o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            	o.uv = v.uv;
            	return o;
            }

			sampler2D _MainTex;
			

			CBUFFER_START(UnityPerMaterial)
				float2 _MainTex_TexelSize;
				float _DepthThreshold;
				float4 _DepthOutlineColor;
								float _NormalThreshold;
                				float4 _NormalOutlineColor;
			CBUFFER_END
            
            float Sobel_Scharr(float3 topLeft, float3 top, float3 topRight,
                                                       float3 left, float3 right,
                                                       float3 bottomLeft, float3 bottom, float3 bottomRight)
            {
                float3 x = -3 * topLeft - 10 * left - 3 * bottomLeft + 3 * topRight   + 10 * right  + 3 * bottomRight;
                float3 y =  3 * topLeft + 10 * top  + 3 * topRight   - 3 * bottomLeft - 10 * bottom - 3 * bottomRight;
            
                return sqrt(dot(x, x) + dot(y, y));
            }
                                    
			float sobel_depth(float2 uv)
			{
				float3 x = 0;
				float3 y = 0;

				float2 pixel = _MainTex_TexelSize;
                
				return Sobel_Scharr(
                                           SampleSceneDepth(uv + float2(-pixel.x, -pixel.y)),
                                           SampleSceneDepth( uv + float2(0, -pixel.y)),
                                           SampleSceneDepth( uv + float2(pixel.x, -pixel.y)),
                                           SampleSceneDepth( uv + float2(-pixel.x, 0)),
                                           SampleSceneDepth( uv + float2(pixel.x, 0)),
                                           SampleSceneDepth( uv + float2(-pixel.x, pixel.y)),
                                           SampleSceneDepth( uv + float2(0, pixel.y)),
                                           SampleSceneDepth( uv + float2(pixel.x, pixel.y))
                                       );
			}
						float3 SobelSampleDepth(float2 uv, float3 offset)
            {
                float3 pixelCenter = SampleSceneDepth( uv);
                float3 pixelLeft   = SampleSceneDepth( uv - offset.xz);
                float3 pixelRight  = SampleSceneDepth( uv + offset.xz);
                float3 pixelUp     = SampleSceneDepth( uv + offset.zy);
                float3 pixelDown   = SampleSceneDepth( uv - offset.zy);
                
                return abs(pixelLeft  - pixelCenter) +
                       abs(pixelRight - pixelCenter) +
                       abs(pixelUp    - pixelCenter) +
                       abs(pixelDown  - pixelCenter);
            }
			float3 SobelSample(float2 uv, float3 offset)
{
    float3 pixelCenter = SampleSceneNormals( uv);
    float3 pixelLeft   = SampleSceneNormals( uv - offset.xz);
    float3 pixelRight  = SampleSceneNormals( uv + offset.xz);
    float3 pixelUp     = SampleSceneNormals( uv + offset.zy);
    float3 pixelDown   = SampleSceneNormals( uv - offset.zy);
    
    return abs(pixelLeft  - pixelCenter) +
           abs(pixelRight - pixelCenter) +
           abs(pixelUp    - pixelCenter) +
           abs(pixelDown  - pixelCenter);
}
			float sobel_normal(float2 uv){
							float3 x = 0;
            				float3 y = 0;
            
            				float2 pixel = _MainTex_TexelSize;
  
            				return Sobel_Scharr(
                                                       SampleSceneNormals(uv + float2(-pixel.x, -pixel.y)),
                                                       SampleSceneNormals( uv + float2(0, -pixel.y)),
                                                       SampleSceneNormals( uv + float2(pixel.x, -pixel.y)),
                                                       SampleSceneNormals( uv + float2(-pixel.x, 0)),
                                                       SampleSceneNormals( uv + float2(pixel.x, 0)),
                                                       SampleSceneNormals( uv + float2(-pixel.x, pixel.y)),
                                                       SampleSceneNormals( uv + float2(0, pixel.y)),
                                                       SampleSceneNormals( uv + float2(pixel.x, pixel.y))
                                                   );
			}
            
            float4 frag (v2f i) : SV_Target
            {
				
				float4 col = tex2D(_MainTex, i.uv);

				float4 background = col;
				float3 offset = float3((1.0 / _ScreenParams.x), (1.0 / _ScreenParams.y), 0.0);
				float3 sobelDepthVec = SobelSampleDepth(i.uv, offset).rgb;
				float sobelDepth = sobelDepthVec.x + sobelDepthVec.y + sobelDepthVec.z;
            	float3 sobelNormalVec = SobelSample(i.uv, offset).rgb;
				float sobelNormal = sobelNormalVec.x + sobelNormalVec.y + sobelNormalVec.z;
				sobelNormal = 1-_NormalThreshold > sobelNormal ? 0 : 1;
				sobelDepth = sobelDepth > _DepthThreshold ? 1 : 0;
            	float3 color = lerp(lerp(background, float3(0.1,0.1,0.1 ), sobelNormal ), _NormalOutlineColor, sobelDepth );
                return float4(color, 1.0);
			
            }
            ENDHLSL
        }
    }
}