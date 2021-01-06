Shader "Unlit/FlatGradientShader"
{
    Properties
    {
        _Clip ("Clip", Range (0, 1)) = 1
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        BlendOp Add

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;
            float _Clip;

            struct appdata
            {
                float2 uv : TEXCOORD0;
                float4 vertex: POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                if (col.r < _Clip)
                    return half4(0, 0, 0, col.a);
                return half4(1, 1, 1, col.a);
            }

            ENDCG
        }
    }
}
