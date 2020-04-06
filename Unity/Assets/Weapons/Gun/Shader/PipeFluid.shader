Shader "Custom/Fluid"
{
    Properties
    {
		_Tint ("Tint", Color) = (1,1,1,1)
        
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
            float3 normal : COLOR2;	//les normales de l'objet
            };
            
            float4 _Tint; // les couleurs du shader
            
            v2f vert (appdata v)
            {
                v2f o; // les données qui vont être passées au fragment shader
                o.vertex = UnityObjectToClipPos(v.vertex); //passe de object space au clip space de la camera
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }
                
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
       
                fixed4 col = _Tint;
                
                return col;    
            }
            ENDCG
        }
    }
}
