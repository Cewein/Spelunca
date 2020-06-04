Shader "Custom/triplanarGround"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_Tex1("Text 1 (RGB)", 2D) = "white" {}
		_Tex2("Text 2 (RGB)", 2D) = "white" {}
		_Tex3("Text 3 (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		tex_scale("tex scale", range(0,1)) = 1.0
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

		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float tex_scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			// Determine the blend weights for the 3 planar projections. 
			// N_orig is the vertex-interpolated normal vector. 
			float3 blend_weights = abs(IN.worldNormal);
			// Tighten up the blending zone: 
			blend_weights = (blend_weights - 0.2) * 7; 
			blend_weights = max(blend_weights, 0);      
			// Force weights to sum to 1.0 (very important!) 
			blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z ).xxx; 
			// Now determine a color value and bump vector for each of the 3 
			// projections, blend them, and store blended results in these two 
			// vectors: 
			float4 blended_color; 
			// .w hold spec value 
			float3 blended_bump_vec; 
			
			// Compute the UV coords for each of the 3 planar projections. 
			// tex_scale (default ~ 1.0) determines how big the textures appear. 
			float2 coord1 = IN.worldPos.yz * tex_scale;
			float2 coord2 = IN.worldPos.zx * tex_scale;
			float2 coord3 = IN.worldPos.xy * tex_scale;
			// This is where you would apply conditional displacement mapping. 
			//if (blend_weights.x > 0) coord1 = . . . 
			//if (blend_weights.y > 0) coord2 = . . . 
			//if (blend_weights.z > 0) coord3 = . . . 
			// Sample color maps for each projection, at those UV coords. 
			float4 col1 = tex2D(_Tex1, coord1);
			float4 col2 = tex2D(_Tex2, coord2);
			float4 col3 = tex2D(_Tex3, coord3);
			// Sample bump maps too, and generate bump vectors. 
			// (Note: this uses an oversimplified tangent basis.) 
			//float2 bumpFetch1 = bumpTex1.Sample(coord1).xy - 0.5; 
			//float2 bumpFetch2 = bumpTex2.Sample(coord2).xy - 0.5;  
			//float2 bumpFetch3 = bumpTex3.Sample(coord3).xy - 0.5;  
			//float3 bump1 = float3(0, bumpFetch1.x, bumpFetch1.y);  
			//float3 bump2 = float3(bumpFetch2.y, 0, bumpFetch2.x);  
			//float3 bump3 = float3(bumpFetch3.x, bumpFetch3.y, 0);  
			//// Finally, blend the results of the 3 planar projections. 
			blended_color = col1.xyzw * blend_weights.xxxx +                
				col2.xyzw * blend_weights.yyyy +                 
				col3.xyzw * blend_weights.zzzz;
			//blended_bump_vec = bump1.xyz * blend_weights.xxx + 
			//	bump2.xyz * blend_weights.yyy + 
			//	bump3.xyz * blend_weights.zzz; 
			
			// Apply bump vector to vertex-interpolated normal vector. 
			//float3 N_for_lighting = normalize(N_orig + blended_bump);
            
			
			// Albedo comes from a texture tinted by color
            fixed4 c = blended_color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
