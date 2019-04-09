using System.Collections.Generic;

namespace TC.Shader
{
	public class TCUIShaderGUI : TCBaseShaderGUI
	{
		#region Rendering Mode

		protected override bool IsRenderingModeEnabled {
			get { return false; }
		}

		#endregion


		#region Property Basic

		private readonly List<string> _basicPropertyNames = new List<string> {
			TCShaderUtil.PROPERTY_MAIN_TEX,
			TCShaderUtil.PROPERTY_MAIN_COLOR
		};

		protected override List<string> BasicPropertyNames {
			get { return _basicPropertyNames; }
		}

		#endregion


		#region Property Group

		private readonly List<TCPropertyGroup> _propertyGroups = new List<TCPropertyGroup> {
			TCShaderUtil.ToonmapPropertyGroup,
			TCShaderUtil.DiscolorPropertyGroup
		};

		protected override List<TCPropertyGroup> PropertyGroups {
			get { return _propertyGroups; }
		}

		#endregion
	}
}