Shader "zerinlabs/sh_fx_CUTOUT_cartoonWaterRipples"
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
		
		_Dist("Distorsion amount",Range (-1, 1)) = 0.03

		_amount("Foam amount",float) = 2.0
		_EdgeSmooth("Foam smoothness",Range(0, 0.5)) = 0.05

		_Cutoff("Alpha cutoff",Range(0,1)) = 0.5
	}

	SubShader
	{

		Tags
		{
			"Queue" = "Transparent"//"AlphaTest"
			"RenderType" = "TransparentCutout"
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

		CGPROGRAM

		#include "UnityCG.cginc"

		#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow

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

		struct Input
		{
			float2 mov_lay1;
			float2 mov_nm;

			float4 color : COLOR;
		};

		/*
		struct appdata_full 
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			fixed4 color : COLOR;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			half4 texcoord2 : TEXCOORD2;
			half4 texcoord3 : TEXCOORD3;
			half4 texcoord4 : TEXCOORD4;
			half4 texcoord5 : TEXCOORD5;
		};
		*/

		float4 _TEX_ST;
		float4 _NM_ST;

		void vert(inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float2 layerUVs = TRANSFORM_TEX(v.texcoord, _TEX);
			float2 normalUVs = TRANSFORM_TEX(v.texcoord, _NM);

			o.mov_lay1 = layerUVs + float2(_Time.x * _TEX1_speedX, _Time.x * _TEX1_speedY);
			o.mov_nm = normalUVs + float2(cos(_Time.x) * _NM_speedX, sin(_Time.x) * _NM_speedY);

			o.color = v.color;

			//UNITY_TRANSFER_FOG(o, o.pos);
		}

		/*
		https://docs.unity3d.com/Manual/SL-SurfaceShaders.html

		struct SurfaceOutputStandard
		{
		fixed3 Albedo;      // base (diffuse or specular) color
		fixed3 Normal;      // tangent space normal, if written
		half3 Emission;
		half Metallic;      // 0=non-metal, 1=metal
		half Smoothness;    // 0=rough, 1=smooth
		half Occlusion;     // occlusion (default 1)
		fixed Alpha;        // alpha for transparencies
		};
		*/

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			//fetch textures
			fixed4 NM = tex2D(_NM, IN.mov_nm) * 2.0 - 1.0;

			fixed4 layers = tex2D(_TEX, IN.mov_lay1 + (NM.rg * _Dist));

			float alphaComp = IN.color.r * layers.r * _amount;

			float alpha = pow(alphaComp, 2.0);
			alpha = smoothstep(0.5- _EdgeSmooth, 0.5+ _EdgeSmooth, alpha);
			alpha = saturate(alpha);
			//float lay1 = layers.r;

			//float mask = pow((IN.color.r + layers.b) * IN.color.r, 2.0);
			//float edge = 1.0 - step(_Edge, mask);
			//float edge = 1.0 - smoothstep(_Edge- _EdgeSmooth , _Edge + _EdgeSmooth, mask);

			fixed4 finalCOL = _colFoam;

			o.Albedo = finalCOL.rgb;

			o.Alpha = alpha;
		}

		ENDCG
		}

		Fallback "Diffuse"
}