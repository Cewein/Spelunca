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
		_Space ("Spacing", Range(0,0.1)) = 0.05
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
			float _Space;
			float2 iResolution;
			float intensity;
			float center;
            float smoothness;
            int debugMode;
            float spacing;
            float lowPassFilter;
            
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

			float3 mainImage(in float2 uv)
			{
			    float3 col = tex2D(_MainTex, uv).rgb;
			    float luminosity = (col.r + col.g + col.b)/3;
			    if(luminosity<=lowPassFilter){
			        float magnitude = mag(uv, float2(spacing/100, spacing/100)); //0.001, 0.002
                    float bottom = center - smoothness;
                    float top = center + smoothness;
                    //on empeche les valeurs de dépasser [0,1]
                    bottom = bottom < 0 ? 0 : bottom; 
                    top = top > 1 ? 1 : top; 
                    magnitude = smoothstep(bottom,top,magnitude);
                    col = magnitude;
				    return col;
                }else{
                    return (0,0,0);
                }
			}

			float4 frag (v2f i) : SV_Target
            {
                float3 col = (0,0,0);
                float3 magnitude = mainImage(i.uv);
                if(debugMode == 1){
                    col = (1. - magnitude);
                }else{
                    col = tex2D(_MainTex, i.uv).rgb * (1. - magnitude*intensity);
                }
				//float3 col = tex2D(_MainTex, i.uv).rgb * (1. - magnitude);
				//float3 col = (1. - magnitude);//tex2D(_MainTex, i.uv).rgb * 

                return float4(col.x, col.y, col.z,1.);
            }
            ENDCG
        }
    }
}
