#ifndef TCCG_SURF_UBER_DIFFUSE_META
#define TCCG_SURF_UBER_DIFFUSE_META

#ifndef UNITY_PASS_META
	#define UNITY_PASS_META
#endif


// includes ----------------------------------------------------
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

#include "../CGUbers/TCCG_SurfUber_Diffuse.cginc"
#include "UnityMetaPass.cginc"


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
	UNITY_POSITION(pos);

	half2 pack0 : TEXCOORD0; // _MainTex
	float4 pack1 : TEXCOORD1; // _PropertyFactorTex _BumpMap
	float4 pack2 : TEXCOORD2; // _BottomTex _DecalTex1
	float4 pack3 : TEXCOORD3; // _DecalTex2 _DecalTex3

	float4 tSpace0 : TEXCOORD4;
	float4 tSpace1 : TEXCOORD5;
	float4 tSpace2 : TEXCOORD6;

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


// variables ---------------------------------------------------
float4 _MainTex_ST;
#ifdef USE_PROPERTY_FACTOR_TEX
	float4 _PropertyFactorTex_ST;
#endif

TC_SURF_PASS_VAR_NORMALMAP
TC_SURF_PASS_VAR_BOTTOM_LAYER
TC_SURF_PASS_VAR_DECAL_LAYER
TC_SURF_PASS_VAR_CUSTOM


// functions (vert) --------------------------------------------
v2f_surf vert_surf (appdata_full v) {
	UNITY_SETUP_INSTANCE_ID(v);

	v2f_surf o;
	UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
	UNITY_TRANSFER_INSTANCE_ID(v,o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);

	// uv
	o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	#ifdef USE_PROPERTY_FACTOR_TEX
		o.pack1.xy = TRANSFORM_TEX(v.texcoord, _PropertyFactorTex);
	#endif
	TC_SURF_PASS_VERT_NORMALMAP(v, o)
	TC_SURF_PASS_VERT_BOTTOM_LAYER(v, o)
	TC_SURF_PASS_VERT_DECAL_LAYER(v, o)
	TC_SURF_PASS_VERT_CUSTOM(v, o)

	// space
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
	fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
	o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
	o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
	o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

	return o;
}


// functions (frag) --------------------------------------------
fixed4 frag_surf (v2f_surf IN) : SV_Target {
	UNITY_SETUP_INSTANCE_ID(IN);
	// prepare and unpack data
	Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input,surfIN);

	surfIN.uv_MainTex = IN.pack0.xy;
	#ifdef USE_PROPERTY_FACTOR_TEX
		surfIN.uv_PropertyFactorTex = IN.pack1.xy;
	#endif

    TC_SURF_PASS_FRAG_NORMALMAP(IN, surfIN)
	TC_SURF_PASS_FRAG_BOTTOM_LAYER(IN, surfIN)
	TC_SURF_PASS_FRAG_DECAL_LAYER(IN, surfIN)
	TC_SURF_PASS_FRAG_CUSTOM(IN, surfIN)

	// space
	float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
	#ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	#else
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;
	#endif
	float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

	// surface
	TC_SURF_PASS_FRAG_REFLECTION(IN, surfIN, worldViewDir)
	TC_SURF_PASS_FRAG_RIM_WRAP(IN, surfIN, worldViewDir)

	surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
	surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
	surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;

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

	UnityMetaInput metaIN;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
	metaIN.Albedo = o.Albedo;
	metaIN.Emission = o.Emission;
	metaIN.SpecularColor = o.Specular;
	return UnityMetaFragment(metaIN);
}

#endif  // #ifndef TCCG_SURF_UBER_DIFFUSE_META
