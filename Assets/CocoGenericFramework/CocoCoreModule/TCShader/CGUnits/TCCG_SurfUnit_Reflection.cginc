#ifndef TCCG_SURF_UNIT_REFLECTION
#define TCCG_SURF_UNIT_REFLECTION

#ifdef _REFLECTION_ON

	// datas ------------------------------------------
	#ifndef TC_SURF_INPUTDATA_REFLECTION
		#define TC_SURF_INPUTDATA_REFLECTION float3 worldRefl;
	#endif

	// properties -------------------------------------
	fixed4 _ReflectColor;
	samplerCUBE _ReflectCube;

	// functions --------------------------------------
	#ifndef TC_SURF_REFLECTION
		#define TC_SURF_REFLECTION(_IN, _OUT) \
			float3 worldRefl = WorldReflectionVector (_IN, _OUT.Normal); \
			fixed4 reflcol = texCUBE (_ReflectCube, worldRefl); \
			reflcol *= _OUT.Gloss * _ReflectColor; \
			_OUT.Emission += reflcol.rgb;
	#endif


	// v/f pass =======================================

	// functions --------------------------------------
	#ifndef TC_SURF_PASS_FRAG_REFLECTION
		#define TC_SURF_PASS_FRAG_REFLECTION(_V2FDATA, _SURF_IN, _worldViewDir) \
			_SURF_IN.worldRefl = -_worldViewDir;
	#endif

#endif  // #ifdef _REFLECTION_ON

#endif  // #ifndef TCCG_SURF_UNIT_REFLECTION
