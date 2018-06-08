// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SkyRefraction"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD0;
				half3 worldPos: TEXCOORD1;
			};

			sampler2D _MainTex;
			fixed4 _BaseColor;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				
				o.worldPos = worldPos;//cos2 * v2 + sin2 * worldNormal;
				o.worldNormal = worldNormal;
				//o.worldRefl = reflect()
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldViewDir = -normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 v1 = cross(i.worldNormal, worldViewDir);
				float3 v2 = cross(v1, i.worldNormal);
				float sin1 = dot(worldViewDir, v2);
				float sin2 = clamp(sin1 * 1.2f, 0, 1);
				float cos2 = sqrt(1 - sin2 * sin2);
				float3 worldRefl = normalize(sin2 * v2 + cos2 * i.worldNormal);
				half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
				half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
				fixed4 color = 0;
				cos2 = abs(cos2) ;
				color.rgb = cos2 * skyColor + (1 - cos2) * _BaseColor.rgb ;
				return color;
			}
			ENDCG
		}
	}
}
