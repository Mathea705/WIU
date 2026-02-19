Shader "Custom/WaterSurface"
{
    Properties
    {
        [Header(Color and Depth)]
        _ShallowColor       ("Shallow Color",        Color)   = (0.10, 0.55, 0.55, 0.12)
        _DeepColor          ("Deep Color",           Color)   = (0.02, 0.08, 0.22, 0.90)
        _DepthMaxDistance   ("Depth Distance",       Float)   = 3.0

        [Header(Waves)]
        _WaveAmplitude      ("Wave Amplitude",       Float)   = 0.15
        _WaveFrequency      ("Wave Frequency",       Float)   = 1.5
        _WaveSpeed          ("Wave Speed",           Float)   = 1.2
        _WaveDir1           ("Wave Dir 1 (XY)",      Vector)  = (1.0, 0.7, 0, 0)
        _WaveDir2           ("Wave Dir 2 (XY)",      Vector)  = (-0.5, 1.0, 0, 0)

        [Header(Normal Maps)]
        _NormalMap          ("Normal Map A",         2D)      = "bump" {}
        _NormalMap2         ("Normal Map B",         2D)      = "bump" {}
        _NormalStrength     ("Normal Strength",      Float)   = 1.2
        _ScrollSpeedA       ("Scroll Speed A (XY)",  Vector)  = (0.03,  0.02, 0, 0)
        _ScrollSpeedB       ("Scroll Speed B (XY)",  Vector)  = (-0.02, 0.05, 0, 0)

        [Header(Refraction)]
        _RefractionStrength ("Refraction Strength",  Float)   = 0.04

        [Header(Ripple)]
        _RippleTex          ("Ripple Texture",       2D)      = "black" {}
        _RippleStrength     ("Ripple Strength",      Float)   = 4.0

        [Header(Foam)]
        _FoamColor          ("Foam Color",           Color)   = (1, 1, 1, 1)
        _FoamEdgeDistance   ("Edge Foam Distance",   Float)   = 0.35
        _FoamWakeThreshold  ("Wake Foam Threshold",  Float)   = 0.08

        [Header(Fresnel and Specular)]
        _FresnelPower       ("Fresnel Power",        Float)   = 5.0
        _SpecularStrength   ("Specular Strength",    Float)   = 0.7
        _Smoothness         ("Smoothness",           Range(0,1)) = 0.92
    }

    SubShader
    {
        Tags
        {
            "RenderType"      = "Transparent"
            "Queue"           = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_NormalMap);  SAMPLER(sampler_NormalMap);
            TEXTURE2D(_NormalMap2); SAMPLER(sampler_NormalMap2);
            TEXTURE2D(_RippleTex);  SAMPLER(sampler_RippleTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float  _DepthMaxDistance;

                float  _WaveAmplitude;
                float  _WaveFrequency;
                float  _WaveSpeed;
                float4 _WaveDir1;
                float4 _WaveDir2;

                float4 _NormalMap_ST;
                float4 _NormalMap2_ST;
                float4 _RippleTex_ST;
                float  _NormalStrength;
                float4 _ScrollSpeedA;
                float4 _ScrollSpeedB;

                float  _RefractionStrength;
                float  _RippleStrength;

                float4 _FoamColor;
                float  _FoamEdgeDistance;
                float  _FoamWakeThreshold;

                float  _FresnelPower;
                float  _SpecularStrength;
                float  _Smoothness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float2 uv          : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float3 tangentWS   : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float4 screenPos   : TEXCOORD5;
                float  fogFactor   : TEXCOORD6;
            };

            float3 BlendNormals(float3 a, float3 b)
            {
                return normalize(float3(a.xy + b.xy, a.z * b.z));
            }

            void AddWave(float2 dir, float amplitude, float frequency, float speed,
                         float2 posXZ, inout float disp, inout float dx, inout float dz)
            {
                dir = normalize(dir);
                float phase = dot(dir, posXZ) * frequency + _Time.y * speed;
                float c = cos(phase);
                disp += amplitude * sin(phase);
                dx   += amplitude * frequency * dir.x * c;
                dz   += amplitude * frequency * dir.y * c;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);

                float disp = 0.0, dx = 0.0, dz = 0.0;
                AddWave(_WaveDir1.xy, _WaveAmplitude,        _WaveFrequency,        _WaveSpeed,       posWS.xz, disp, dx, dz);
                AddWave(_WaveDir2.xy, _WaveAmplitude * 0.6,  _WaveFrequency * 1.4,  _WaveSpeed * 0.8, posWS.xz, disp, dx, dz);
                posWS.y += disp;

                float3 waveNormalWS = normalize(float3(-dx, 1.0, -dz));

                OUT.positionCS  = TransformWorldToHClip(posWS);
                OUT.positionWS  = posWS;
                OUT.uv          = IN.uv;

                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS    = waveNormalWS;
                OUT.tangentWS   = normInputs.tangentWS;
                OUT.bitangentWS = normalize(cross(waveNormalWS, normInputs.tangentWS)) * IN.tangentOS.w;

                OUT.screenPos   = ComputeScreenPos(OUT.positionCS);
                OUT.fogFactor   = ComputeFogFactor(OUT.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // ── Normal maps ──────────────────────────────────────────────
                float2 uvA = IN.uv * _NormalMap_ST.xy  + _NormalMap_ST.zw  + _ScrollSpeedA.xy * _Time.y;
                float2 uvB = IN.uv * _NormalMap2_ST.xy + _NormalMap2_ST.zw + _ScrollSpeedB.xy * _Time.y;
                float3 nA = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap,  sampler_NormalMap,  uvA));
                float3 nB = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap2, sampler_NormalMap2, uvB));
                float3 surfaceNormal = BlendNormals(nA, nB);
                surfaceNormal.xy *= _NormalStrength;
                surfaceNormal = normalize(surfaceNormal);

                // ── Ripple normal ────────────────────────────────────────────
                float2 rippleUV = IN.uv * _RippleTex_ST.xy + _RippleTex_ST.zw;
                float2 px = float2(0.002, 0.0);
                float2 py = float2(0.0,   0.002);
                float h0 = SAMPLE_TEXTURE2D(_RippleTex, sampler_RippleTex, rippleUV     ).r;
                float hR = SAMPLE_TEXTURE2D(_RippleTex, sampler_RippleTex, rippleUV + px).r;
                float hU = SAMPLE_TEXTURE2D(_RippleTex, sampler_RippleTex, rippleUV + py).r;
                float3 rippleNormal = normalize(float3((h0-hR)*_RippleStrength,
                                                       (h0-hU)*_RippleStrength, 1.0));

                float3 tangentNormal = BlendNormals(surfaceNormal, rippleNormal);
                float3x3 TBN = float3x3(normalize(IN.tangentWS),
                                        normalize(IN.bitangentWS),
                                        normalize(IN.normalWS));
                float3 worldNormal = normalize(mul(tangentNormal, TBN));
                float3 viewDir     = normalize(GetWorldSpaceViewDir(IN.positionWS));

                // ── Depth ────────────────────────────────────────────────────
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float  rawDepth = SampleSceneDepth(screenUV);
                float  sceneZ   = LinearEyeDepth(rawDepth, _ZBufferParams);
                float  surfaceZ = IN.screenPos.w;
                float  depthDiff = saturate((sceneZ - surfaceZ) / _DepthMaxDistance);

                // ── Refraction ───────────────────────────────────────────────
                // Distort screen UV using tangent-space normal; fade distortion in deep water
                float2 refrOffset  = tangentNormal.xy * _RefractionStrength * (1.0 - depthDiff);
                float2 refractedUV = clamp(screenUV + refrOffset, 0.001, 0.999);

                // Safety: if refracted sample is in front of the surface, use straight UV
                float refractDepthRaw = SampleSceneDepth(refractedUV);
                float refractSceneZ   = LinearEyeDepth(refractDepthRaw, _ZBufferParams);
                float2 finalUV        = (refractSceneZ < surfaceZ) ? screenUV : refractedUV;

                float3 sceneColor = SampleSceneColor(finalUV);

                // ── Water color tint over refracted scene ────────────────────
                // Shallow alpha is very low so the bottom is clearly visible.
                // Deep alpha is high so the water becomes opaque at depth.
                float3 waterTint  = lerp(_ShallowColor.rgb, _DeepColor.rgb, depthDiff);
                float  tintAmount = lerp(_ShallowColor.a,   _DeepColor.a,   depthDiff);

                float3 col = lerp(sceneColor, waterTint, tintAmount);

                // ── Fresnel ──────────────────────────────────────────────────
                float fresnel = pow(1.0 - saturate(dot(worldNormal, viewDir)), _FresnelPower);

                // ── Specular ─────────────────────────────────────────────────
                Light  mainLight = GetMainLight(TransformWorldToShadowCoord(IN.positionWS));
                float3 halfDir   = normalize(viewDir + mainLight.direction);
                float  NdotH     = saturate(dot(worldNormal, halfDir));
                float  specPow   = exp2(_Smoothness * 10.0 + 1.0);
                float3 specular  = mainLight.color * pow(NdotH, specPow) * _SpecularStrength * fresnel;

                col += specular;
                col += fresnel * 0.04;   // subtle rim brightening

                // ── Foam ─────────────────────────────────────────────────────
                float edgeFoam = 1.0 - saturate(depthDiff / max(_FoamEdgeDistance, 0.001));
                float wakeFoam = step(_FoamWakeThreshold, abs(h0));
                float foamMask = saturate(edgeFoam + wakeFoam);
                col = lerp(col, _FoamColor.rgb, foamMask * _FoamColor.a);

                // ── Alpha ─────────────────────────────────────────────────────
                // Keep shallow water nearly transparent; deep water and foam are opaque
                float alpha = saturate(tintAmount + fresnel * 0.15 + foamMask * 0.85);

                col = MixFog(col, IN.fogFactor);
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
