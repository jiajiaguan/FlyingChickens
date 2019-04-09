using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TC.Shader
{
	public class TCShaderUtil
	{
		#region Property Key

		// base
		public const string PROPERTY_MAIN_TEX = "_MainTex";
		public const string PROPERTY_MAIN_COLOR = "_Color";
		public const string PROPERTY_COLOR_FACTOR = "_ColorFactor";

		// rending mode
		public const string PROPERTY_RENDING_MODE = "_Mode";
		public const string PROPERTY_CUTOFF = "_Cutoff";

		// light
		public const string PROPERTY_SPECULAR = "_Specular";
		public const string PROPERTY_SPEC_COLOR = "_SpecColor";
		public const string PROPERTY_SHININESS = "_Shininess";

		public const string PROPERTY_REFLECTION = "_Reflection";
		public const string PROPERTY_REFLECT_COLOR = "_ReflectColor";
		public const string PROPERTY_REFLECT_CUBE = "_ReflectCube";

		public const string PROPERTY_RIM_WRAP = "_RimWrap";
		public const string PROPERTY_RIM_COLOR = "_RimColor";
		public const string PROPERTY_RIM_POWER = "_RimPower";
		public const string PROPERTY_WRAP_POWER = "_WrapPower";
		public const string PROPERTY_LIGHT_POWER = "_LightPower";

		// cubemap
		public const string PROPERTY_NORMALMAP = "_Normalmap";
		public const string PROPERTY_NORMAL_BUMP_MAP = "_BumpMap";

		public const string PROPERTY_TOONMAP = "_Toonmap";
		public const string PROPERTY_TOONMAP_CUBE = "_ToonCube";

		// discolor
		public const string PROPERTY_DISCOLOR = "_Discolor";

		// hls blend
		public const string PROPERTY_HUE = "_Hue";
		public const string PROPERTY_SATURATION = "_Saturation";
		public const string PROPERTY_LIGHTNESS = "_Lightness";
		public const string PROPERTY_MIXING_FACTOR = "_MixingFactor";

		// hue replace
		public const string PROPERTY_HUE_COLOR = "_HueColor";
		public const string PROPERTY_SATUR_MIN = "_SaturMin";
		public const string PROPERTY_SATUR_RATIO = "_SaturRatio";
		public const string PROPERTY_SATUR_ADD = "_SaturAdd";
		public const string PROPERTY_LIGHT_MAX = "_LightMax";
		public const string PROPERTY_LIGHT_RATIO = "_LightRatio";
		public const string PROPERTY_LIGHT_ADD = "_LightAdd";

		// bottom layer
		public const string PROPERTY_BOTTOM_LAYER = "_BottomLayer";
		public const string PROPERTY_BOTTOM_TEX = "_BottomTex";

		// decal layer
		public const string PROPERTY_DECAL_LAYER = "_Decal";
		public const string PROPERTY_DECAL_TEX1 = "_DecalTex1";
		public const string PROPERTY_DECAL_TEX1_UV2 = "_DecalTex1UV2";
		public const string PROPERTY_DECAL_TEX2 = "_DecalTex2";
		public const string PROPERTY_DECAL_TEX2_UV2 = "_DecalTex2UV2";
		public const string PROPERTY_DECAL_TEX3 = "_DecalTex3";
		public const string PROPERTY_DECAL_TEX3_UV2 = "_DecalTex3UV2";

		public const string PROPERTY_FACTOR_TEX = "_PropertyFactorTex";

		#endregion


		#region Keyword

		// rending mode
		public const string KEYWORD_ALPHA_TEST_ON = "_ALPHATEST_ON";
		public const string KEYWORD_ALPHA_BLEND_ON = "_ALPHABLEND_ON";

		// light
		public const string KEYWORD_SPECULAR_ON = "_SPECULAR_ON";
		public const string KEYWORD_REFLECTION_ON = "_REFLECTION_ON";
		public const string KEYWORD_RIM_WRAP_ON = "_RIM_WRAP_ON";

		// cubemap
		public const string KEYWORD_NORMALMAP_ON = "_NORMALMAP_ON";
		public const string KEYWORD_TOONMAP_ON = "_TOONMAP_ON";

		// discolor
		public const string KEYWORD_DISCOLOR_HSL_BLEND = "_DISCOLOR_HSL_BLEND";
		public const string KEYWORD_DISCOLOR_HUE_REPLACE = "_DISCOLOR_HUE_REPLACE";

		// bottom layer
		public const string KEYWORD_BOTTOM_LAYER_ON = "_BOTTOM_LAYER_ON";

		// decal layer
		public const string KEYWORD_DECAL_LAYER_1 = "_DECAL_LAYER_1";
		public const string KEYWORD_DECAL_LAYER_2 = "_DECAL_LAYER_2";
		public const string KEYWORD_DECAL_LAYER_3 = "_DECAL_LAYER_3";
		public const string KEYWORD_DECAL_LAYER_1_UV2_ON = "_DECAL_LAYER_1_UV2_ON";
		public const string KEYWORD_DECAL_LAYER_2_UV2_ON = "_DECAL_LAYER_2_UV2_ON";
		public const string KEYWORD_DECAL_LAYER_3_UV2_ON = "_DECAL_LAYER_3_UV2_ON";

		#endregion


		#region Property Group

		// rendering mode
		public static readonly TCPropertyGroup AlphaTestPropertyGroup = new TCTogglePropertyGroup {
			MainKeyword = KEYWORD_ALPHA_TEST_ON,
			SubNames = new[] { PROPERTY_CUTOFF }
		};

		// light
		public static readonly TCPropertyGroup SpecularPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_SPECULAR,
			MainKeyword = KEYWORD_SPECULAR_ON,
			SubNames = new[] { PROPERTY_SPEC_COLOR, PROPERTY_SHININESS }
		};

		public static readonly TCPropertyGroup ReflectionPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_REFLECTION,
			MainKeyword = KEYWORD_REFLECTION_ON,
			SubNames = new[] { PROPERTY_REFLECT_COLOR, PROPERTY_REFLECT_CUBE }
		};

		public static readonly TCPropertyGroup RimWrapPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_RIM_WRAP,
			MainKeyword = KEYWORD_RIM_WRAP_ON,
			SubNames = new[] { PROPERTY_RIM_COLOR, PROPERTY_RIM_POWER, PROPERTY_WRAP_POWER, PROPERTY_LIGHT_POWER }
		};

		// cubemap
		public static readonly TCPropertyGroup NormalmapPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_NORMALMAP,
			MainKeyword = KEYWORD_NORMALMAP_ON,
			SubNames = new[] { PROPERTY_NORMAL_BUMP_MAP }
		};

		public static readonly TCPropertyGroup ToonmapPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_TOONMAP,
			MainKeyword = KEYWORD_TOONMAP_ON,
			SubNames = new[] { PROPERTY_TOONMAP_CUBE }
		};

		// discolor
		public static readonly TCPropertyGroup DiscolorPropertyGroup = new TCKeywordPropertyGroup {
			MainName = PROPERTY_DISCOLOR,
			SubNames = new Dictionary<string, string[]> {
				{
					KEYWORD_DISCOLOR_HSL_BLEND,
					new[] {
						PROPERTY_HUE, PROPERTY_SATURATION, PROPERTY_LIGHTNESS, PROPERTY_MIXING_FACTOR
					}
				}, {
					KEYWORD_DISCOLOR_HUE_REPLACE,
					new[] {
						PROPERTY_HUE_COLOR,
						PROPERTY_SATUR_MIN, PROPERTY_SATUR_RATIO, PROPERTY_SATUR_ADD,
						PROPERTY_LIGHT_MAX, PROPERTY_LIGHT_RATIO, PROPERTY_LIGHT_ADD
					}
				}
			}
		};

		// bottom layer
		public static readonly TCPropertyGroup BottomLayerPropertyGroup = new TCTogglePropertyGroup {
			MainName = PROPERTY_BOTTOM_LAYER,
			MainKeyword = KEYWORD_BOTTOM_LAYER_ON,
			SubNames = new[] { PROPERTY_BOTTOM_TEX }
		};

		// decal layer
		public static readonly TCPropertyGroup DecalLayerPropertyGroup = new TCKeywordPropertyGroup {
			MainName = PROPERTY_DECAL_LAYER,
			SubNames = new Dictionary<string, string[]> {
				{
					KEYWORD_DECAL_LAYER_1,
					new[] {
						PROPERTY_DECAL_TEX1, PROPERTY_DECAL_TEX1_UV2
					}
				}, {
					KEYWORD_DECAL_LAYER_2,
					new[] {
						PROPERTY_DECAL_TEX1, PROPERTY_DECAL_TEX1_UV2,
						PROPERTY_DECAL_TEX2, PROPERTY_DECAL_TEX2_UV2
					}
				}, {
					KEYWORD_DECAL_LAYER_3,
					new[] {
						PROPERTY_DECAL_TEX1, PROPERTY_DECAL_TEX1_UV2,
						PROPERTY_DECAL_TEX2, PROPERTY_DECAL_TEX2_UV2,
						PROPERTY_DECAL_TEX3, PROPERTY_DECAL_TEX3_UV2
					}
				}
			}
		};

		private static readonly int _srcBlend = UnityEngine.Shader.PropertyToID ("_SrcBlend");
		private static readonly int _dstBlend = UnityEngine.Shader.PropertyToID ("_DstBlend");
		private static readonly int _zWrite = UnityEngine.Shader.PropertyToID ("_ZWrite");

		#endregion


		#region Rendering Mode

		// rending mode
		public static void SetupMaterialWithRenderingMode (Material material, TCShaderRenderingMode renderingMode, bool resetQueue)
		{
			var renderQueue = -1;

			switch (renderingMode) {
			case TCShaderRenderingMode.Cutout:
				material.SetOverrideTag ("RenderType", "TransparentCutout");
				material.SetInt (_srcBlend, (int)BlendMode.One);
				material.SetInt (_dstBlend, (int)BlendMode.Zero);
				material.SetInt (_zWrite, 1);
				material.EnableKeyword (KEYWORD_ALPHA_TEST_ON);
				material.DisableKeyword (KEYWORD_ALPHA_BLEND_ON);
				renderQueue = (int)RenderQueue.AlphaTest;
				break;
			case TCShaderRenderingMode.Transparent:
				material.SetOverrideTag ("RenderType", "Transparent");
				material.SetInt (_srcBlend, (int)BlendMode.SrcAlpha);
				material.SetInt (_dstBlend, (int)BlendMode.OneMinusSrcAlpha);
				material.SetInt (_zWrite, 0);
				material.DisableKeyword (KEYWORD_ALPHA_TEST_ON);
				material.EnableKeyword (KEYWORD_ALPHA_BLEND_ON);
				renderQueue = (int)RenderQueue.Transparent;
				break;
			default: // Opaque
				material.SetOverrideTag ("RenderType", "");
				material.SetInt (_srcBlend, (int)BlendMode.One);
				material.SetInt (_dstBlend, (int)BlendMode.Zero);
				material.SetInt (_zWrite, 1);
				material.DisableKeyword (KEYWORD_ALPHA_TEST_ON);
				material.DisableKeyword (KEYWORD_ALPHA_BLEND_ON);
				break;
			}

			if (resetQueue) {
				material.renderQueue = renderQueue;
			}
		}

		#endregion
	}
}