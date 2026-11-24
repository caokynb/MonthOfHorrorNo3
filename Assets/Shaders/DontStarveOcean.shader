Shader "Custom/DontStarveOcean"
{
    Properties
    {
        [Header(Base Ocean Watercolor)]
        _DeepColor ("Deep Water Color", Color) = (0.05, 0.14, 0.22, 1.0)
        _ShallowColor ("Mid Water Color", Color) = (0.12, 0.32, 0.40, 1.0)
        _WashSpeed ("Water Drift Speed", Float) = 0.15

        [Header(Dont Starve Wave Ripples)]
        _FoamColor ("Wave White Chalk Stroke", Color) = (0.95, 0.98, 0.95, 1.0)
        _ShadowColor ("Wave Dark Inking Shadow", Color) = (0.02, 0.06, 0.10, 1.0)
        _WaveScale ("Wave Density Scale", Float) = 0.35
        _WaveSpeed ("Wave Bob and Drift Speed", Float) = 1.0
        _WaveCurvature ("Wave Arch Curvature", Range(0.5, 3.5)) = 1.8
        _WaveThickness ("Wave Stroke Thickness", Range(0.01, 0.15)) = 0.04

        [Header(Stylized Lighting and Brightness)]
        _BaseSelfLit ("Base Visibility in Dark", Range(0.1, 1.2)) = 0.65
        _LightIntensity ("Boat Light Sensitivity", Range(0.5, 4.0)) = 2.0
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
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
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
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _DeepColor;
                float4 _ShallowColor;
                float4 _FoamColor;
                float4 _ShadowColor;
                float _WashSpeed;
                float _WaveScale;
                float _WaveSpeed;
                float _WaveCurvature;
                float _WaveThickness;
                float _BaseSelfLit;
                float _LightIntensity;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Silky smooth watercolor wash using harmonic continuous waves - ZERO pixelation
            float WatercolorWash(float2 pos, float time)
            {
                float2 drift = float2(time * 0.02, time * 0.01);
                float w1 = sin(pos.x * 0.03 + drift.x) * cos(pos.y * 0.025 + drift.y);
                float w2 = sin((pos.x + pos.y) * 0.015 - drift.x * 0.7);
                float w3 = cos(pos.x * 0.06 - pos.y * 0.04 + drift.y * 1.2);
                return (w1 * 0.45 + w2 * 0.35 + w3 * 0.20) * 0.5 + 0.5;
            }

            // Procedural hand-drawn crescent wave ripples in Don't Starve art style
            float2 WaveCrescents(float2 worldXZ, float time, float density, float speed)
            {
                float2 p = worldXZ * density;
                
                // Stagger alternate rows to prevent rigid grid lines
                p.x += (floor(p.y) % 2.0) * 0.5;

                float2 cellId = floor(p);
                float2 cellUv = frac(p) - 0.5;

                float whiteStroke = 0.0;
                float darkShadow = 0.0;

                [unroll]
                for (int dy = -1; dy <= 1; dy++)
                {
                    [unroll]
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        float2 neighbor = float2(dx, dy);
                        float2 curCell = cellId + neighbor;

                        float h1 = Hash21(curCell);
                        float h2 = Hash21(curCell + float2(43.12, 89.23));
                        float h3 = Hash21(curCell + float2(127.54, 311.71));

                        // Spawn on ~75% of cells
                        if (h1 < 0.25) continue;

                        float2 centerOffset = (float2(h2, h3) - 0.5) * 0.6;
                        float2 waveCenter = neighbor + centerOffset;

                        // Periodic breathing cycle: drift, appear, peak, fade out
                        float lifeTime = time * speed + h1 * 6.28318;
                        float cycle = sin(lifeTime);
                        if (cycle <= 0.0) continue;

                        float alpha = smoothstep(0.0, 0.35, cycle) * smoothstep(1.0, 0.65, cycle);

                        // Gentle breeze drift in diagonal direction
                        float2 driftDir = float2(0.4, 0.6);
                        float2 rel = cellUv - waveCenter - driftDir * (1.0 - cycle) * 0.25;

                        // Slight random hand-drawn rotation tilt
                        float rotAngle = (h3 - 0.5) * 0.35;
                        float cosA = cos(rotAngle);
                        float sinA = sin(rotAngle);
                        float2 rotP = float2(rel.x * cosA - rel.y * sinA, rel.x * sinA + rel.y * cosA);

                        // Hand-drawn arch dimensions
                        float arcWidth = lerp(0.24, 0.38, h2);
                        float curve = _WaveCurvature * lerp(0.9, 1.2, h1);

                        if (abs(rotP.x) < arcWidth)
                        {
                            // Parabolic arch: y = -curve * x^2 + offset
                            float archY = -curve * (rotP.x * rotP.x) + 0.03;
                            float distWhite = abs(rotP.y - archY);

                            // Chalk/brush taper: thick in center, razor sharp at tips
                            float taper = 1.0 - pow(abs(rotP.x) / arcWidth, 1.6);
                            float thickness = _WaveThickness * (taper * 0.8 + 0.2);

                            // White foam stroke
                            float stroke = smoothstep(thickness, thickness * 0.2, distWhite) * taper;
                            whiteStroke += stroke * alpha;

                            // Dark ink shadow underneath the white crest
                            float shadowY = archY - thickness * 1.6;
                            float distShadow = abs(rotP.y - shadowY);
                            float sStroke = smoothstep(thickness * 1.4, 0.0, distShadow) * taper * 0.7;
                            darkShadow += sStroke * alpha;
                        }
                    }
                }

                return float2(saturate(whiteStroke), saturate(darkShadow));
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);

                output.positionWS = worldPos;
                output.positionCS = TransformWorldToHClip(worldPos);
                output.fogFactor = ComputeFogFactor(output.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 worldPos = input.positionWS;
                float timeVal = _Time.y;

                // 1. Smooth watercolor wash background (zero noise, silky gradients)
                float wash = WatercolorWash(worldPos.xz, timeVal * _WashSpeed);
                half3 waterColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, wash);

                // 2. Primary Hand-Drawn Don't Starve Wave Crescents
                float2 primaryWaves = WaveCrescents(worldPos.xz, timeVal, _WaveScale, _WaveSpeed);

                // 3. Secondary smaller ripple layer for rich 2.5D visual depth
                float2 secondaryWaves = WaveCrescents(worldPos.xz + 53.7, timeVal * 0.85, _WaveScale * 1.6, _WaveSpeed * 0.85);

                // Blend ink shadows first, then white chalk foam strokes
                float totalShadow = saturate(primaryWaves.y + secondaryWaves.y * 0.6);
                float totalWhite  = saturate(primaryWaves.x + secondaryWaves.x * 0.7);

                waterColor = lerp(waterColor, _ShadowColor.rgb, totalShadow);
                waterColor = lerp(waterColor, _FoamColor.rgb, totalWhite);

                // 4. Stylized Base Visibility (Atmosphere)
                half3 litColor = waterColor * _BaseSelfLit;

                // 5. Directional / Main Light (Moonlight or Sun)
                Light mainLight = GetMainLight();
                litColor += mainLight.color * (mainLight.distanceAttenuation * 0.25) * waterColor;

                // 6. Real-time Boat Spotlight & Additional Lights (Supports Unity 6 Forward & Forward+ Clustered)
                InputData inputData = (InputData)0;
                inputData.positionWS = worldPos;
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

                #if defined(_ADDITIONAL_LIGHTS) || defined(_CLUSTER_LIGHT_LOOP)
                uint lightsCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(lightsCount)
                    Light addLight = GetAdditionalLight(lightIndex, worldPos);
                    half attenuation = addLight.distanceAttenuation * addLight.shadowAttenuation;
                    litColor += addLight.color * (attenuation * _LightIntensity) * (waterColor * 1.2 + 0.1);
                LIGHT_LOOP_END
                #endif

                // 7. Distance Fog
                litColor = MixFog(litColor, input.fogFactor);

                return half4(litColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
