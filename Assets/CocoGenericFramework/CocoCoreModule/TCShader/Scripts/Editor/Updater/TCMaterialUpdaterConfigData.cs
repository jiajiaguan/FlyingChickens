using System;
using System.Collections.Generic;

namespace TC.Shader.MaterialUpdater
{
	public enum OriginalDecalLayerType
	{
		None = 0,
		Layer1 = 1,
		Layer2 = 2,
		Layer3 = 3,
		Layer3UV2 = 4
	}


	[Serializable]
	public class UpdaterConfigData
	{
		public List<ShaderConfigData> ShaderConfigDatas = new List<ShaderConfigData> ();

		private Dictionary<string, ShaderConfigData> _shaderConfigDataDic;

		private Dictionary<string, ShaderConfigData> ShaderConfigDataDic {
			get {
				return _shaderConfigDataDic ??
				       (_shaderConfigDataDic = UpdaterHelper.CreateDictionary (ShaderConfigDatas, data => data.OriginShaderName));
			}
		}

		public ShaderConfigData GetShaderConfigData (string shaderName)
		{
			return ShaderConfigDataDic.ContainsKey (shaderName) ? ShaderConfigDataDic [shaderName] : null;
		}
	}


	[Serializable]
	public class ShaderConfigData
	{
		public string OriginShaderName = string.Empty;
		public string NewShaderName = string.Empty;

		public TCShaderRenderingMode RenderingMode = TCShaderRenderingMode.Opaque;
		public int RenderQueueAdd;

		public bool IsSpecularEnabled;
		public bool IsReflectionEnabled;
		public bool IsRimWrapEnabled;

		public TCShaderDiscolorMode DiscolorMode = TCShaderDiscolorMode.None;

		public bool IsNormalmapEnabled;
		public bool IsToonmapEnabled;

		public bool IsBottomLayerEnabled;

		public OriginalDecalLayerType DecalLayerType = OriginalDecalLayerType.None;

		public bool NeedCheck;
	}


	public class PropertyGroupConfigData
	{
		public string PropertyName = string.Empty;
		public Dictionary<string, int> KeywordPropertyValues = new Dictionary<string, int> ();
	}

	public class DecalLayerTextureConfigData
	{
		public string TexturePropertyName;
		public string UVPropertyName;
		public string UVKeyword;
	}


}