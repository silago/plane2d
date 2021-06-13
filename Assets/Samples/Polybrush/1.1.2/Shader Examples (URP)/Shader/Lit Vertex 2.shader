    Shader "Custom/StandardVertexEmissive"
    {
        Properties
        {
            _Color ("Color", Color) = (1,1,1,1)
            _MainTex ("Albedo (RGB)", 2D) = "white" {}
        }
        SubShader
        {
            Tags { "RenderType"="Opaque" }
            LOD 200
     
            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0
     
            sampler2D _MainTex;
            fixed4 _Color;
     
            struct Input
            {
                float2 uv_MainTex;
                float4 color : COLOR; //Vertex color
            };
     
            void surf (Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.color;
                o.Albedo = c.rgb;
                o.Emission = c.rgb;
            }
            ENDCG
        }
        FallBack "VertexLit"
    }
