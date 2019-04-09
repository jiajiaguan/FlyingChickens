using System.Collections.Generic;
using UnityEngine;

namespace TC.Shader
{
	public abstract class TCPropertyGroup
	{
		public string MainName = string.Empty;

		public abstract string[] GetEnabledSubNames (Material material);
	}


	public class TCTogglePropertyGroup : TCPropertyGroup
	{
		public string MainKeyword;

		public string[] SubNames;

		public override string[] GetEnabledSubNames (Material material)
		{
			return material.IsKeywordEnabled (MainKeyword) ? SubNames : null;
		}
	}


	public class TCKeywordPropertyGroup : TCPropertyGroup
	{
		public Dictionary<string, string[]> SubNames;

		public override string[] GetEnabledSubNames (Material material)
		{
			if (SubNames == null) {
				return null;
			}

			foreach (var subNames in SubNames) {
				if (material.IsKeywordEnabled (subNames.Key)) {
					return subNames.Value;
				}
			}

			return null;
		}
	}
}