Shader "Custom/GradientShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)
        _BottomColor ("Bottom Color", Color) = (0, 0, 0, 1)
        _Brightness ("Brightness", Float) = 1.2
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _ColorTemperature ("Color Temperature", Range(0, 1)) = 0.8
        _EmissionColor ("Emission Color", Color) = (1, 0.8, 0.6, 1)
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.1
        _NoiseScale ("Noise Scale", Float) = 10
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        fixed4 _TopColor;
        fixed4 _BottomColor;
        float _Brightness;
        float _Smoothness;
        float _ColorTemperature;
        fixed4 _EmissionColor;
        float _NoiseStrength;
        float _NoiseScale;

        struct Input
        {
            float2 uv_MainTex;
        };

        // 노이즈 함수 (Perlin 노이즈 사용)
        float noise(float2 UV)
        {
            return frac(sin(dot(UV ,float2(12.9898,78.233))) * 43758.5453);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

            // 그라데이션 값 계산 및 스무딩
            float gradientValue = smoothstep(0, 1, IN.uv_MainTex.y);

            // 색상 보간 및 밝기 조절
            fixed4 gradientColor = lerp(_BottomColor, _TopColor, gradientValue);
            fixed4 finalColor = lerp(texColor, gradientColor, _Brightness);

            // 색온도 적용
            fixed4 colorTemp = lerp(fixed4(1, 0.9, 0.8, 1), fixed4(0.8, 0.9, 1, 1), _ColorTemperature);
            finalColor *= colorTemp;

            // 노이즈 추가
            float n = noise(IN.uv_MainTex * _NoiseScale) * _NoiseStrength;
            finalColor += n;

            // 발광 효과
            o.Emission = _EmissionColor * finalColor;

            o.Albedo = finalColor.rgb;
            o.Alpha = texColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
