#ifndef TCCG_VF_UBER_UNLIT
#define TCCG_VF_UBER_UNLIT


// includes --------------------------------------------
#include "UnityCG.cginc"
#include "../CGUnits/TCCG_VFUnit_Toonmap.cginc"
#include "../CGUnits/TCCG_VFUnit_DecalLayer.cginc"
#include "../CGUnits/TCCG_VFUnit_Discolor.cginc"


// custom (default) ------------------------------------

// datas (appdata) -------------
#ifndef TC_VF_APPDATA_TOONMAP
	#define TC_VF_APPDATA_TOONMAP
#endif

#ifndef TC_VF_APPDATA_DECAL_LAYER
	#define TC_VF_APPDATA_DECAL_LAYER
#endif

#ifndef TC_VF_APPDATA_CUSTOM
	#define TC_VF_APPDATA_CUSTOM
#endif

// datas (v2f) -----------------
#ifndef TC_VF_V2FDATA_TOONMAP
	#define TC_VF_V2FDATA_TOONMAP
#endif

#ifndef TC_VF_V2FDATA_CUSTOM
	#define TC_VF_V2FDATA_CUSTOM
#endif

// functions (vert) ------------
#ifndef TC_VF_VERT_TOONMAP
	#define TC_VF_VERT_TOONMAP(_APPDATA, _V2FDATA)
#endif

#ifndef TC_VF_VERT_DECAL_LAYER
	#define TC_VF_VERT_DECAL_LAYER(_APPDATA, _V2FDATA)
#endif

#ifndef TC_VF_V2FDATA_DECAL_LAYER
	#define TC_VF_V2FDATA_DECAL_LAYER
#endif

#ifndef TC_VF_VERT_CUSTOM
	#define TC_VF_VERT_CUSTOM
#endif

// functions (frag) ------------
#ifndef TC_VF_FRAG_TOONMAP
	#define TC_VF_FRAG_TOONMAP(_V2FDATA, _mainTex)
#endif

#ifndef TC_VF_FRAG_DECAL_LAYER
	#define TC_VF_FRAG_DECAL_LAYER(_V2FDATA, _mainTex)
#endif

#ifndef TC_VF_FRAG_DISCOLOR
	#define TC_VF_FRAG_DISCOLOR(_mainTex)
#endif

#ifndef TC_VF_FRAG_MAIN_CUSTOM
	#define TC_VF_FRAG_MAIN_CUSTOM(_mainTex, _mainColor) \
		_mainTex *= _mainColor;
#endif


// structs ---------------------------------------------
struct appdata {
	float4 vertex : POSITION;
	float2 uv0 : TEXCOORD0;

	TC_VF_APPDATA_TOONMAP
	TC_VF_APPDATA_DECAL_LAYER

	TC_VF_APPDATA_CUSTOM

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f {
	float4 pos : SV_POSITION;
	float2 texcoord : TEXCOORD0;

	TC_VF_V2FDATA_TOONMAP
	TC_VF_V2FDATA_DECAL_LAYER

	TC_VF_V2FDATA_CUSTOM

	UNITY_VERTEX_OUTPUT_STEREO
};


// properties ------------------------------------------
float4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;

#ifdef _ALPHATEST_ON
	half _Cutoff;
#endif


// vertex function -------------------------------------
v2f vert (appdata v)
{
	v2f o;

	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.pos = UnityObjectToClipPos (v.vertex);
	o.texcoord = TRANSFORM_TEX(v.uv0, _MainTex);

	// toonmap
	TC_VF_VERT_TOONMAP(v, o)

	// decal layer
	TC_VF_VERT_DECAL_LAYER(v, o)

	// custom
	TC_VF_VERT_CUSTOM

	return o;
}


// fragment function  ----------------------------------
float4 frag (v2f i) : SV_Target
{
	fixed4 mainTex = tex2D(_MainTex, i.texcoord);

	// discolor
	TC_VF_FRAG_DISCOLOR(mainTex)

    // blend decal layer
	TC_VF_FRAG_DECAL_LAYER(i, mainTex)

	// blend main color (custom)
	TC_VF_FRAG_MAIN_CUSTOM(mainTex, _Color)

	#ifdef _ALPHATEST_ON
		clip (mainTex.a - _Cutoff);
	#endif

	// toonmap
	TC_VF_FRAG_TOONMAP(i, mainTex)

	return mainTex;
}

#endif  // #ifndef TCCG_VF_UBER_UNLIT
