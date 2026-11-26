Shader "Custom/DonStarveWakeRipple"
{
    Properties
    {
        _BaseColor ("Chalk Foam Color", Color) = (0.92, 0.96, 0.94, 0.9)
        _ShadowColor ("Ink Shadow Color", Color) = (0.02, 0.06, 0.10, 0.6)
        _Curvature ("Arch Curvature", Range(0.5, 3.0)) = 1.3
        _Thickness ("Line Thickness", Range(0.05, 0.35)) = 0.14
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
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
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ShadowColor;
                float _Curvature;
                float _Thickness;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Normalize UV to [-1, 1] centered
                float2 p = input.uv * 2.0 - 1.0;

                // Main crescent arc opening backwards behind the boat
                float arcW = 0.75;
                float whiteStroke = 0.0;
                float darkShadow = 0.0;

                if (abs(p.x) < arcW)
                {
                    float targetY = -_Curvature * (p.x * p.x) + 0.25;
                    float dist = abs(p.y - targetY);
                    float taper = 1.0 - pow(abs(p.x) / arcW, 1.4);
                    float th = _Thickness * (taper * 0.7 + 0.3);

                    if (dist < th)
                    {
                        whiteStroke = (1.0 - dist / th) * taper;
                    }

                    // Inky dark shadow slightly inside the white arch
                    float shadowY = targetY - th * 1.3;
                    float distShadow = abs(p.y - shadowY);
                    if (distShadow < th * 1.2)
                    {
                        darkShadow = (1.0 - distShadow / (th * 1.2)) * taper * 0.7;
                    }
                }

                // Secondary smaller inner ripple arc
                float arcW2 = 0.45;
                if (abs(p.x) < arcW2)
                {
                    float targetY2 = -_Curvature * 1.2 * (p.x * p.x) + 0.02;
                    float dist2 = abs(p.y - targetY2);
                    float taper2 = 1.0 - pow(abs(p.x) / arcW2, 1.4);
                    float th2 = _Thickness * 0.7 * (taper2 * 0.7 + 0.3);

                    if (dist2 < th2)
                    {
                        whiteStroke += (1.0 - dist2 / th2) * taper2 * 0.75;
                    }
                }

                // Composite: shadow underneath white chalk
                half4 col = lerp(half4(_ShadowColor.rgb, 0.0), _ShadowColor, saturate(darkShadow));
                col = lerp(col, _BaseColor, saturate(whiteStroke));

                float finalAlpha = saturate(whiteStroke + darkShadow) * input.color.a * _BaseColor.a;
                col.a = finalAlpha;

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
