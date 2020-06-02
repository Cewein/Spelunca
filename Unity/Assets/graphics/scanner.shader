Shader "Hidden/scanner"
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
				float2 depth : TEXCOORD0;

            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				UNITY_TRANSFER_DEPTH(o.depth);
                return o;
            }

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			float distanceFromCam;
			float width;

            fixed4 frag (v2f i) : SV_Target
            {
				//get the texture of the screen
                fixed4 col = tex2D(_MainTex, i.uv);

				//get the depth for the gien pixel
				float depth = tex2D(_CameraDepthTexture, i.uv).r;

				//linear depth between camera and far clipping plane
				depth = Linear01Depth(depth) * _ProjectionParams.z;

				//render the scanner line
				if (depth < distanceFromCam && depth >(distanceFromCam - width))
				{
					//TODO add other stuff for better scanner
					float3 neg = 1.0 - col.rgb;
					float m = smoothstep((distanceFromCam - width), distanceFromCam, depth);
					col.rgb = lerp(col.rgb, neg, m);
				}
                return col;
            }
            ENDCG
        }
    }
}
