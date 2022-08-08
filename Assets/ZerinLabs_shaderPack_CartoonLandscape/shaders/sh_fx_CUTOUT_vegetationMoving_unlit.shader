Shader "zerinlabs/sh_fx_CUTOUT_vegetationMoving_unlit"
{
	Properties
	{
		[Header(TEXTURE AND COLOUR PARAMETERS)]
		_Color("Main Color", Color) = (1,1,1,1)
		_Color2("Secondary Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_DF2("Diffuse (world coords)", 2D) = "white" {}
		_DiffuseScale("Diffuse scale", Float) = 0.5
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

		[Header(TEXTURE BLENDING PARAMETERS)]
		_influence("WorldCoords diffuse influence", Range(0, 1)) = 1.0

		[Header(ANIMATION PARAMETERS)]
		_movementAmount("Amplitude", Float) = 0.5
		_movementSpeed("Speed", Float) = 0.5
		_movementPeriod("Period", Float) = 1
	}

		SubShader
		{

			Tags
			{
				"Queue" = "AlphaTest"
				"RenderType" = "TransparentCutout"
				"IgnoreProjector" = "True"
			}

			LOD 200

			//Cull Off //two sided
			Lighting Off
			//ZWrite Off

			//Blending mode----------------------------------------------
			Blend SrcAlpha OneMinusSrcAlpha			// Traditional transparency
			//Blend One OneMinusSrcAlpha				// Premultiplied transparency
			//Blend One One								// Additive
			//Blend OneMinusDstColor One				// Soft Additive
			//Blend DstColor Zero						// Multiplicative
			//Blend DstColor SrcColor					// 2x Multiplicative

			Pass
			{
				CGPROGRAM

				#include "UnityCG.cginc"

				//#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
				
				//#pragma alphatest:_Cutoff
				#pragma vertex vert 
				#pragma fragment frag

				sampler2D _MainTex;
				sampler2D _DF2;
				float _DiffuseScale;
				fixed4 _Color;
				fixed4 _Color2;
				float _movementAmount;
				float _movementSpeed;
				float _movementPeriod;
				float _Metallic;
				float _Smoothness;
				float _influence;
				float _Cutoff;

				struct vertexInput
				{
					float4 vertex : POSITION;
					//float4 tangent : TANGENT;
					//float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
					//half4 texcoord2 : TEXCOORD2;
					//half4 texcoord3 : TEXCOORD3;
					//half4 texcoord4 : TEXCOORD4;
					//half4 texcoord5 : TEXCOORD5;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float2 _MainTex_uv : TEXCOORD0;
					float2 _DF2_uv : TEXCOORD1;

					float4 vertexCol : COLOR;
					float3 worldP : TEXCOORD2; //world mapping coords	
				};

				float4 _MainTex_ST;
				float4 _DF2_ST;

				vertexOutput vert(vertexInput v)
				{
					vertexOutput OUT;
					
					float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					OUT.worldP = wPos;

					float movAmount = _movementAmount * v.color.r;

					float3 vColTotal = v.color * 2.0 - 1.0;

					float stepPosX = movAmount * sin((_Time + wPos.x * (vColTotal.g + vColTotal.b)  * _movementPeriod) * _movementSpeed);
					float stepPosY = movAmount * cos((_Time + wPos.y * vColTotal.g * _movementPeriod) * _movementSpeed);
					float stepPosZ = movAmount * sin((_Time + wPos.z * vColTotal.b * _movementPeriod) * _movementSpeed);

					v.vertex.xyz += float3(stepPosX, stepPosY, stepPosZ);

					OUT.pos = UnityObjectToClipPos(v.vertex);

					OUT._MainTex_uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					OUT._DF2_uv = TRANSFORM_TEX(v.texcoord, _DF2);					

					OUT.vertexCol = v.color;

					return OUT;
				}

				half4 frag(vertexOutput IN) : COLOR
				{
					float2 uvs = IN.worldP.zx * _DiffuseScale;
					
					fixed4 col = lerp(_Color2, _Color, IN.vertexCol.r);
					fixed4 Complete = tex2D(_MainTex, IN._MainTex_uv) * col;
					float alpha = Complete.a;

					fixed4 c_world = tex2D(_DF2, uvs) * col;
					Complete = lerp(Complete, c_world, _influence);

					clip(Complete.a - _Cutoff);
					
					return Complete;
				}

				ENDCG

			}//end pass

		}//end subshader

		Fallback "Fx/Flare"
}