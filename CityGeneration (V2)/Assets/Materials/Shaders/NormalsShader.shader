Shader "Custom/TextureDependingNormal"
{
	Properties
	{
		_RoofTex("Roof Texture", 2D) = "white" {}
		_WallTex("Wall Texture", 2D)   = "white" {}
		_SlopeTex("Slope Texture", 2D) = "white" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }


		CGPROGRAM
#pragma surface surf Standard

		struct Input {
		float3 worldNormal;
		float2 uv_RoofTex;
		float2 uv_WallTex;
		float2 uv_SlopeTex;
	};

	sampler2D _RoofTex;
	sampler2D _WallTex;
	sampler2D _SlopeTex;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Floor
		if (IN.worldNormal.y > 0.9)
		{
			o.Albedo = tex2D(_RoofTex, IN.uv_RoofTex).rgb;
		}

		// Slant
		else if (IN.worldNormal.y > 0.1)
		{
			o.Albedo = tex2D(_SlopeTex, IN.uv_SlopeTex).rgb;
		}

		// Wall
		else
		{
			o.Albedo = tex2D(_WallTex, IN.uv_WallTex).rgb;
		}
		//o.Emission = half3(1, 1, 1) * o.Albedo;
		o.Metallic = 0.0;
		o.Smoothness = 0.5;
	}

	ENDCG
	}
}