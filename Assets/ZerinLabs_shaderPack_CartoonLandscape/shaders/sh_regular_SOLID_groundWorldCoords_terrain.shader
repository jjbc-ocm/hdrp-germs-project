Shader "zerinlabs/sh_regular_SOLID_groundWorldCoords_terrain"
{
	Properties
	{
		[Header(LIGHTING PARAMETERS)]
		_MetallicFactor1("Metallic Factor",Range(0, 1)) = 0
		_GlossMapScale1("Smoothness", Range(0, 1)) = 0

		[Header(GROUND PARAMETERS)]
		_DF1("DF (DiffuseMap)", 2D) = "white" {}
		_NM1("NM (NormalMap)", 2D) = "bump" {}
		_BumpScale1("NM intensity", range(0,3)) = 1.0
		_DiffuseScale("WorldMap scale (DF/NM)", float) = 0.5

		[Space(20)]
		_DM("DM (DetailMap)", 2D) = "white" {}
		_DMScale("DM intensity", range(0, 1)) = 1.0
		_DetailScale("WorldMapScale (DM)", float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry" 
		}

		LOD 200

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

		float _MetallicFactor1;
		float _GlossMapScale1;

		sampler2D _DF1;
		sampler2D _NM1;
		float _BumpScale1;
		float _DiffuseScale;

		sampler2D _DM;
		float _DetailScale;
		float _DMScale;

		struct Input
		{
			fixed4 color : COLOR;

			float3 worldPos; //world mapping coords		
		};

		/*
		struct appdata_full
		{
			//float4 vertex : POSITION;
			//float4 tangent : TANGENT;
			//float3 normal : NORMAL;
			fixed4 color : COLOR;
			//float4 texcoord : TEXCOORD0;
			//float4 texcoord1 : TEXCOORD1;
			//half4 texcoord2 : TEXCOORD2;
			//half4 texcoord3 : TEXCOORD3;
			//half4 texcoord4 : TEXCOORD4;
			//half4 texcoord5 : TEXCOORD5;
		};
		*/

		void vert(inout appdata_full v, out Input o)
		{

			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

		/*
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
			// DM (detailmap)
			float2 uvsDM = IN.worldPos.xz * _DetailScale;
			fixed4 DM = tex2D(_DM, uvsDM) * 2.0 - 1.0;
			DM = DM * _DMScale;

			// DF & NM 1
			float2 uvsDF = IN.worldPos.xz * _DiffuseScale;
			fixed4 DF_1 = tex2D(_DF1, uvsDF);
			float3 NM_1 = lerp(fixed3(0.0, 0.0, 1.0), UnpackNormal(tex2D(_NM1, uvsDF)), _BumpScale1).rgb;

			o.Albedo = DF_1 + DM;
			o.Alpha = DF_1.a;

			o.Normal = NM_1;

			o.Metallic = _MetallicFactor1;
			o.Smoothness = _GlossMapScale1;
		}

	ENDCG

	}

	FallBack "Diffuse"
}