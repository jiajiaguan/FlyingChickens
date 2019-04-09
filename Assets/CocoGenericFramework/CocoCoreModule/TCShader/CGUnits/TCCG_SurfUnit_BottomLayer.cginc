#ifndef TCCG_SURF_UNIT_BOTTOM_LAYER
#define TCCG_SURF_UNIT_BOTTOM_LAYER

#ifdef _BOTTOM_LAYER_ON

	// datas ------------------------------------------
	#ifndef TC_SURF_INPUTDATA_BOTTOM_LAYER
		#define TC_SURF_INPUTDATA_BOTTOM_LAYER float2 uv_BottomTex;
	#endif

	// properties -------------------------------------
	sampler2D _BottomTex;

	// functions --------------------------------------
	#ifndef TC_SURF_BOTTOM_LAYER
		#define TC_SURF_BOTTOM_LAYER(_IN, _mainTex, _mainColor, _colorFactor) \
			fixed4 botTex = tex2D (_BottomTex, _IN.uv_BottomTex) * _mainColor; \
			_mainTex.rgb = lerp (botTex.rgb, _mainTex.rgb, _colorFactor);
	#endif


	// v/f pass =======================================

	// variable ---------------------------------------
	#ifndef TC_SURF_PASS_VAR_BOTTOM_LAYER
		#define TC_SURF_PASS_VAR_BOTTOM_LAYER float4 _BottomTex_ST;
	#endif

	// functions --------------------------------------
	#ifndef TC_SURF_PASS_VERT_BOTTOM_LAYER
		#define TC_SURF_PASS_VERT_BOTTOM_LAYER(_APPDATA, _V2FDATA) \
			_V2FDATA.pack2.xy = TRANSFORM_TEX(_APPDATA.texcoord, _BottomTex);
	#endif

	#ifndef TC_SURF_PASS_FRAG_BOTTOM_LAYER
		#define TC_SURF_PASS_FRAG_BOTTOM_LAYER(_V2FDATA, _SURF_IN) \
			_SURF_IN.uv_BottomTex = _V2FDATA.pack2.xy;
	#endif

#endif  // #ifdef _REFLECTION_ON

#endif  // #ifndef TCCG_SURF_UNIT_BOTTOM_LAYER
