Shader "Shader/Ball"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _BaseColor ("Base Color", Color) = (0.45, 0.75, 1.0, 1.0)
        _CoreColor ("Core Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _GlowColor ("Glow Color", Color) = (0.35, 0.8, 1.0, 1.0)

        _Radius ("Radius (0~0.5)", Range(0.05, 0.5)) = 0.48
        _EdgeSoftness ("Edge Softness", Range(0.0001, 0.2)) = 0.06

        _CorePower ("Core Power", Range(0.1, 8.0)) = 2.2
        _CoreIntensity ("Core Intensity", Range(0.0, 5.0)) = 1.2

        _GlowWidth ("Glow Width", Range(0.0, 0.5)) = 0.12
        _GlowSoftness ("Glow Softness", Range(0.0001, 0.5)) = 0.18
        _GlowIntensity ("Glow Intensity", Range(0.0, 6.0)) = 1.5

        _Alpha ("Overall Alpha", Range(0.0, 1.0)) = 1.0

        // --- UI Stencil (copy from UI/Default style) ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15

        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "OrbGlow"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _BaseColor;
            fixed4 _CoreColor;
            fixed4 _GlowColor;

            float _Radius;
            float _EdgeSoftness;

            float _CorePower;
            float _CoreIntensity;

            float _GlowWidth;
            float _GlowSoftness;
            float _GlowIntensity;

            float _Alpha;

            float4 _ClipRect;
            float _UseUIAlphaClip;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 读取 sprite alpha 作为形状（圆形遮罩）
                fixed4 tex = tex2D(_MainTex, i.uv);
                float spriteA = tex.a;

                // UI 裁剪（RectMask2D）
                float2 pos = i.worldPos.xy;
                float clipFactor = UnityGet2DClipping(pos, _ClipRect);

                // 以 UV 中心为圆心做径向距离
                float2 p = i.uv - 0.5;
                // 如果你的贴图不是正方形，可以按需做比例修正（可选）
                // p.x *= _MainTex_TexelSize.y / _MainTex_TexelSize.x;

                float d = length(p);

                // --- 圆盘主体：radius 内为 1，向边缘用 smoothstep 模糊过渡 ---
                float inner = 1.0 - smoothstep(_Radius - _EdgeSoftness, _Radius, d);

                // --- 核心高光：越靠中心越亮（可调 power） ---
                float core = pow(saturate(1.0 - d / max(_Radius, 1e-5)), _CorePower) * _CoreIntensity;

                // --- 外圈光晕：在 radius 外面一段范围出现并渐隐 ---
                float glowStart = _Radius;
                float glowEnd   = _Radius + _GlowWidth;
                // 进入 glow 区间的权重（0->1）
                float glowIn = smoothstep(glowStart, glowStart + _GlowSoftness, d);
                // 离开 glow 区间的权重（1->0）
                float glowOut = 1.0 - smoothstep(glowEnd - _GlowSoftness, glowEnd, d);
                float glow = saturate(glowIn * glowOut) * _GlowIntensity;

                // 用贴图 alpha 约束形状：主体用 spriteA，光晕也乘一点 spriteA 以避免方形边缘露出
                // 如果你的贴图留边足够透明，halo 会更好看
                float shape = spriteA;

                // 颜色合成：主体颜色 + 核心加亮 + 外圈 halo
                float3 col = 0;
                col += _BaseColor.rgb * inner;
                col += _CoreColor.rgb * core * inner;  // 核心主要在主体内部更自然
                col += _GlowColor.rgb * glow;          // halo 可在外面

                // alpha：主体 alpha + halo alpha（halo 通常更透明）
                float a = (inner * shape) + (glow * 0.35 * shape);
                a *= _Alpha;

                fixed4 outCol = fixed4(col, a);

                // 乘上 UI 顶点色（Image.color）
                outCol *= i.color;

                // UI clip
                outCol.a *= clipFactor;

                // 可选：AlphaClip（用于某些 UI 场景的硬裁边）
                if (_UseUIAlphaClip > 0.5)
                    clip(outCol.a - 0.001);

                return outCol;
            }
            ENDCG
        }
    }
}
