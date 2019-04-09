#ifndef TCCG_SURF_UBER_DIFFUSE
#define TCCG_SURF_UBER_DIFFUSE


// includes ----------------------------------------------------
#include "../CGUnits/TCCG_SurfUnit_Specular.cginc"
#include "../CGUnits/TCCG_SurfUnit_Normalmap.cginc"
#include "../CGUnits/TCCG_SurfUnit_Reflection.cginc"
#include "../CGUnits/TCCG_SurfUnit_BottomLayer.cginc"
#include "../CGUnits/TCCG_SurfUnit_DecalLayer.cginc"
#include "../CGUnits/TCCG_SurfUnit_RimWrap.cginc"
#include "../CGUnits/TCCG_SurfUnit_Discolor.cginc"


// custom (default) --------------------------------------------

// datas -----------------------
#ifndef TC_SURF_INPUTDATA_NORMALMAP
	#define TC_SURF_INPUTDATA_NORMALMAP
#endif
#ifndef TC_SURF_INPUTDATA_REFLECTION
	#define TC_SURF_INPUTDATA_REFLECTION
#endif
#ifndef TC_SURF_INPUTDATA_BOTTOM_LAYER
	#define TC_SURF_INPUTDATA_BOTTOM_LAYER
#endif
#ifndef TC_SURF_INPUTDATA_DECAL_LAYER
	#define TC_SURF_INPUTDATA_DECAL_LAYER
#endif
#ifndef TC_SURF_INPUTDATA_RIM_WRAP
	#define TC_SURF_INPUTDATA_RIM_WRAP
#endif

#ifndef TC_SURF_INPUTDATA_CUSTOM
	#define TC_SURF_INPUTDATA_CUSTOM
#endif

// functions -------------------
#ifndef TC_SURF_SPECULAR
	#define TC_SURF_SPECULAR(_OUT, _reflGloss, _texBlendFactor)
#endif

#ifndef TC_SURF_NORMALMAP
	#define TC_SURF_NORMALMAP(_IN, _OUT, _blendFactor) \
		_OUT.Normal = fixed3 (0, 0, 1);
#endif

#ifndef TC_SURF_REFLECTION
	#define TC_SURF_REFLECTION(_IN, _OUT)
#endif

#ifndef TC_SURF_BOTTOM_LAYER
	#define TC_SURF_BOTTOM_LAYER(_IN, _mainTex, _mainColor, _colorFactor)
#endif

#ifndef TC_SURF_DECAL_LAYER
	#define TC_SURF_DECAL_LAYER(_IN, _mainTex)
#endif

#ifndef TC_SURF_RIM_WRAP
	#define TC_SURF_RIM_WRAP(_IN, _OUT)
#endif

#ifndef TC_SURF_DISCOLOR
	#define TC_SURF_DISCOLOR(_mainTex)
#endif

#ifndef TC_SURF_MAIN_CUSTOM
	#define TC_SURF_MAIN_CUSTOM(_mainTex, _mainColor, _colorFactor) \
		_mainTex *= lerp (fixed4 (1, 1, 1, 1), _mainColor, _colorFactor);
#endif

#if defined(_REFLECTION_ON) || defined(_BOTTOM_LAYER_ON)
    #define USE_PROPERTY_FACTOR_TEX
#endif

// structs -------------------------------------
struct Input {
	half2 uv_MainTex;

	#ifdef USE_PROPERTY_FACTOR_TEX
		float2 uv_PropertyFactorTex;
	#endif

	TC_SURF_INPUTDATA_NORMALMAP
	TC_SURF_INPUTDATA_REFLECTION
	TC_SURF_INPUTDATA_BOTTOM_LAYER
	TC_SURF_INPUTDATA_DECAL_LAYER
	TC_SURF_INPUTDATA_RIM_WRAP

	TC_SURF_INPUTDATA_CUSTOM

	INTERNAL_DATA
};


// properties -------------------------------------
fixed4 _Color;
sampler2D _MainTex;
half _ColorFactor;

#ifdef _ALPHATEST_ON
	half _Cutoff;
#endif

#ifdef USE_PROPERTY_FACTOR_TEX
    sampler2D _PropertyFactorTex;
#endif


// surface function -------------------------------------
void surf (Input IN, inout SurfaceOutput o) {
	// property -----------------------
	fixed texBlendFactor = 1;
	fixed reflGlossFactor = 1;

	#ifdef USE_PROPERTY_FACTOR_TEX
		fixed4 propertyFactorTex = tex2D (_PropertyFactorTex, IN.uv_PropertyFactorTex);
		#ifdef _REFLECTION_ON
			reflGlossFactor = propertyFactorTex.g;
		#endif
		#ifdef _BOTTOM_LAYER_ON
			texBlendFactor = propertyFactorTex.r;
		#endif
	#endif

	// albedo -------------------------
	// main texture
	fixed4 mainTex = tex2D (_MainTex, IN.uv_MainTex);

	// discolor
	TC_SURF_DISCOLOR(mainTex)

	// blend decal layer
	TC_SURF_DECAL_LAYER(IN, mainTex)

	// blend main color (custom)
	TC_SURF_MAIN_CUSTOM(mainTex, _Color, _ColorFactor)

	// blend bottom layer
	TC_SURF_BOTTOM_LAYER(IN, mainTex, _Color, propertyFactorTex.r)

	o.Albedo = mainTex.rgb;

	// alpha --------------------------
	#if defined(_ALPHATEST_ON) || defined(_ALPHABLEND_ON)
		o.Alpha = mainTex.a;

		#ifdef _ALPHATEST_ON
			clip (o.Alpha - _Cutoff);
		#endif
	#else
		o.Alpha = 1;
	#endif

	// normal -------------------------
	TC_SURF_NORMALMAP(IN, o, texBlendFactor)

	// specular -----------------------
	TC_SURF_SPECULAR(o, (mainTex.a * reflGlossFactor), texBlendFactor)

	// emission & gloss ---------------
	o.Emission = 0;
	TC_SURF_REFLECTION (IN, o)
	TC_SURF_RIM_WRAP(IN, o)
}

#endif  // #ifndef TCCG_SURF_UBER_DIFFUSE
