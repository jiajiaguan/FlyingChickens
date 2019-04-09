#ifndef TCCG_VF_UNIT_TOONMAP
#define TCCG_VF_UNIT_TOONMAP

#ifdef _TOONMAP_ON

	// datas ------------------------------------------
	#ifndef TC_VF_APPDATA_TOONMAP
		#define TC_VF_APPDATA_TOONMAP float3 normal : NORMAL;
	#endif

    #ifndef TC_VF_V2FDATA_TOONMAP
        #define TC_VF_V2FDATA_TOONMAP float3 cubenormal : TEXCOORD1;
    #endif

	// properties -------------------------------------
	samplerCUBE _ToonCube;

	// functions --------------------------------------
	#ifndef TC_VF_VERT_TOONMAP
		#define TC_VF_VERT_TOONMAP(_APPDATA, _V2FDATA) \
		    _V2FDATA.cubenormal = UnityObjectToViewPos (float4 (_APPDATA.normal, 0));
	#endif
	
	#ifndef TC_VF_FRAG_TOONMAP
		#define TC_VF_FRAG_TOONMAP(_V2FDATA, _mainTex) \
		    half4 toonCube = texCUBE(_ToonCube, _V2FDATA.cubenormal); \
		    _mainTex.rgb = _mainTex.rgb * toonCube.rgb * 2.0;
	#endif
	
#endif  // #ifdef _TOONMAP_ON

#endif  // #ifndef TCCG_VF_UNIT_TOONMAP
