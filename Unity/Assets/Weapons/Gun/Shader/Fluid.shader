Shader "Custom/Fluid"
{
    Properties
    {
		_Tint ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 0.0
        _Rotation ("Rotation", Range(0,360)) = 360.0
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _IsRimAdditive ("Is Rim additive color", Range(0,1)) = 1
		_RimColor ("Rim Color", Color) = (1,1,1,1)
	    _RimPower ("Rim Power", Range(0,10)) = 0.0
	    
        [HideInInspector] _Height ("Height", Float) = 1.0
        
        [HideInInspector] _WobbleX ("WobbleX", Range(-1,1)) = 0.0
		[HideInInspector] _WobbleZ ("WobbleZ", Range(-1,1)) = 0.0
        
    }
    SubShader
    {   
        //Queue permet de dire au moteur quand lancer le shader, ici on lui dit de le lancer dans la queue de géométrie
        //DisableBatching est obligatoire parce que si Draw Call Batching est appelé, la notion d' "object space" est perdue,
        //... ce qui empecherait le liquide de s'animer depuis la position de l'objet. 
        Tags {"Queue"="Geometry"  "DisableBatching" = "True" }
        Pass{//permet de rendre cet objet qu'une seule fois.
        
            Zwrite On
		    Cull Off // Empeche de rendre que les faces visibles, on rends les deux cotés du mesh (utile pour la face du dessus)
            AlphaToMask On //permet de rendre le liquide transparent
            
            CGPROGRAM//début du programme
    
            //on déclare à la compilation le nom de du vertex shader
            #pragma vertex vert
            //pareil mais pour le fragment shader
            #pragma fragment frag
            //une lib Unity
            #include "UnityCG.cginc"
            
            // les données lues en entrée du shader
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;	
            };
            
            //les données passées du vertex shader au fragment shader
            struct v2f
            {
            float2 uv : TEXCOORD0; //les coordonnées UV
            float4 vertex : SV_POSITION; //les coordonnées des vertexs
            float3 viewDir : COLOR; //la direction de la camera
            float3 normal : COLOR2;	//les normales de l'objet
            float fillEdge : TEXCOORD2; //aucune idée pour le moment
            };
            
            float _FillAmount, _WobbleX, _WobbleZ; //respectivement la quantité remplie, et la force du wobble en X et Y
            float4 _TopColor, _RimColor, _Tint; // les couleurs du shader
            float _Rim, _RimPower;//l'effet de lumière sur les bords
            float _Rotation;
            float _IsRimAdditive;
            float _Height;

			float luminance(float3 rgb)
			{
				float3 w = float3(0.2125, 0.7154, 0.0721);
				return clamp(dot(rgb, w),0.,1.);
			}
            
            v2f vert (appdata v)
            {
                v2f o; // les données qui vont être passées au fragment shader
                o.vertex = UnityObjectToClipPos(v.vertex); //passe de object space au clip space de la camera
                o.uv = v.uv;
                // On récupère la position du vertex dans le world space en multipliant la matrice ObjectToWorld avec ses coordonnées
                float3 worldPos = mul (unity_ObjectToWorld, v.vertex.xyz);   
			    // combine rotations with worldPos, based on sine wave from script
			    float3 worldPosAdjusted = worldPos + (worldPos.x  * _WobbleX)+ (worldPos.z* _WobbleZ); 
			    
                //le niveau de liquide
                o.fillEdge = worldPosAdjusted.y + _Height/2 + 0.5 - _FillAmount*_Height;
                //o.fillEdge = worldPos.y + (_FillAmount * 2. * _Scale.y ) - _Scale.y;
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                return o;
            }
                
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
       
                fixed4 col = _Tint;
                
                // rim light
                float dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
                float4 RimResult = smoothstep(0.5, 1.0, dotProduct);
                RimResult *= _RimColor;
                
                float4 result = step(i.fillEdge, 0.5);//step((i.fillEdge), 0.);
                float4 resultColored = result * col;
                
                float4 finalResult = resultColored;	//je pourrais plus tard ajouter une texture ici
				float lum = luminance(_RimColor);

				float mx = step(0.5, _IsRimAdditive);
                float3 res = finalResult.xyz + RimResult;

				//alors qu'ici on fait un fade entre les deux
				float3 res2 = finalResult.xyz*(1-lum) + RimResult*lum;

				finalResult.xyz = lerp(res2, res, mx);
                
                //On calcule la couleur des pixels qui représente le dessus du liquide
                float4 topColor = _TopColor * (result);
                //VFACE retourne une valeur supérieur à 0 si le vertex est face à la cam et l'inverse si on voit la face arriere du vertex
                return facing > 0 ? finalResult: topColor;    
            }
            ENDCG
        }
    }
}
