Shader "Custom/ground"
{
    Properties
    {
		[Header(Color setting)]
		_Grass("Grass", Color) = (1,1,1,1)
		_Stone("Stone", Color) = (0,0,0,1)

		[Header(Texture setting)]
		[MaterialToggle] 
		[hideInInspector]
		_UseText("Use texture ", Float) = 0
		[hideInInspector]
		_GrassTex("Grass texture (RGB)", 2D) = "white" {}
		[hideInInspector]
		_StoneTex("Stone texture (RGB)", 2D) = "black" {}

		[Header(Blend setting)]
		_Offset("Offset", Range(-1,1)) = 0.0
		_Strengh("Strengh", Range(0,1)) = 0.5

		[Header(bands)]
		_size("Bands size", Range(0,2)) = 1.0

		[hideInInspector]
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		[hideInInspector]
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[hideInInspector]
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
        };

		float _UseText;
		sampler2D _GrassTex;
		sampler2D _StoneTex;
        half _Glossiness;
        half _Metallic;
		float4 _Grass;
		float4 _Stone;
		float _Offset, _Strengh, _size;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		// for more info on how the function look : https://www.desmos.com/calculator/qceqjaoil3
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // calculate the blender between bot color/texutre 
			// here we use a smoothstep so we get a smooth/hard (depends on setting)
			// between 0. and 1., this value is used in the lerp function to blend color/texture
			float3 m = smoothstep(0., 1., (dot(IN.worldNormal, float3(0,1,0)) + _Offset) / (1-_Strengh));

			// mix texture of color together
			// use the m for a gradual mix
			_Stone.xyz += clamp(
				sin(
					IN.worldPos.y * _size + 
					cos(IN.worldPos.x * _size) * 0.1 + 
					sin(IN.worldPos.z * _size) * 0.1
				),
				0.0, 0.1);
            float3 c = lerp(_Stone, _Grass, m);

			float3 t = lerp(tex2D(_StoneTex, IN.uv_MainTex), tex2D(_GrassTex, IN.uv_MainTex),m);

			//if we should use "c" color or "t" texutre since _UseText is only 0 or 1
			o.Albedo = lerp(c, t, _UseText);

			// not usefull as of now
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
