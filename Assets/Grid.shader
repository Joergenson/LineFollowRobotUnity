Shader "Custom/Grid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineWidth ("Line Width", Vector) = (0.1, 0.1, 0, 0)
        _GridScale ("Grid Scale", Float) = 10.0
        _InvertMat ("Invert Material", Int) = 1
        _GridColor ("Grid Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _LineWidth;
            float _GridScale;
            int _InvertMat;
            float4 _GridColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 fract(float2 value)
            {
                return value - floor(value);
            }

            float pristineGrid(float2 uv, float2 lineWidth)
            {
                float2 uvDeriv = float2(ddx(uv).x, ddy(uv).y);
                bool2 invertLine = bool2(lineWidth.x > 0.5, lineWidth.y > 0.5);
                float2 targetWidth = float2(
                    invertLine.x ? 1.0 - lineWidth.x : lineWidth.x,
                    invertLine.y ? 1.0 - lineWidth.y : lineWidth.y
                );
                float2 drawWidth = clamp(targetWidth, uvDeriv, float2(0.5, 0.5));
                float2 lineAA = uvDeriv * 1.5;
                float2 gridUV = abs(fract(uv) * 2.0 - 1.0);
                gridUV.x = invertLine.x ? gridUV.x : 1.0 - gridUV.x;
                gridUV.y = invertLine.y ? gridUV.y : 1.0 - gridUV.y;
                float2 grid2 = smoothstep(drawWidth + lineAA, drawWidth - lineAA, gridUV);

                grid2 *= clamp(targetWidth / drawWidth, 0.0, 1.0);
                grid2 = lerp(grid2, targetWidth, clamp(uvDeriv * 2.0 - 1.0, 0.0, 1.0));
                grid2.x = invertLine.x ? 1.0 - grid2.x : grid2.x;
                grid2.y = invertLine.y ? 1.0 - grid2.y : grid2.y;
                return lerp(grid2.x, 1.0, grid2.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * _GridScale;
                float gridValue = pristineGrid(uv, _LineWidth);

                fixed4 gridColor = _GridColor;
                if (_InvertMat == 1)
                {
                    gridColor = 1.0 - gridColor;
                    gridValue = 1.0 - gridValue;
                }

                gridColor.a = gridValue;
                return gridColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}