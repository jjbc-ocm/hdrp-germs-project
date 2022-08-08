Shader "zerinlabs/sh_fx_SOLID_cartoonWater"
{

	Properties
	{
		_TEX1("Water map", 2D) = "white" {}
		_TEX1_speedX("Water speed X:", Float) = 0.3
		_TEX1_speedY("Water speed Y:", Float) = 0.3
		
		_TEX2("Edge noise map", 2D) = "white" {}
		_EdgeColor("Edge color", color) = (1.0, 1.0, 1.0, 1.0)
		_EdgeThick("Edge thickness",Range(0, 1)) = 0.008
		_EdgeSmooth("Edge smoothness",Range(0, 0.5)) = 0.002

		_NM("Distorsion map (NormalMap)", 2D) = "white" {}
		_NM_speedX("Distorsion speed X:", Float) = 1
		_NM_speedY("Distorsion speed Y:", Float) = 1
		
		_Dist("Distorsion amount",Range (-1, 1)) = 0.03

		_Emission("Emissive color", color) = (0.0, 0.0, 0.0, 1.0)
	}

	SubShader
	{

		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		LOD 200

		Lighting Off

		CGPROGRAM

		#include "UnityCG.cginc"

		#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow

		uniform sampler2D 	_TEX1;
		uniform sampler2D 	_TEX2;
		uniform sampler2D 	_NM;
		
		uniform float _TEX1_speedX;
		uniform float _TEX1_speedY;
		
		uniform float _NM_speedX;
		uniform float _NM_speedY;

		uniform float _Dist;
		uniform float _EdgeThick;
		uniform float _EdgeSmooth;
		uniform fixed4 _EdgeColor;

		uniform fixed4 _Emission;

		struct Input
		{
			float2 mov_tex;
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

		float4 _TEX1_ST;
		float4 _TEX2_ST;
		float4 _NM_ST;
		
		void vert(inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float2 layerUVs = TRANSFORM_TEX(v.texcoord, _TEX1);
			float2 normalUVs = TRANSFORM_TEX(v.texcoord, _NM);

			o.mov_tex = layerUVs + float2(sin(_Time.x) * _TEX1_speedX, cos(_Time.x) * _TEX1_speedY);
			o.mov_nm = normalUVs + float2(cos(_Time.x) * _NM_speedX, sin(_Time.x) * _NM_speedY);

			o.color = v.color;
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

			fixed4 waterTex = tex2D(_TEX1, IN.mov_tex + (NM.rg * _Dist));

			fixed4 edgeTex = tex2D(_TEX2, IN.mov_tex + (NM.rg * _Dist));

			float edge = pow((IN.color.r + edgeTex.r) * IN.color.r, 2.0);
			edge = 1.0 - smoothstep(_EdgeThick - _EdgeSmooth , _EdgeThick + _EdgeSmooth, edge);

			fixed4 finalCOL = lerp(waterTex, _EdgeColor, edge);

			o.Albedo = finalCOL.rgb;

			o.Emission = _Emission.rgb;
		}

		ENDCG
		}

		Fallback "Diffuse"
}