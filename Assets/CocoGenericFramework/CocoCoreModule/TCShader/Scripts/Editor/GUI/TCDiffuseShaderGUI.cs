using System.Collections.Generic;

namespace TC.Shader
{
	public class TCDiffuseShaderGUI : TCBaseShaderGUI
	{
		#region Rendering Mode

		protected override bool IsRenderingModeEnabled {
			get { return true; }
		}

		#endregion


		#region Property Basic

		private readonly List<string> _basicPropertyNames = new List<string> {
			TCShaderUtil.PROPERTY_MAIN_COLOR,
			TCShaderUtil.PROPERTY_MAIN_TEX,
			TCShaderUtil.PROPERTY_COLOR_FACTOR
		};

		protected override List<string> BasicPropertyNames {
			get { return _basicPropertyNames; }
		}

		#endregion


		#region Property Group

		private readonly List<TCPropertyGroup> _propertyGroups = new List<TCPropertyGroup> {
			TCShaderUtil.AlphaTestPropertyGroup,
			TCShaderUtil.SpecularPropertyGroup,
			TCShaderUtil.NormalmapPropertyGroup,
			TCShaderUtil.ReflectionPropertyGroup,
			TCShaderUtil.BottomLayerPropertyGroup,
			new TCKeywordPropertyGroup {
				SubNames = new Dictionary<string, string[]> {
					{ TCShaderUtil.KEYWORD_REFLECTION_ON, new[] { TCShaderUtil.PROPERTY_FACTOR_TEX } },
					{ TCShaderUtil.KEYWORD_BOTTOM_LAYER_ON, new[] { TCShaderUtil.PROPERTY_FACTOR_TEX } }
				}
			},
			TCShaderUtil.DecalLayerPropertyGroup,
			TCShaderUtil.RimWrapPropertyGroup,
			TCShaderUtil.DiscolorPropertyGroup
		};

		protected override List<TCPropertyGroup> PropertyGroups {
			get { return _propertyGroups; }
		}

		#endregion
	}
}