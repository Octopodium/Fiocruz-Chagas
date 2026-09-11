Shader "Custom/InteractableMarker"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _FresnelMultiplier ("Fresnel Multiplier", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags { 
        "RenderType" = "Transparent"
        "Queue" = "Transparent"
        "RenderPipeline" = "UniversalPipeline" 
        }
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
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD2;
                float3 cameraDirection : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _FresnelMultiplier;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexPositionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normal = TransformObjectToWorldNormal(IN.normal);
                OUT.positionHCS = vertexPositionInputs.positionCS;
                OUT.cameraDirection = GetCameraPositionWS() - vertexPositionInputs.positionWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                float fresnel = dot(normalize(IN.cameraDirection), IN.normal);
                return (1 - fresnel) * _FresnelMultiplier;
            }
            ENDHLSL
        }
    }
}
