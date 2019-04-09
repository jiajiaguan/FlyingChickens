#ifndef TCCG_SURF_UNIT_SPECULAR
#define TCCG_SURF_UNIT_SPECULAR

#ifdef _SPECULAR_ON

	// properties -------------------------------------
	half _Shininess;

	// functions -------------------------------------
	#ifndef TC_SURF_SPECULAR
		#define TC_SURF_SPECULAR(_OUT, _reflGloss, _texBlendFactor) \
			_SpecColor *= ceil (_texBlendFactor); \
			_OUT.Gloss = _reflGloss * _texBlendFactor; \
			_OUT.Specular = _Shininess;
	#endif

#endif  // #ifdef _SPECULAR_ON

#endif  // #ifndef TCCG_SURF_UNIT_SPECULAR
