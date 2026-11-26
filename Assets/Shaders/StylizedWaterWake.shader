Shader "Custom/StylizedWaterWake"
{
    Properties
    {
        _BaseColor ("Noise Color", Color) = (1.0, 1.0, 1.0, 1.0)

        [Header(World Space Noise Settings)]
        _NoiseScale ("Noise Scale", Float) = 6.0
        _NoiseThreshold ("Noise Density Threshold", Range(0.1, 0.9)) = 0.55
        _ScrollSpeed ("Noise Drift Speed", Float) = 0.35
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent-1" 
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv         : TEXCOORD1;
                float4 color      : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _NoiseScale;
                float _NoiseThreshold;
                float _ScrollSpeed;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float SmoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // World-space XZ noise so foam stays pinned to the water when turning
                float2 noiseUv = input.positionWS.xz * _NoiseScale;
                float n1 = SmoothNoise(noiseUv + float2(_Time.y * _ScrollSpeed, _Time.y * _ScrollSpeed * 0.4));
                float n2 = SmoothNoise(noiseUv * 2.3 - float2(_Time.y * _ScrollSpeed * 0.5, 3.1));
                float noise = n1 * 0.6 + n2 * 0.4;

                // Thin out noise density toward side edges and tail, but keep each speck solid white (no gradient blur)
                float edgeMask = 1.0 - abs(input.uv.y - 0.5) * 2.0;
                float density = saturate(input.color.a * saturate(edgeMask * 1.3));
                float threshold = lerp(1.02, _NoiseThreshold, density);

                float alpha = step(threshold, noise);
                if (alpha < 0.5) discard;

                return half4(_BaseColor.rgb, _BaseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
