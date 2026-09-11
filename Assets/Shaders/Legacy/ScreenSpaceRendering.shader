Shader "Custom/ScreenSpaceRendering"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Scale ("Scale", Float) = 1
    }

    SubShader
    {
        Tags {
        "Queue" = "Overlay"
        "RenderType" = "Overlay"
        "ForceNoShadowCasting" = "True"
        "IgnoreProjector" = "True"
        "RenderPipeline" = "UniversalPipeline" 
        }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always 


        Pass
        {   
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _Scale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                float4 ScreenCenter = TransformObjectToHClip(float4(0,0,0,1));
                float2 normalizedScreenPositions = ScreenCenter.xy/ScreenCenter.w;
                float aspectRatio = _ScreenParams.x / _ScreenParams.y; 
                Varyings OUT;
                OUT.position = float4((normalizedScreenPositions.xy + float2(IN.positionOS.x  , IN.positionOS.y * aspectRatio) * _Scale) * ScreenCenter.w, ScreenCenter.z, ScreenCenter.w);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return color;
            }
            ENDHLSL
        }
    }
}
