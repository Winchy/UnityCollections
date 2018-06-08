// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Nebula"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DarkTex("Dark Texture", 2D) = "white" {}
		_Shimmer("Shimmer Noise", 2D) = "white" {} 
		_Color("Nebula Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Pass
		{
			Tags {"RenderType"="Opaque" }
			LOD 100

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Shimmer;
			sampler2D _DarkTex;
			float4 _DarkTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed2 uv = TRANSFORM_TEX(i.uv, _MainTex);
				fixed2 uv2 = i.uv - float2(0.5, 0.5);
				uv2 = fixed2(uv2.x * _CosTime.x - uv2.y * _SinTime.x, uv2.x * _SinTime.x + uv2.y * _CosTime.x);
				uv2 = uv2 + float2(0.5, 0.5);
				uv = uv + tex2D(_Shimmer, uv2).xy * 0.03;
				fixed4 col = (tex2D(_MainTex, uv))   * _Color;
				uv = TRANSFORM_TEX(i.uv, _DarkTex);
				fixed4 darkColor = tex2D(_DarkTex, uv);
				//return tex2D(_Shimmer, uv2);
				return fixed4(col.rgb - darkColor.rgb, col.a);
			}
			ENDCG
		}
	}
}
