Shader "Custom/PlayerDistortEffect"
{
    Properties
    {
        _BlurAmount      ("Blur Amount",      Range(0, 1))    = 0
        _VignetteAmount  ("Vignette Amount",  Range(0, 1))    = 0
        _ChromaticAmount ("Chromatic Amount", Range(0, 0.05)) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "PlayerDistortPass"
            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _BlurAmount;
            float _VignetteAmount;
            float _ChromaticAmount;

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                // --- Chromatic aberration ---
                float2 ca = (uv - 0.5) * _ChromaticAmount;
                half r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + ca).r;
                half g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv     ).g;
                half b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv - ca).b;
                half4 col = half4(r, g, b, 1);

                // --- Box blur (9-tap) ---
                if (_BlurAmount > 0.001)
                {
                    float s = _BlurAmount * 0.008;
                    half4 blur = 0;
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-s, -s));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0, -s));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( s, -s));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-s,  0));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv                 );
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( s,  0));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-s,  s));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0,  s));
                    blur += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( s,  s));
                    blur /= 9.0;
                    col = lerp(col, blur, _BlurAmount);
                }

                // --- Vignette ---
                float2 vigUV = uv - 0.5;
                float vig = 1.0 - dot(vigUV, vigUV) * 4.0 * _VignetteAmount;
                col.rgb *= saturate(vig);

                return col;
            }
            ENDHLSL
        }
    }
}
