Shader "Custom/AtmosphericOcean"
{
    Properties
    {
        [Header(Water Colors)]
        _DeepColor ("Deep Water Color", Color) = (0.015, 0.035, 0.07, 1.0)
        _ShallowColor ("Crest / Shallow Color", Color) = (0.05, 0.14, 0.22, 1.0)

        [Header(Wave Parameters)]
        _WaveSpeed ("Wave Speed", Float) = 1.2
        _WaveFrequency ("Wave Frequency", Float) = 0.08
        _WaveHeight ("Vertex Wave Height", Float) = 0.3
        _NormalStrength ("Normal Ripple Strength", Float) = 2.5

        [Header(Lighting and Specular)]
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.88
        _SpecularTint ("Specular Tint", Color) = (0.9, 0.95, 1.0, 1.0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float fogFactor   : TEXCOORD1;
                float waveFactor  : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _DeepColor;
                float4 _ShallowColor;
                float4 _SpecularTint;
                float _WaveSpeed;
                float _WaveFrequency;
                float _WaveHeight;
                float _NormalStrength;
                float _Smoothness;
            CBUFFER_END

            float CalculateWave(float3 worldPos, float timeVal)
            {
                float w1 = sin(worldPos.x * _WaveFrequency + timeVal * _WaveSpeed);
                float w2 = cos(worldPos.z * (_WaveFrequency * 1.3) + timeVal * (_WaveSpeed * 0.85));
                float w3 = sin((worldPos.x + worldPos.z) * (_WaveFrequency * 0.6) + timeVal * (_WaveSpeed * 1.1));
                return (w1 + w2 + w3) * 0.333;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float timeVal = _Time.y;

                float wave = CalculateWave(worldPos, timeVal);
                worldPos.y += wave * _WaveHeight;

                output.positionWS = worldPos;
                output.positionCS = TransformWorldToHClip(worldPos);
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                output.waveFactor = wave;

                return output;
            }

            float3 CalculateProceduralNormal(float3 worldPos, float timeVal)
            {
                float delta = 0.2;
                float wCenter = CalculateWave(worldPos, timeVal);
                float wRight  = CalculateWave(worldPos + float3(delta, 0, 0), timeVal);
                float wForward= CalculateWave(worldPos + float3(0, 0, delta), timeVal);

                float dX = (wRight - wCenter) / delta;
                float dZ = (wForward - wCenter) / delta;

                float3 normalWS = normalize(float3(-dX * _NormalStrength, 1.0, -dZ * _NormalStrength));
                return normalWS;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 worldPos = input.positionWS;
                float timeVal = _Time.y;

                float3 normalWS = CalculateProceduralNormal(worldPos, timeVal);
                float3 viewDirWS = normalize(GetCameraPositionWS() - worldPos);

                // Blend color between wave troughs (deep) and wave crests (shallow)
                float blend = saturate(input.waveFactor * 0.5 + 0.5);
                half3 waterBaseColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, blend);

                // Fresnel effect for water edge reflections
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 4.0);
                waterBaseColor = lerp(waterBaseColor, _ShallowColor.rgb * 1.3, fresnel * 0.6);

                // Lighting calculation (Main Light + Boat Spotlight)
                half3 totalDiffuse = half3(0, 0, 0);
                half3 totalSpecular = half3(0, 0, 0);

                // 1. Main Directional Light
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                totalDiffuse += mainLight.color * NdotL;

                float3 halfDirMain = normalize(mainLight.direction + viewDirWS);
                float NdotHMain = saturate(dot(normalWS, halfDirMain));
                float specMain = pow(NdotHMain, _Smoothness * 128.0) * _Smoothness;
                totalSpecular += mainLight.color * specMain * _SpecularTint.rgb;

                // 2. Additional Lights (Boat Spotlight, Lantern, etc.)
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < pixelLightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, worldPos);
                    half NdotLAdd = saturate(dot(normalWS, addLight.direction));
                    half3 attenuatedLight = addLight.color * (addLight.distanceAttenuation * addLight.shadowAttenuation);

                    totalDiffuse += attenuatedLight * NdotLAdd;

                    float3 halfDirAdd = normalize(addLight.direction + viewDirWS);
                    float NdotHAdd = saturate(dot(normalWS, halfDirAdd));
                    float specAdd = pow(NdotHAdd, _Smoothness * 128.0) * _Smoothness;
                    totalSpecular += attenuatedLight * specAdd * _SpecularTint.rgb;
                }
                #endif

                // Ambient contribution
                half3 ambient = SampleSH(normalWS) * 0.5;
                half3 finalColor = waterBaseColor * (ambient + totalDiffuse) + totalSpecular;

                // Apply URP Fog
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
