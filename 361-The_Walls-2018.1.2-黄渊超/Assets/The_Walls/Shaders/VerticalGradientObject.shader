// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "SgLib/VerticalGradientObject" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_StartY("Top Y", Float) = 0
		_EndY("Bottom Y", Float) = -10
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "AlphaTest" "IgnoreProjector" = "True" }
		LOD 200
			Pass{
			ZWrite On
			ColorMask 0
		}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		float  _StartY;
		float  _EndY;

	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Albedo *= _Color.rgb;	// Tint color

		// Alpha blending based on y position
		float colorBlend = clamp((IN.worldPos.y - _EndY) / (_StartY - _EndY), 0, 1);
		o.Alpha = lerp(0, 1, colorBlend);
	}
	ENDCG
	}
	FallBack Off
}
