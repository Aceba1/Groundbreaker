// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/InvertColors"
{
    Properties
    {
        _Color("Tint", Color) = (1,1,1,1)
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
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

        Pass
        {
           ZWrite On
           ColorMask 0
        }
        
        Blend OneMinusDstColor OneMinusSrcAlpha
        BlendOp Add

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            uniform float4 _Color ;
            sampler2D _MainTex;

            struct vertexInput
            {
                float2 uv : TEXCOORD0;
                float4 vertex: POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(vertexInput i)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;

                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 texColor = tex2D(_MainTex, i.uv);

                return half4(texColor.a, texColor.a, texColor.a, texColor.a);
            }

            ENDCG
        }
    }
}