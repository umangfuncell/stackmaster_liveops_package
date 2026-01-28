Shader "Custom/BuiltIn/Sprite_Scroll"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color   ("Tint Color", Color) = (1,1,1,1)
        _SpeedX  ("Scroll Speed X (UV/sec)", Float) = 0.5
        _SpeedY  ("Scroll Speed Y (UV/sec)", Float) = 0.0
        _OffsetX ("Initial Offset X", Float) = 0.0
        _OffsetY ("Initial Offset Y", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _SpeedX;
            float _SpeedY;
            float _OffsetX;
            float _OffsetY;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Multiply vertex color (from SpriteRenderer) with _Color tint
                o.color = v.color * _Color;
                // Apply Unity texture scale/offset (material _ST) to incoming UVs
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // Time: _Time.y = seconds since level load
                float t = _Time.y;
                float2 offset = float2(_OffsetX, _OffsetY) + float2(_SpeedX, _SpeedY) * t;

                float2 uv = IN.uv + offset;

                fixed4 tex = tex2D(_MainTex, uv);
                fixed4 outCol = tex * IN.color;

                return outCol;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
