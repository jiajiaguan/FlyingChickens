#ifndef TCCG_VF_UBER_UI
#define TCCG_VF_UBER_UI


// includes --------------------------------------------
#include "UnityCG.cginc"
#include "UnityUI.cginc"
#include "../CGUnits/TCCG_VFUnit_Toonmap.cginc"
#include "../CGUnits/TCCG_VFUnit_Discolor.cginc"


// custom (default) ------------------------------------

// datas (appdata) -------------
#ifndef TC_VF_APPDATA_TOONMAP
	#define TC_VF_APPDATA_TOONMAP
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

#ifndef TC_VF_VERT_CUSTOM
	#define TC_VF_VERT_CUSTOM
#endif

// functions (frag) ------------
#ifndef TC_VF_FRAG_TOONMAP
	#define TC_VF_FRAG_TOONMAP(_V2FDATA, _mainTex)
#endif

#ifndef TC_VF_FRAG_DISCOLOR
	#define TC_VF_FRAG_DISCOLOR(_mainTex)
#endif

#ifndef TC_VF_FRAG_MAIN_CUSTOM
	#define TC_VF_FRAG_MAIN_CUSTOM(_mainTex, _mainColor) \
		_mainTex *= _mainColor;
#endif


// structs ---------------------------------------------
struct appdata_t
{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;

	TC_VF_APPDATA_TOONMAP

	TC_VF_APPDATA_CUSTOM
};

struct v2f {
	float4 vertex   : SV_POSITION;
	fixed4 color    : COLOR;
	half2 texcoord  : TEXCOORD0;
	float4 worldPosition : TEXCOORD2;

	TC_VF_V2FDATA_TOONMAP

	TC_VF_V2FDATA_CUSTOM

	UNITY_VERTEX_OUTPUT_STEREO
};


// properties ------------------------------------------
float4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;

fixed4 _TextureSampleAdd;

#ifdef UNITY_UI_CLIP_RECT
	float4 _ClipRect;
#endif


// vertex function -------------------------------------
v2f vert (appdata_t v)
{
	v2f OUT;

	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

	OUT.worldPosition = v.vertex;
	OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

	OUT.texcoord = v.texcoord;

	OUT.color = v.color * _Color;

	// toonmap
	TC_VF_VERT_TOONMAP(v, OUT)

	// custom
	TC_VF_VERT_CUSTOM

	return OUT;
}


// fragment function  ----------------------------------
float4 frag (v2f IN) : SV_Target
{
	half4 color = (tex2D (_MainTex, IN.texcoord) + _TextureSampleAdd);

	// discolor
	TC_VF_FRAG_DISCOLOR(color)

	// blend main color (custom)
	TC_VF_FRAG_MAIN_CUSTOM(color, IN.color)

	#ifdef UNITY_UI_CLIP_RECT
		color.a *= UnityGet2DClipping (IN.worldPosition.xy, _ClipRect);
	#endif

	#ifdef UNITY_UI_ALPHACLIP
		clip (color.a - 0.001);
	#endif

	// toonmap
	TC_VF_FRAG_TOONMAP(IN, color)

	return color;
}

#endif  // #ifndef TCCG_VF_UBER_UNLIT
