Shader "Hidden/ParticleRendere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ParticleTex("OverLay Texture", 2D) = "white" {}
		_Radius("Radius", float) = 0.2
		_PosX("PosX", float) = 0
		_PosY("PosY", float) = 0
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
			sampler2D _ParticleTex;
			float _PosX;
			float _PosY;
			float _Radius;


			float remap_value(float value,float old_min,float old_max,float new_min, float new_max) 
			{
				return clamp(new_min + (value - old_min)*(new_max - new_min) / (old_max - old_min),0,1);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

				fixed2 OverlayUV = (
					fixed2(
						remap_value(i.uv.x, _PosX - _Radius, _PosX + _Radius, 0, 1), 
						remap_value(i.uv.y, _PosY - _Radius, _PosY + _Radius, 0, 1)
					)
				);

				col += tex2D(_ParticleTex, OverlayUV);
                return col;
            }
            ENDCG
        }
    }
}
