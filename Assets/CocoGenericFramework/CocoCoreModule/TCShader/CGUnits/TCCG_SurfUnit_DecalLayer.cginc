#ifndef TCCG_SURF_UNIT_DECAL_LAYER
#define TCCG_SURF_UNIT_DECAL_LAYER

#if defined(_DECAL_LAYER_1) || defined(_DECAL_LAYER_2) || defined(_DECAL_LAYER_3)

	#include "../CGUtils/TCCG_Util_LayerBlend.cginc"

	// uv ---------------------------------------------
	#ifndef _DECAL_LAYER_1_UV2_ON
		#define UV_DECAL1 uv_DecalTex1
	#else
		#define UV_DECAL1 uv2_DecalTex1
	#endif
	#ifndef _DECAL_LAYER_2_UV2_ON
		#define UV_DECAL2 uv_DecalTex2
	#else
		#define UV_DECAL2 uv2_DecalTex2
	#endif
	#ifndef _DECAL_LAYER_3_UV2_ON
		#define UV_DECAL3 uv_DecalTex3
	#else
		#define UV_DECAL3 uv2_DecalTex3
	#endif

	// datas ------------------------------------------
	#ifndef TC_SURF_INPUTDATA_DECAL_LAYER
		#ifdef _DECAL_LAYER_1
			#define TC_SURF_INPUTDATA_DECAL_LAYER half2 UV_DECAL1;
		#elif defined(_DECAL_LAYER_2)
			#define TC_SURF_INPUTDATA_DECAL_LAYER half2 UV_DECAL1; half2 UV_DECAL2;
		#else   // defined(_DECAL_LAYER_3)
			#define TC_SURF_INPUTDATA_DECAL_LAYER half2 UV_DECAL1; half2 UV_DECAL2; half2 UV_DECAL3;
		#endif
	#endif


	// properties -------------------------------------
	#ifdef _DECAL_LAYER_1
		sampler2D _DecalTex1;
	#elif defined(_DECAL_LAYER_2)
		sampler2D _DecalTex1;
		sampler2D _DecalTex2;
	#else   // defined(_DECAL_LAYER_3)
		sampler2D _DecalTex1;
		sampler2D _DecalTex2;
		sampler2D _DecalTex3;
	#endif

	// functions -------------------------------------
	#ifndef TC_SURF_DECAL_LAYER
		#ifdef _DECAL_LAYER_1
			#define TC_SURF_DECAL_LAYER(_IN, _mainTex) \
				fixed4 decal1 = tex2D (_DecalTex1, _IN.UV_DECAL1); \
				_mainTex.rgb = blend_layer1 (_mainTex.rgb, decal1.rgb, decal1.a);
		#elif defined(_DECAL_LAYER_2)
			#define TC_SURF_DECAL_LAYER(_IN, _mainTex) \
				fixed4 decal1 = tex2D (_DecalTex1, _IN.UV_DECAL1); \
				fixed4 decal2 = tex2D (_DecalTex2, _IN.UV_DECAL2); \
				_mainTex.rgb = blend_layer2 (_mainTex.rgb, decal1.rgb, decal1.a, decal2.rgb, decal2.a);
		#else   // defined(_DECAL_LAYER_3)
			#define TC_SURF_DECAL_LAYER(_IN, _mainTex) \
				fixed4 decal1 = tex2D (_DecalTex1, _IN.UV_DECAL1); \
				fixed4 decal2 = tex2D (_DecalTex2, _IN.UV_DECAL2); \
				fixed4 decal3 = tex2D (_DecalTex3, _IN.UV_DECAL3); \
				_mainTex.rgb = blend_layer3 (_mainTex.rgb, decal1.rgb, decal1.a, decal2.rgb, decal2.a, decal3.rgb, decal3.a);
		#endif
	#endif



	// v/f pass =======================================

	// uv ---------------------------------------------
	#ifndef _DECAL_LAYER_1_UV2_ON
		#define TEXCOORD_DECAL1 texcoord
	#else
		#define TEXCOORD_DECAL1 texcoord1
	#endif
	#ifndef _DECAL_LAYER_2_UV2_ON
		#define TEXCOORD_DECAL2 texcoord
	#else
		#define TEXCOORD_DECAL2 texcoord1
	#endif
	#ifndef _DECAL_LAYER_3_UV2_ON
		#define TEXCOORD_DECAL3 texcoord
	#else
		#define TEXCOORD_DECAL3 texcoord1
	#endif

	// variable ---------------------------------------
	#ifndef TC_SURF_PASS_VAR_DECAL_LAYER
		#ifdef _DECAL_LAYER_1
			#define TC_SURF_PASS_VAR_DECAL_LAYER float4 _DecalTex1_ST;
		#elif defined(_DECAL_LAYER_2)
			#define TC_SURF_PASS_VAR_DECAL_LAYER float4 _DecalTex1_ST; float4 _DecalTex2_ST;
		#else   // defined(_DECAL_LAYER_3)
			#define TC_SURF_PASS_VAR_DECAL_LAYER float4 _DecalTex1_ST; float4 _DecalTex2_ST; float4 _DecalTex3_ST;
		#endif
	#endif

	// functions --------------------------------------
	#ifndef TC_SURF_PASS_VERT_DECAL_LAYER
		#ifdef _DECAL_LAYER_1
			#define TC_SURF_PASS_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.pack2.zw = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL1, _DecalTex1);
		#elif defined(_DECAL_LAYER_2)
			#define TC_SURF_PASS_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.pack2.zw = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL1, _DecalTex1); \
				_V2FDATA.pack3.xy = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL2, _DecalTex2);
		#else   // defined(_DECAL_LAYER_3)
			#define TC_SURF_PASS_VERT_DECAL_LAYER(_APPDATA, _V2FDATA) \
				_V2FDATA.pack2.zw = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL1, _DecalTex1); \
				_V2FDATA.pack3.xy = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL2, _DecalTex2); \
				_V2FDATA.pack3.zw = TRANSFORM_TEX(_APPDATA.TEXCOORD_DECAL3, _DecalTex3);
		#endif
	#endif

	#ifndef TC_SURF_PASS_FRAG_DECAL_LAYER
		#ifdef _DECAL_LAYER_1
			#define TC_SURF_PASS_FRAG_DECAL_LAYER(_V2FDATA, _SURF_IN) \
				_SURF_IN.UV_DECAL1 = _V2FDATA.pack2.zw;
		#elif defined(_DECAL_LAYER_2)
			#define TC_SURF_PASS_FRAG_DECAL_LAYER(_V2FDATA, _SURF_IN) \
				_SURF_IN.UV_DECAL1 = _V2FDATA.pack2.zw; \
				_SURF_IN.UV_DECAL2 = _V2FDATA.pack3.xy;
		#else   // defined(_DECAL_LAYER_3)
			#define TC_SURF_PASS_FRAG_DECAL_LAYER(_V2FDATA, _SURF_IN) \
				_SURF_IN.UV_DECAL1 = _V2FDATA.pack2.zw; \
				_SURF_IN.UV_DECAL2 = _V2FDATA.pack3.xy; \
				_SURF_IN.UV_DECAL3 = _V2FDATA.pack3.zw;
		#endif
	#endif

#endif  // #if defined(_DECAL_LAYER_1) || defined(_DECAL_LAYER_2) || defined(_DECAL_LAYER_3)

#endif  // #ifndef TCCG_SURF_UNIT_DECAL_LAYER
