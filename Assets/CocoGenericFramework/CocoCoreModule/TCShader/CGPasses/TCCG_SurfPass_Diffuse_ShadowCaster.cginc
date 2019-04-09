#ifndef TCCG_SURF_UBER_DIFFUSE_SHADOWCASTER
#define TCCG_SURF_UBER_DIFFUSE_SHADOWCASTER

#ifndef UNITY_PASS_SHADOWCASTER
	#define UNITY_PASS_SHADOWCASTER
#endif


// includes ----------------------------------------------------
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

#include "../CGUbers/TCCG_SurfUber_Diffuse.cginc"


// custom (default) --------------------------------------------

// variables -------------------
#ifndef TC_SURF_PASS_VAR_NORMALMAP
	#define TC_SURF_PASS_VAR_NORMALMAP
#endif
#ifndef TC_SURF_PASS_VAR_BOTTOM_LAYER
	#define TC_SURF_PASS_VAR_BOTTOM_LAYER
#endif
#ifndef TC_SURF_PASS_VAR_DECAL_LAYER
	#define TC_SURF_PASS_VAR_DECAL_LAYER
#endif
#ifndef TC_SURF_PASS_VAR_CUSTOM
	#define TC_SURF_PASS_VAR_CUSTOM
#endif

// functions (vert) ------------
#ifndef TC_SURF_PASS_VERT_NORMALMAP
	#define TC_SURF_PASS_VERT_NORMALMAP(_APPDATA, _V2FDATA)
#endif
#ifndef TC_SURF_PASS_VERT_BOTTOM_LAYER
	#define TC_SURF_PASS_VERT_BOTTOM_LAYER(_APPDATA, _V2FDATA)
#endif
#ifndef TC_SURF_PASS_VERT_DECAL_LAYER
	#define TC_SURF_PASS_VERT_DECAL_LAYER(_APPDATA, _V2FDATA)
#endif
#ifndef TC_SURF_PASS_VERT_CUSTOM
	#define TC_SURF_PASS_VERT_CUSTOM(_APPDATA, _V2FDATA)
#endif

// functions (frag) ------------
#ifndef TC_SURF_PASS_FRAG_NORMALMAP
	#define TC_SURF_PASS_FRAG_NORMALMAP(_V2FDATA, _SURF_IN)
#endif
#ifndef TC_SURF_PASS_FRAG_BOTTOM_LAYER
	#define TC_SURF_PASS_FRAG_BOTTOM_LAYER(_V2FDATA, _SURF_IN)
#endif
#ifndef TC_SURF_PASS_FRAG_DECAL_LAYER
	#define TC_SURF_PASS_FRAG_DECAL_LAYER(_V2FDATA, _SURF_IN)
#endif
#ifndef TC_SURF_PASS_FRAG_REFLECTION
	#define TC_SURF_PASS_FRAG_REFLECTION(_V2FDATA, _SURF_IN, _worldViewDir)
#endif
#ifndef TC_SURF_PASS_FRAG_RIM_WRAP
	#define TC_SURF_PASS_FRAG_RIM_WRAP(_V2FDATA, _SURF_IN, _worldViewDir)
#endif
#ifndef TC_SURF_PASS_FRAG_CUSTOM
	#define TC_SURF_PASS_FRAG_CUSTOM(_V2FDATA, _SURF_IN)
#endif


// v2f data ----------------------------------------------------
struct v2f_surf {
	V2F_SHADOW_CASTER;

	half2 pack0 : TEXCOORD1; // _MainTex

	float3 worldPos : TEXCOORD2;

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


// variables ---------------------------------------------------
float4 _MainTex_ST;


// functions (vert) --------------------------------------------
v2f_surf vert_surf (appdata_full v) {
	UNITY_SETUP_INSTANCE_ID(v);
	v2f_surf o;
	UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
	UNITY_TRANSFER_INSTANCE_ID(v,o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	o.worldPos = worldPos;

	TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
	return o;
}


// functions (frag) --------------------------------------------
fixed4 frag_surf (v2f_surf IN) : SV_Target {
	UNITY_SETUP_INSTANCE_ID(IN);
	// prepare and unpack data
	Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input,surfIN);

	surfIN.uv_MainTex = IN.pack0.xy;

	float3 worldPos = IN.worldPos;
	#ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	#else
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;
	#endif

	#ifdef UNITY_COMPILER_HLSL
		SurfaceOutput o = (SurfaceOutput)0;
	#else
	 SurfaceOutput o;
	#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Specular = 0.0;
	o.Alpha = 0.0;
	o.Gloss = 0.0;
	fixed3 normalWorldVertex = fixed3(0,0,1);

	// call surface function
	surf (surfIN, o);

	SHADOW_CASTER_FRAGMENT(IN)
}


#endif  // #ifndef TCCG_SURF_UBER_DIFFUSE_SHADOWCASTER
