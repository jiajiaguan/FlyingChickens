#ifndef TCCG_SURF_UNIT_DISCOLOR
#define TCCG_SURF_UNIT_DISCOLOR

#if defined(_DISCOLOR_HSL_BLEND) || defined(_DISCOLOR_HUE_REPLACE)

	#include "../CGUtils/TCCG_Util_ColorModel.cginc"

	#ifdef _DISCOLOR_HSL_BLEND

		// properties -------------------------------------
		half _Hue;
		half _Saturation;
		half _Lightness;
		half _MixingFactor;

		// functions ------------------------------------------
		#ifndef TC_SURF_DISCOLOR
			#define TC_SURF_DISCOLOR(_mainTex) \
				_mainTex.rgb = blend_hsl_to_rgb (_mainTex.rgb, _Hue, _Saturation, _Lightness) * _MixingFactor;
		#endif

	#else // defined _DISCOLOR_HUE_REPLACE

	   // properties -------------------------------------
		half4 _HueColor;
		half _SaturMin;
		half _SaturRatio;
		half _SaturAdd;
		half _LightMax;
		half _LightRatio;
		half _LightAdd;

		// functions ------------------------------------------
		#ifndef TC_SURF_DISCOLOR
			#define TC_SURF_DISCOLOR(_mainTex) \
				float3 hsl = rgb_to_hsv (_HueColor.rgb); \
				_mainTex.rgb = replace_hue_by_hsv_fix (_mainTex.rgb, hsl.x, _SaturMin, _SaturRatio, _SaturAdd, _LightMax, _LightRatio, _LightAdd);
		#endif

	#endif  // #ifdef _DISCOLOR_HSL_BLEND

#endif  // #if defined(_DISCOLOR_HSL_BLEND) || defined(_DISCOLOR_HUE_REPLACE)

#endif  // #ifndef TCCG_SURF_UNIT_DISCOLOR
