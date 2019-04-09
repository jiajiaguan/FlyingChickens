#ifndef TCCG_SURF_UNIT_RIM_WRAP
#define TCCG_SURF_UNIT_RIM_WRAP

#ifdef _RIM_WRAP_ON

	// datas ------------------------------------------
	#ifndef TC_SURF_INPUTDATA_RIM_WRAP
		#define TC_SURF_INPUTDATA_RIM_WRAP half3 viewDir;
	#endif

	// properties -------------------------------------
	// wrap
	half _WrapPower;
	half _LightPower;
	// rim
	fixed4 _RimColor;
	half _RimPower;

	// functions --------------------------------------
	#ifndef TC_SURF_RIM_WRAP
		#define TC_SURF_RIM_WRAP(_IN, _OUT) \
			half rim = 1.0 - saturate (dot (normalize (_IN.viewDir), _OUT.Normal)); \
			_OUT.Emission += _RimColor.rgb * pow (rim, _RimPower);
	#endif

	// lighting functions -----------------------------
	#ifdef _SPECULAR_ON

		// blinn phong ----------------
		inline fixed4 WrapBlinnPhongLight (SurfaceOutput s, half3 viewDir, UnityGI gi)
		{
			half NdotL = max (0, dot (s.Normal, gi.light.dir));
			half diff = (NdotL * (1 - _WrapPower) + _WrapPower) * _LightPower;

			half3 h = normalize (gi.light.dir + viewDir);
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0) * s.Gloss;

			fixed4 c;
			c.rgb = s.Albedo * gi.light.color * diff + gi.light.color * _SpecColor.rgb * spec;
			c.a = s.Alpha;

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
			#endif

			return c;
		}

	#else   // #ifdef _SPECULAR_ON

		// lambert --------------------
		inline fixed4 WrapLambertLight (SurfaceOutput s, UnityGI gi)
		{
			half NdotL = max (0, dot (s.Normal, gi.light.dir));
			half diff = (NdotL * (1 - _WrapPower) + _WrapPower) * _LightPower;

			fixed4 c;
			c.rgb = s.Albedo * gi.light.color * diff;
			c.a = s.Alpha;

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
			#endif

			return c;
		}

	#endif   // #ifdef _SPECULAR_ON


	// v/f pass =======================================

	// functions --------------------------------------
	#ifndef TC_SURF_PASS_FRAG_RIM_WRAP
		#define TC_SURF_PASS_FRAG_RIM_WRAP(_V2FDATA, _SURF_IN, _worldViewDir) \
			float3 viewDir = _V2FDATA.tSpace0.xyz * _worldViewDir.x + _V2FDATA.tSpace1.xyz * _worldViewDir.y + _V2FDATA.tSpace2.xyz * _worldViewDir.z; \
			_SURF_IN.viewDir = viewDir;
	#endif

#endif  // #ifdef _RIM_WRAP_ON


// lighting functions -------------------------------------

inline fixed4 LightingWrap (SurfaceOutput s, half3 viewDir, UnityGI gi)
{
	#ifdef _RIM_WRAP_ON
		#ifdef _SPECULAR_ON
			return WrapBlinnPhongLight (s, viewDir, gi);
		#else
			return WrapLambertLight (s, gi);
		#endif
	#else
		// no wrap, use default lighting functions
		#ifdef _SPECULAR_ON
			return LightingBlinnPhong (s, viewDir, gi);
		#else
			return LightingLambert (s, gi);
		#endif
	#endif
}

inline void LightingWrap_GI (SurfaceOutput s, UnityGIInput data, inout UnityGI gi)
{
	gi = UnityGlobalIllumination (data, 1.0, s.Normal);
}


#endif  // #ifndef TCCG_SURF_UNIT_RIM_WRAP
