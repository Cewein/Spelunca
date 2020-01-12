//
// shader made by Maximilien "cewein" nowak
// ported from shadertoy to Unity
// https://www.shadertoy.com/view/WtGGW1
//

Shader "Hidden/CelShadingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			float2 iResolution;

			float pixelIntensity(float3 color)
			{
				return sqrt(color.x*color.x + color.y*color.y + color.z*color.z);
			}


			float mag(float2 pixel, float2 spacing)
			{
				float tl = tex2D(_MainTex, pixel + float2(-spacing.x, spacing.y)).rgb;
				float tm = tex2D(_MainTex, pixel + float2(0, spacing.y)).rgb;
				float tr = tex2D(_MainTex, pixel + float2(spacing.x, spacing.y)).rgb;

				float ml = tex2D(_MainTex, pixel + float2(-spacing.x, 0)).rgb;
				float mr = tex2D(_MainTex, pixel + float2(spacing.x, 0)).rgb;

				float bl = tex2D(_MainTex, pixel + float2(-spacing.x, -spacing.y)).rgb;
				float bm = tex2D(_MainTex, pixel + float2(0, -spacing.y)).rgb;
				float br = tex2D(_MainTex, pixel + float2(spacing.x, -spacing.y)).rgb;

				float x = tl + 2. * ml + bl - tr - 2. * mr - br;
				float y = tl + 2. * tm + tr - bl - 2. * bm - br;

				return sqrt((x*x) + (y*y));
			}

			float space = 1.0;

			float4 mainImage(in float2 uv)
			{
				float magnitude = mag(uv, float2(space/iResolution.x, space/iResolution.y));

				float3 col = magnitude;

				return float4(col, 1.);
			}

			float4 frag (v2f i) : SV_Target
            {
				float4 col = fixed4(i.uv.x,i.uv.y,0,0);

				col = tex2D(_MainTex,i.uv);

                return col;
            }
            ENDCG
        }
    }
}
