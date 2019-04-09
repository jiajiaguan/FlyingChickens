#ifndef TCCG_VF_UNIT_DECAL_LAYER
#define TCCG_VF_UNIT_DECAL_LAYER

#if defined(_DECAL_LAYER_1) || defined(_DECAL_LAYER_2) || defined(_DECAL_LAYER_3)

	#include "../CGUtils/TCCG_Util_LayerBlend.cginc"

	// uv ---------------------------------------------
	#if defined(_DECAL_LAYER_1_UV2_ON) || defined(_DECAL_LAYER_2_UV2_ON) || defined(_DECAL_LAYER_3_UV2_ON)
		#ifndef TC_VF_APPDATA_DECAL_LAYER
			#define TC_VF_APPDATA_DECAL_LAYER float2 uv1 : TEXCOORD1;
		#endif
	#endif

	#ifndef _DECAL_LAYER_1_UV2_ON
		#define UV_DECAL1 uv0
	#else
		#define UV_DECAL1 uv1
	#endif
	#ifndef _DECAL_LAYER_2_UV2_ON
		#define UV_DECAL2 uv0
	#else
		#define UV_DECAL2 uv1
	#endif
	#ifndef _DECAL_LAYER_3_UV2_ON
		#define UV_DECAL3 uv0
	#else
		#define UV_DECAL3 uv1
	#endif

	#ifdef _DECAL_LAYER_1

		// datas --------------------------------------
		#ifndef TC_VF_V2FDATA_DECAL_LAYER
			#define TC_VF_V2FDATA_DECAL_LAYER float2 texcoord1 : TEXCOORD2;
		#endif

		// properties ---------------------------------
		sampler2D _DecalTex1;
		float4 _DecalTex1_ST;

		// functions ----------------------------------
		#ifndef TC_VF_VERT_DECAL_LAYER
			#define TC_VF_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.texcoord1 = TRANSFORM_TEX(v.UV_DECAL1, _DecalTex1);
		#endif

		#ifndef TC_VF_FRAG_DECAL_LAYER
			#define TC_VF_FRAG_DECAL_LAYER(_V2FDATA, _mainTex) \
				fixed4 decal1 = tex2D(_DecalTex1, _V2FDATA.texcoord1); \
				_mainTex.rgb = blend_layer1 (_mainTex.rgb, decal1.rgb, decal1.a);
		#endif

	#elif _DECAL_LAYER_2

		// datas --------------------------------------
		#ifndef TC_VF_V2FDATA_DECAL_LAYER
			#define TC_VF_V2FDATA_DECAL_LAYER float4 texcoord1 : TEXCOORD2;
		#endif

		// properties ---------------------------------
		sampler2D _DecalTex1;
		float4 _DecalTex1_ST;
		sampler2D _DecalTex2;
		float4 _DecalTex2_ST;

		// functions ----------------------------------
		#ifndef TC_VF_VERT_DECAL_LAYER
			#define TC_VF_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.texcoord1.xy = TRANSFORM_TEX(v.UV_DECAL1, _DecalTex1); \
				_V2FDATA.texcoord1.zw = TRANSFORM_TEX(v.UV_DECAL2, _DecalTex2);
		#endif

		#ifndef TC_VF_FRAG_DECAL_LAYER
			#define TC_VF_FRAG_DECAL_LAYER(_V2FDATA, _mainTex) \
				fixed4 decal1 = tex2D(_DecalTex1, _V2FDATA.texcoord1.xy); \
				fixed4 decal2 = tex2D(_DecalTex2, _V2FDATA.texcoord1.zw); \
				_mainTex.rgb = blend_layer2 (_mainTex.rgb, decal1.rgb, decal1.a, decal2.rgb, decal2.a);
		#endif

	#else   // defined _DECAL_LAYER_3

		// datas --------------------------------------
		#ifndef TC_VF_V2FDATA_DECAL_LAYER
			#define TC_VF_V2FDATA_DECAL_LAYER float4 texcoord1 : TEXCOORD2; float2 texcoord2 : TEXCOORD3;
		#endif

		// properties ---------------------------------
		sampler2D _DecalTex1;
		float4 _DecalTex1_ST;
		sampler2D _DecalTex2;
		float4 _DecalTex2_ST;
		sampler2D _DecalTex3;
		float4 _DecalTex3_ST;

		// functions ----------------------------------
		#ifndef TC_VF_VERT_DECAL_LAYER
			#define TC_VF_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.texcoord1.xy = TRANSFORM_TEX(v.UV_DECAL1, _DecalTex1); \
				_V2FDATA.texcoord1.zw = TRANSFORM_TEX(v.UV_DECAL2, _DecalTex2); \
				_V2FDATA.texcoord2 = TRANSFORM_TEX(v.UV_DECAL3, _DecalTex3);
		#endif

		#ifndef TC_VF_FRAG_DECAL_LAYER
			#define TC_VF_FRAG_DECAL_LAYER(_V2FDATA, _mainTex) \
				fixed4 decal1 = tex2D(_DecalTex1, _V2FDATA.texcoord1.xy); \
				fixed4 decal2 = tex2D(_DecalTex2, _V2FDATA.texcoord1.zw); \
				fixed4 decal3 = tex2D(_DecalTex3, _V2FDATA.texcoord2); \
				_mainTex.rgb = blend_layer3 (_mainTex.rgb, decal1.rgb, decal1.a, decal2.rgb, decal2.a, decal3.rgb, decal3.a);
		#endif

	#endif  // #ifdef _DECAL_LAYER_1

#endif  // #if defined(_DECAL_LAYER_1) || defined(_DECAL_LAYER_2) || defined(_DECAL_LAYER_3)

#endif  // #ifndef TCCG_VF_UNIT_DECAL_LAYER
