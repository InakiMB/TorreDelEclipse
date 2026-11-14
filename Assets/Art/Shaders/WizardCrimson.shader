Shader "TorreDelEclipse/WizardCrimson"
{
    Properties
    {
        _BaseMap("Textura original", 2D) = "white" {}
        _BaseColor("Color general", Color) = (1,1,1,1)
        _RobeColor("Tunica rojo oscuro", Color) = (0.28,0.015,0.03,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _RobeColor;
                half _Cutoff;
            CBUFFER_END
            struct Input { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; };
            struct Output { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half3 normalWS:TEXCOORD1; float3 positionWS:TEXCOORD2; half fog:TEXCOORD3; };
            Output Vert(Input input)
            {
                Output output;
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = position.positionCS; output.positionWS = position.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fog = ComputeFogFactor(position.positionCS.z);
                return output;
            }
            half4 Frag(Output input):SV_Target
            {
                half3 textureColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).rgb;
                // Recolorear solamente el azul de la tunica; conservar barba, piel y bordes dorados.
                half robe = saturate((textureColor.b / max(max(textureColor.r, textureColor.g), 0.001) - 1.1) * 4);
                half brightness = saturate(textureColor.b * 4 + 0.28);
                half3 albedo = lerp(textureColor, _RobeColor.rgb * brightness, robe) * _BaseColor.rgb;
                half3 normal = normalize(input.normalWS);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 illumination = max(SampleSH(normal), half3(0.12,0.12,0.12));
                illumination += mainLight.color * saturate(dot(normal, mainLight.direction)) * mainLight.shadowAttenuation;
                return half4(MixFog(albedo * illumination, input.fog), 1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
}
