Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _WallColor ("WallColor", Color) = (1,1,1,1)
        _GroundColor ("GroundColor", Color) = (1,1,1,1)
        _EmissionColor ("EmissionColor", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WallTex ("WallTexture (RGB)", 2D) = "white" {}
        _GroundTex ("GroundTexture (RGB)", 2D) = "white" {}
        _ScaleWall("Scale Wall", Range(0.01, 10)) = 1
        _ScaleGround("Scale Ground", Range(0.01, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _WallTex;
        sampler2D _GroundTex;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            //float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        half _ScaleGround;
        half _ScaleWall;
        fixed4 _EmissionColor;
        fixed4 _WallColor;
        fixed4 _GroundColor;
        fixed4 _EmissionLM;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c;
            fixed4 d;

            if (abs(IN.worldNormal.x) > 0.5)
            {
                c = tex2D(_WallTex, IN.worldPos.zy / _ScaleWall) * _WallColor;
            }
            else if (abs(IN.worldNormal.z) > 0.5)
            {
                c =   tex2D(_WallTex, IN.worldPos.xy / _ScaleWall) * _WallColor;
            }
            else
            {
                c = tex2D(_GroundTex, IN.worldPos.xz / _ScaleGround) * _GroundColor;
            }

            if (abs(IN.worldNormal.x) > 0.5)
            {
                d = tex2D(_WallTex, IN.worldPos.zy / _ScaleWall) * _EmissionColor;
            }
            else if (abs(IN.worldNormal.z) > 0.5)
            {
                d = tex2D(_WallTex, IN.worldPos.xy / _ScaleWall) * _EmissionColor;
            }
            else
            {
                d = tex2D(_GroundTex, IN.worldPos.xz / _ScaleGround) * _EmissionColor;
            }

            

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = d.rgb;
            //o.BumpMap = _BumpMap;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
