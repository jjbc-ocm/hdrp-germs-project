Shader "zerinlabs/sh_fx_ADD_cartoonWaterRipples"
{
	Properties
	{
		_colFoam("Foam color", color) = (1.0, 1.0, 1.0, 1.0)

		_TEX("Foam map", 2D) = "white" {}
		_NM("Distorsion map (NormalMap)", 2D) = "white" {}

		_TEX1_speedX("Foam speed X:", Float) = 0.3
		_TEX1_speedY("Foam speed Y:", Float) = 0.3

		_NM_speedX("Distorsion speed X:", Float) = 1
		_NM_speedY("Distorsion speed Y:", Float) = 1

		_Dist("Distorsion amount",Range(-1, 1)) = 0.03

		_amount("Foam amount",float) = 2.0
		_EdgeSmooth("Foam smoothness",Range(0, 0.5)) = 0.05
	}

	SubShader
	{

		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		LOD 200

		Cull Off //two sided
		Lighting Off
		ZWrite Off

		//Blending mode----------------------------------------------
		//Blend SrcAlpha OneMinusSrcAlpha			// Traditional transparency
		//Blend One OneMinusSrcAlpha				// Premultiplied transparency
		Blend One One								// Additive
		//Blend OneMinusDstColor One				// Soft Additive
		//Blend DstColor Zero						// Multiplicative
		//Blend DstColor SrcColor					// 2x Multiplicative

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			//#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow

			#pragma vertex vert 
			#pragma fragment frag

			uniform sampler2D 	_TEX;
			uniform sampler2D 	_NM;

			uniform float _TEX1_speedX;
			uniform float _TEX1_speedY;

			uniform float _NM_speedX;
			uniform float _NM_speedY;

			uniform float _Dist;
			uniform float _amount;
			uniform float _EdgeSmooth;

			uniform fixed4 _colFoam;

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

				float2 mov_lay1 : TEXCOORD0;
				float2 mov_nm : TEXCOORD1;

				float4 color : COLOR;
			};

			float4 _TEX_ST;
			float4 _NM_ST;

			vertexOutput vert(vertexInput v)
			{
				vertexOutput OUT;
				OUT.pos = UnityObjectToClipPos(v.vertex);

				float2 layerUVs = TRANSFORM_TEX(v.texcoord, _TEX);
				float2 normalUVs = TRANSFORM_TEX(v.texcoord, _NM);

				OUT.mov_lay1 = layerUVs + float2(_Time.x * _TEX1_speedX, _Time.x * _TEX1_speedY);
				OUT.mov_nm = normalUVs + float2(cos(_Time.x) * _NM_speedX, sin(_Time.x) * _NM_speedY);

				OUT.color = v.color;

				return OUT;
			}

			half4 frag(vertexOutput IN) : COLOR
			{
				//fetch textures
				fixed4 NM = tex2D(_NM, IN.mov_nm) * 2.0 - 1.0;

				fixed4 layers = tex2D(_TEX, IN.mov_lay1 + (NM.rg * _Dist));

				float alphaComp = IN.color.r * layers.r * _amount;

				float alpha = pow(alphaComp, 2.0);
				alpha = smoothstep(0.5 - _EdgeSmooth, 0.5 + _EdgeSmooth, alpha);

				fixed4 Complete = _colFoam * alpha;

				return Complete;
			}

			ENDCG

		}//end pass

	}//end subshader

	Fallback "Fx/Flare"
}