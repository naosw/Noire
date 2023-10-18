Shader "Custom/DepthTextureGreyScale"
{
    Properties
    { 
        _MainTex("Texture", 2D) = "white" {}
        _Distort("Distort", float) = 0
        _ColorOne("Color One", color) = (1,1,1,1)
        _ColorTwo("Color Two", color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"  

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenspace : TEXCOORD1;
            };
            
            float CorrectDepth(float rawDepth)
            {
                float persp = Linear01Depth(rawDepth);
                float ortho = (_ProjectionParams.z - _ProjectionParams.y) * (1 - rawDepth) + _ProjectionParams.y;
                return lerp(persp,ortho,unity_OrthoParams.w);
            }
            
            sampler2D _MainTex, _CameraDepthTexture;
            float4 _MainTex_ST, _ColorOne, _ColorTwo;
            float _Distort;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenspace = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 UV_offset = tex2D(_MainTex, i.uv);
                float2 screenSpaceUVs = i.screenspace.xy / i.screenspace.w;
                screenSpaceUVs = 2 * screenSpaceUVs - 1;
                screenSpaceUVs += UV_offset * _Distort;
                screenSpaceUVs = screenSpaceUVs * 0.5 + 0.5;
                
                float depth = CorrectDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUVs));
                float3 mixedColor = lerp(_ColorOne, _ColorTwo, depth);
                
                return fixed4(mixedColor, 1);
            }
            
            ENDCG
        }
    }
}