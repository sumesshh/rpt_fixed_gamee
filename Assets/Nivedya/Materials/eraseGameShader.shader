Shader "Custom/eraseGameShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushTex ("Brush Texture", 2D) = "white" {}
        _ErasePosition ("Erase Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
            sampler2D _BrushTex;
            float4 _ErasePosition;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                float4 brush = tex2D(_BrushTex, i.uv - _ErasePosition.xy);

                // Gradually erase instead of instantly making it transparent
                float eraseAmount = brush.r * 0.4; // Adjust erasing strength
                color.a = max(0, color.a - eraseAmount); 

                return color;
            }
            ENDCG
        }
    }
}
