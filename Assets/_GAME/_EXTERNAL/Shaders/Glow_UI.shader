Shader "UI/Glow"
{
    Properties
    {
        [HDR] _GlowColor ("Glow Color", Color) = (0, 1, 1, 1)

        _BoxSize ("Inner Box Size", Range(0.1, 0.99)) = 0.8
        _Roundness ("Corner Roundness", Range(0.0, 0.5)) = 0.15

        _GlowSpread ("Glow Spread", Range(0.01, 2.0)) = 0.5
        _GlowPower ("Glow Intensity", Range(0.0, 10.0)) = 2.0

        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "False"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        // Обычная прозрачность UI
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            ColorMask [_ColorMask]

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 color : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _GlowColor;

            float _BoxSize;
            float _Roundness;
            float _GlowSpread;
            float _GlowPower;

            float4 _ClipRect;

            float _Stencil;
            float _StencilComp;
            float _StencilOp;
            float _StencilWriteMask;
            float _StencilReadMask;
            float _ColorMask;


            v2f vert(appdata_t v)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.worldPosition = v.vertex;
                OUT.color = v.color;

                return OUT;
            }


            // ------------------------------------------------------------
            // Signed Distance Field for rounded rectangle
            // ------------------------------------------------------------

            float sdRoundBox(float2 p, float2 halfSize, float radius)
            {
                float2 q = abs(p) - halfSize + radius;

                return length(max(q, 0.0))
                     + min(max(q.x, q.y), 0.0)
                     - radius;
            }


            fixed4 frag(v2f IN) : SV_Target
            {
                // UV: 0..1
                // Переводим в диапазон -1..1
                float2 p = IN.texcoord * 2.0 - 1.0;


                // --------------------------------------------------------
                // Форма
                // --------------------------------------------------------

                float2 halfSize = float2(
                    _BoxSize,
                    _BoxSize
                );

                float dist = sdRoundBox(
                    p,
                    halfSize,
                    _Roundness
                );


                // --------------------------------------------------------
                // Glow
                // --------------------------------------------------------

                // Нас интересует только область СНАРУЖИ формы.
                float outsideDistance = max(dist, 0.0);

                // Экспоненциальное затухание.
                float glow = exp(
                    -outsideDistance *
                    (10.0 / max(_GlowSpread, 0.001))
                );


                // Внутри формы glow не рисуем.
                float outsideMask = step(0.0, dist);

                glow *= outsideMask;


                // Интенсивность Glow.
                glow *= _GlowPower;

                glow = saturate(glow);


                // --------------------------------------------------------
                // Soft edge
                // --------------------------------------------------------

                // Немного сглаживаем границу самой формы.
                float edge = smoothstep(
                    0.0,
                    0.02,
                    dist
                );

                glow *= edge;


                // --------------------------------------------------------
                // UI RectMask2D
                // --------------------------------------------------------

                #if UNITY_UI_CLIP_RECT

                    glow *= UnityGet2DClipping(
                        IN.worldPosition.xy,
                        _ClipRect
                    );

                #endif


                // --------------------------------------------------------
                // Canvas / Graphic alpha
                // --------------------------------------------------------

                glow *= IN.color.a;


                // --------------------------------------------------------
                // Alpha clipping
                // --------------------------------------------------------

                #if UNITY_UI_ALPHACLIP

                    clip(glow - 0.001);

                #endif


                // --------------------------------------------------------
                // Final color
                // --------------------------------------------------------

                // RGB отвечает за цвет/яркость.
                // Alpha отвечает только за прозрачность Glow.
                float3 finalColor =
                    _GlowColor.rgb * _GlowPower;


                return fixed4(
                    finalColor,
                    glow
                );
            }

            ENDCG
        }
    }
}