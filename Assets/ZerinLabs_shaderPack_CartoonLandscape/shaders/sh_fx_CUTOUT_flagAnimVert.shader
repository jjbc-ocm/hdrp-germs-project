Shader "zerinlabs/sh_fx_CUTOUT_flagAnimVert"
{
	
	Properties
	{
		[Header(LIGHTING PARAMETERS)]
		_Metallic("Metallic", Range(0,1)) = 0.1
		_Smoothness("Smoothness", Range(0,1)) = 0.1

		[Header(TEXTURE AND COLOUR PARAMETERS)]
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff",Range(0,1)) = 0.5

		[Header(ANIMATION PARAMETERS)]
		_movDir("Movement direction", Vector) = (1,1,1,1)
		_movementAmount("Vertex amplitude XYZ", Vector) = (0.4, 0.2, 0.1, 1)
		_movementSpeed("Vertex speed", Float) = 150
		_movPeriod("Vertex period", Float) = 20

	}

	SubShader
	{

		Tags
		{ 
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}

		LOD 200

		CGPROGRAM

		#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow

		sampler2D _MainTex;
		fixed4 _Color;
		float4 _movementAmount;
		float _movementSpeed;
		float _Metallic;
		float _Smoothness;
		float4 _movDir;
		float _movPeriod;

		struct Input 
		{
			float2 uv_MainTex;
			float3 vertexCol;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			float stepPosX = sin(_Time * _movementSpeed + (v.color.r * _movPeriod)) * v.color.r * _movementAmount.x;
			float stepPosY = cos(_Time * _movementSpeed + (v.color.g * _movPeriod)) * v.color.r * _movementAmount.y;
			float stepPosZ = sin(_Time * _movementSpeed + (v.color.b * _movPeriod)) * v.color.r * _movementAmount.z;

			float3 ampl = _movDir.xyz * (stepPosX + stepPosY + stepPosZ);
			v.vertex.xyz += ampl;
			v.normal.xyz = normalize(ampl * 2.0 - 1.0);
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
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		 
		ENDCG
	}
	
	FallBack "Diffuse"
}