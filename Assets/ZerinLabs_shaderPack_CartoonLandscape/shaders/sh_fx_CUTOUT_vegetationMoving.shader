Shader "zerinlabs/sh_fx_CUTOUT_vegetationMoving"
{
	
	Properties
	{
		[Header(LIGHTING PARAMETERS)]
		_Metallic("Metallic", Range(0,1)) = 0.1
		_Smoothness("Smoothness", Range(0,1)) = 0.1
		
		[Header(TEXTURE AND COLOUR PARAMETERS)]
		_Color("Main Color", Color) = (1,1,1,1)
		_Color2("Secondary Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_DF2("Diffuse (world coords)", 2D) = "white" {}
		_DiffuseScale("Diffuse scale", Float) = 0.5
		
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
		}

		LOD 200

		CGPROGRAM

		#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow

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

		struct Input 
		{
			float2 uv_MainTex;
			float3 vertexCol;

			float3 worldPos; //world mapping coords	
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;

			float movAmount = _movementAmount * v.color.r ;
			
			float3 vColTotal = v.color * 2.0 - 1.0;

			float stepPosX = movAmount * sin((_Time + wPos.x * (vColTotal.g + vColTotal.b)  * _movementPeriod) * _movementSpeed);
			float stepPosY = movAmount * cos((_Time + wPos.y * vColTotal.g * _movementPeriod) * _movementSpeed);
			float stepPosZ = movAmount * sin((_Time + wPos.z * vColTotal.b * _movementPeriod) * _movementSpeed);

			v.vertex.xyz += float3(stepPosX, stepPosY, stepPosZ);

			o.vertexCol = v.color;
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
			float2 uvs = IN.worldPos.zx * _DiffuseScale;

			fixed4 col = lerp(_Color2, _Color, IN.vertexCol.r);
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * col;
			float alpha = c.a;

			fixed4 c_world = tex2D(_DF2, uvs) * col;
			c = lerp(c, c_world, _influence);

			o.Albedo = c.rgb;
			o.Alpha = alpha;

			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		 
		ENDCG
	}
	
	Fallback "Diffuse"
}