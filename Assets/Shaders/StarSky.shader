//Render star sky

Shader "Custom/StarSky"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color0 ("Color0", Color) = (1, 1, 1, 1)
		_Color1 ("Color1", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags {"Queue" = "Background" "RenderType"="Opaque" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		Cull off
		ZWrite off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct starPos
			{
				float3 pos;
			};

			struct starPulse {
				float3 pulse;
			};

			struct starColor {
				float radius;
				float color;
			};

			struct input
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			StructuredBuffer<starPos> starPoses;
			StructuredBuffer<starPulse> starPulses;
			StructuredBuffer<starColor> starColors;

			float minRadius;
			float maxRadius;
			float speed;

			float4x4 worldMatrix;

			sampler2D _MainTex;
			float4 _Color0;
			float4 _Color1;

			input vert (uint id : SV_VertexID)
			{
				input o;
				o.vertex = float4(starPoses[id].pos * (minRadius + (maxRadius - minRadius) * starColors[id].radius) + starPulses[id].pulse * _Time.x * speed, 1);
				o.vertex = mul(worldMatrix, o.vertex);
				o.uv = float2(0, 0);

				float3 c = _Color0.rgb + (_Color1.rgb - _Color0.rgb) * starColors[id].color;

				o.color = float4(c, 1);
				return o;
			}


			float4 RotPoint(float4 pos, float3 offset, float3 side, float3 up) {
				float3 finalPos = pos.xyz;
				finalPos += offset.x * side;
				finalPos += offset.y * up;
				return float4(finalPos, 1);
			}

			[maxvertexcount(4)]
			void geom(point input p[1], inout TriangleStream<input> triStream) {
				float halfSize = 0.5;
				float4 pos = p[0].vertex;
				float4 v[4];

				float3 up = float3(0, 1, 0);
				float3 look = normalize(_WorldSpaceCameraPos - pos);
				float3 right = normalize(cross(look, up));
				up = normalize(cross(right, look));

				v[0] = RotPoint(pos, float3(-halfSize, -halfSize, 0), right, up);
				v[1] = RotPoint(pos, float3(-halfSize, halfSize, 0), right, up);
				v[2] = RotPoint(pos, float3(halfSize, -halfSize, 0), right, up);
				v[3] = RotPoint(pos, float3(halfSize, halfSize, 0), right, up);

				input pOut;
				pOut.color = p[0].color;

				pOut.vertex = mul(UNITY_MATRIX_VP, v[0]);
				pOut.uv = float2(0, 0);
				triStream.Append(pOut);

				pOut.vertex = mul(UNITY_MATRIX_VP, v[1]);
				pOut.uv = float2(0, 1.0);
				triStream.Append(pOut);

				pOut.vertex = mul(UNITY_MATRIX_VP, v[2]);
				pOut.uv = float2(1.0, 0);
				triStream.Append(pOut);

				pOut.vertex = mul(UNITY_MATRIX_VP, v[3]);
				pOut.uv = float2(1.0, 1.0);
				triStream.Append(pOut);

			}
			
			fixed4 frag (input i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				//col = float4(1, 1, 0, 1);
				return col;
			}
			ENDCG
		}
	}

	//FallBack "Diffuse"
}
