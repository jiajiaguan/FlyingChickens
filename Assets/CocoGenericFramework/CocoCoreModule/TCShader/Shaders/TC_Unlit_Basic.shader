Shader "TC/Unlit/Basic" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		// toonmap begin ----------------------
		[Toggle (_TOONMAP_ON)]
		_Toonmap ("Toonmap", Float) = 0
		_ToonCube ("Toon Cubemap(RGB)", CUBE) = "" {}
		// toonmap end ------------------------

		// decal layer begin ------------------
		[KeywordEnum (Off, Layer_1, Layer_2, Layer_3)]
		_Decal ("Decal Layer", Float) = 0
		_DecalTex1 ("Decal Texture 1", 2D) = "black" {}
		[Toggle (_DECAL_LAYER_1_UV2_ON)]
		_DecalTex1UV2 ("Decal Texture 1 UV2", Float) = 0
		_DecalTex2 ("Decal Texture 2", 2D) = "black" {}
		[Toggle (_DECAL_LAYER_2_UV2_ON)]
		_DecalTex2UV2 ("Decal Texture 2 UV2", Float) = 0
		_DecalTex3 ("Decal Texture 3", 2D) = "black" {}
		[Toggle (_DECAL_LAYER_3_UV2_ON)]
		_DecalTex3UV2 ("Decal Texture 3 UV2", Float) = 0
		// decal layer end --------------------

		// discolor begin ---------------------
		[KeywordEnum (Off, Hsl_Blend, Hue_Replace)]
		_Discolor ("Discolor Mode", Float) = 0
		// hsl blend
		_Hue ("Hue", Range (-0.5, 0.5)) = 0
		_Saturation ("Saturation", Range (-1, 1)) = 0
		_Lightness ("Lightness", Range (-1, 1)) = 0
		_MixingFactor ("Mixing Factor", Range (0, 4)) = 1
		// replace hue
		_HueColor ("Color (Hue Only)", Color) = (1, 1, 1, 0)
		_SaturMin ("Saturation Min", Range (0, 1.5)) = 0.1
		_SaturRatio ("Saturation Ratio", Range (-2, 2)) = 2
		_SaturAdd ("Saturation Add", Range (-2, 2)) = 0.05
		_LightMax ("Light Max", Range (0, 2)) = 0.9
		_LightRatio ("Ligh Ratio", Range (-2, 2)) = 2
		_LightAdd ("Light Add", Range (-3, 3)) = 0.5
		// discolor end -----------------------

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass {
			Name "BASE"

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#pragma shader_feature __ _ALPHATEST_ON _ALPHABLEND_ON

			#pragma shader_feature _TOONMAP_ON
			#pragma shader_feature __ _DISCOLOR_HSL_BLEND _DISCOLOR_HUE_REPLACE

			#pragma shader_feature __ _DECAL_LAYER_1 _DECAL_LAYER_2 _DECAL_LAYER_3
			#pragma shader_feature _DECAL_LAYER_1_UV2_ON
			#pragma shader_feature _DECAL_LAYER_2_UV2_ON
			#pragma shader_feature _DECAL_LAYER_3_UV2_ON

			#pragma vertex vert
			#pragma fragment frag

			#include "../CGUbers/TCCG_VFUber_Unlit.cginc"

			ENDCG
		}
	}

	CustomEditor "TC.Shader.TCUnlitShaderGUI"
}
