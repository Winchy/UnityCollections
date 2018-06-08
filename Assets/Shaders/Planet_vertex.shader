// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Planet Vertex" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpTex ("Normal Map", 2D) = "bump" {}
		_HeightTex("Height Map", 2D) = "white" {}
		_BumpIntensity ("Bump Intensity", Float) = 1.0
		_HeightScale("Height Scale", Float) = 1.0
		_WaterHeight("Water Height", Float) = 0.5
		_MinWaterHeight("Min Water Height", Float) = 0
		_WaterColor("Water Color", Color) = (0.66, 0.79, 1.0, 1.0)
		_ShoreColor("Shore Color", Color) = (1, 1, 1, 1)
		_AtmosphereColor("Atmosphere Color", Color) = (1, 1, 1, 0.3)
		_AtmosphereThickness("Atmosphere Thickness", Float) = 10
		_Atmosphere("Atmosphere", Float) = 0.5
	}
	SubShader {
		
		Pass {
			Name "Diffuse"
			Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
			LOD 200
			
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _BumpTex;
			sampler2D _HeightTex;
			fixed _BumpIntensity;
			fixed _WaterHeight;
			fixed4 _WaterColor;
			fixed _HeightScale;
			fixed _MinWaterHeight;
			fixed4 _ShoreColor;

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord: TEXCOORD0;
				float4 tangent : TANGENT;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				//float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				//fixed4 diff : COLOR0;
				half3 tspace0 : TEXCOORD1;
				half3 tspace1 : TEXCOORD2;
				half3 tspace2 : TEXCOORD3;
			};

			v2f vert ( appdata v) {
				v2f output;
				output.pos = UnityObjectToClipPos(v.vertex);
				output.uv = v.texcoord;

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				output.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				output.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				output.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				/*
				fixed3 n = (tex2D(_BumpTex, texcoord) * _BumpIntensity).rgb;

				half3 worldNormal = UnityObjectToWorldNormal(n);
				output.normal = worldNormal;

				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				output.diff = nl * _LightColor0;
				*/
				return output;
			}


			fixed4 frag(v2f i) : COLOR {
				fixed3 n = (tex2D(_BumpTex, i.uv) * _BumpIntensity).rgb;
				half3 worldNormal;
				worldNormal.x = dot(i.tspace0, n);
				worldNormal.y = dot(i.tspace1, n);
				worldNormal.z = dot(i.tspace2, n);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				fixed4 diff = nl * _LightColor0;
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed height = saturate(n.g * _HeightScale);
				fixed waterThresh = height - _WaterHeight;
				fixed waterMask = saturate(_MinWaterHeight * _HeightScale);
				fixed shade = saturate(height/max(_WaterHeight, 0.0001));
				//waterThresh = 0.01;
				fixed shoreline = 1.0 - saturate(abs(waterThresh)*(20));

				fixed4 heightColor = tex2D(_HeightTex, fixed2(height, 0.5)); 
				fixed4 waterColor = _WaterColor * shade;
				fixed4 landColor = c * heightColor;

				//n.z = 1;
				fixed4 finalColor = lerp(waterColor, landColor, shade);
				finalColor = lerp(finalColor, _ShoreColor, shoreline);
				return finalColor * diff;
			}

			ENDCG
		}

		Pass {
			Name "Atmosphere"
			Tags { "RenderType"="Transparent"  "LightMode"="ForwardBase"}
			LOD 200

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _BumpTex;
			sampler2D _HeightTex;
			fixed _BumpIntensity;
			fixed _WaterHeight;
			fixed4 _WaterColor;
			fixed _HeightScale;
			fixed _MinWaterHeight;
			fixed4 _ShoreColor;

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 normal : NORMAL;
			};

			float4 _AtmosphereColor;
			float _AtmosphereThickness;
			float _Atmosphere;

			v2f vert ( appdata v) {
				v2f output;

				float4 pos = v.vertex;
				pos.xyz += v.normal * _AtmosphereThickness;
				output.pos = UnityObjectToClipPos(pos);
				output.worldPos = UnityObjectToClipPos(pos).xyz;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				output.normal = worldNormal;


				return output;
			}

			fixed4 frag(v2f i) : COLOR {
				fixed4 c = _AtmosphereColor;
				i.normal = normalize(i.normal);
				float3 worldViewDir = UnityWorldSpaceViewDir(i.worldPos);
				float border = 1 - abs(dot(worldViewDir, i.normal));
				float alpha = (border * (1 - _Atmosphere) + _Atmosphere);
				c.a *= alpha;
				return c;
			}

			ENDCG
		}

	}



	FallBack "Diffuse"
}
