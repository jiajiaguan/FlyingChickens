#ifndef TCCG_SURF_UNIT_NORMALMAP
#define TCCG_SURF_UNIT_NORMALMAP

#ifdef _NORMALMAP_ON

	// datas ------------------------------------------
	#ifndef TC_SURF_INPUTDATA_NORMALMAP
		#define TC_SURF_INPUTDATA_NORMALMAP float2 uv_BumpMap;
	#endif

	// properties -------------------------------------
	sampler2D _BumpMap;

	// functions --------------------------------------
	#ifndef TC_SURF_NORMALMAP
		#define TC_SURF_NORMALMAP(_IN, _OUT, _blendFactor) \
			fixed3 normal = UnpackNormal (tex2D (_BumpMap, _IN.uv_BumpMap)); \
			_OUT.Normal = lerp (fixed3 (0, 0, 1), normal, ceil (_blendFactor));
	#endif


	// v/f pass =======================================

	// variable ---------------------------------------
	#ifndef TC_SURF_PASS_VAR_NORMALMAP
		#define TC_SURF_PASS_VAR_NORMALMAP float4 _BumpMap_ST;
	#endif

	// functions --------------------------------------
	#ifndef TC_SURF_PASS_VERT_NORMALMAP
		#define TC_SURF_PASS_VERT_NORMALMAP(_APPDATA, _V2FDATA) \
			_V2FDATA.pack1.zw = TRANSFORM_TEX(_APPDATA.texcoord, _BumpMap);
	#endif

	#ifndef TC_SURF_PASS_FRAG_NORMALMAP
		#define TC_SURF_PASS_FRAG_NORMALMAP(_V2FDATA, _SURF_IN) \
			_SURF_IN.uv_BumpMap = _V2FDATA.pack1.zw;;
	#endif

#endif  // #ifdef _NORMALMAP_ON

#endif  // #ifndef TCCG_SURF_UNIT_NORMALMAP
