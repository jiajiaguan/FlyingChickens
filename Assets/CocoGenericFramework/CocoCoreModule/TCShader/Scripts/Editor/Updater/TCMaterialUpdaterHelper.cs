using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TC.Shader.MaterialUpdater
{
	public class UpdaterHelper
	{
		#region Property Group

		// light
		public static readonly PropertyGroupConfigData SpecularPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_SPECULAR,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_SPECULAR_ON, 1 }
			}
		};

		public static readonly PropertyGroupConfigData ReflectionPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_REFLECTION,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_REFLECTION_ON, 1 }
			}
		};

		public static readonly PropertyGroupConfigData RimWrapPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_RIM_WRAP,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_RIM_WRAP_ON, 1 }
			}
		};

		// cubemap
		public static readonly PropertyGroupConfigData NormalmapPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_NORMALMAP,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_NORMALMAP_ON, 1 }
			}
		};

		public static readonly PropertyGroupConfigData ToonmapPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_TOONMAP,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_TOONMAP_ON, 1 }
			}
		};

		// discolor
		public static readonly PropertyGroupConfigData DiscolorPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_DISCOLOR,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_DISCOLOR_HSL_BLEND, 1 },
				{ TCShaderUtil.KEYWORD_DISCOLOR_HUE_REPLACE, 2 }
			}
		};

		// bottom layer
		public static readonly PropertyGroupConfigData BottomLayerPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_BOTTOM_LAYER,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_BOTTOM_LAYER_ON, 1 }
			}
		};

		// decal layer
		public static readonly PropertyGroupConfigData DecalLayerPropertyGroupConfigData = new PropertyGroupConfigData {
			PropertyName = TCShaderUtil.PROPERTY_DECAL_LAYER,
			KeywordPropertyValues = new Dictionary<string, int> {
				{ TCShaderUtil.KEYWORD_DECAL_LAYER_1, 1 },
				{ TCShaderUtil.KEYWORD_DECAL_LAYER_2, 2 },
				{ TCShaderUtil.KEYWORD_DECAL_LAYER_3, 3 }
			}
		};

		public static readonly List<DecalLayerTextureConfigData> DecalLayerTextureConfigDatas = new List<DecalLayerTextureConfigData> {
			new DecalLayerTextureConfigData {
				TexturePropertyName = TCShaderUtil.PROPERTY_DECAL_TEX1,
				UVPropertyName = TCShaderUtil.PROPERTY_DECAL_TEX1_UV2,
				UVKeyword = TCShaderUtil.KEYWORD_DECAL_LAYER_1_UV2_ON
			},
			new DecalLayerTextureConfigData {
				TexturePropertyName = TCShaderUtil.PROPERTY_DECAL_TEX2,
				UVPropertyName = TCShaderUtil.PROPERTY_DECAL_TEX2_UV2,
				UVKeyword = TCShaderUtil.KEYWORD_DECAL_LAYER_2_UV2_ON
			},
			new DecalLayerTextureConfigData {
				TexturePropertyName = TCShaderUtil.PROPERTY_DECAL_TEX3,
				UVPropertyName = TCShaderUtil.PROPERTY_DECAL_TEX3_UV2,
				UVKeyword = TCShaderUtil.KEYWORD_DECAL_LAYER_3_UV2_ON
			}
		};

		public static bool EnableMaterialPropertyGroup (Material material, PropertyGroupConfigData propertyGroupConfigData, int value, bool shouldEnabled)
		{
			var keywordValues = propertyGroupConfigData.KeywordPropertyValues;

			var enabledKeyword = string.Empty;
			foreach (var keywordValue in keywordValues) {
				if (shouldEnabled && keywordValue.Value == value) {
					enabledKeyword = keywordValue.Key;
				} else {
					material.DisableKeyword (keywordValue.Key);
				}
			}

			if (string.IsNullOrEmpty (enabledKeyword)) {
				material.SetFloat (propertyGroupConfigData.PropertyName, 0);
				return false;
			}

			material.SetFloat (propertyGroupConfigData.PropertyName, value);
			material.EnableKeyword (enabledKeyword);
			return true;
		}

		#endregion


		#region Path

		public static string GetAssetFullPath (string path)
		{
			var projectPath = Path.GetDirectoryName (Application.dataPath);
			if (string.IsNullOrEmpty (projectPath)) {
				return string.Empty;
			}

			return path.StartsWith (projectPath) ? path : Path.Combine (projectPath, path);
		}

		public static string GetAssetRelativePath (string path)
		{
			var projectPath = Path.GetDirectoryName (Application.dataPath);
			if (string.IsNullOrEmpty (projectPath)) {
				return string.Empty;
			}

			if (!path.StartsWith (projectPath)) {
				return path;
			}

			var length = projectPath.Length;
			return path.Length > length ? path.Substring (length + 1) : string.Empty;
		}

		public static void UpdateAssetPath (out string relativePath, out string fullPath, string newPath)
		{
			relativePath = GetAssetRelativePath (newPath);
			fullPath = GetAssetFullPath (newPath);
		}

		#endregion


		#region Data

		public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue> (IList<TValue> values, Func<TValue, TKey> keyFunc)
		{
			if (values == null) {
				return null;
			}

			var dictionary = new Dictionary<TKey, TValue> (values.Count);
			foreach (var value in values) {
				var key = keyFunc (value);
				if (dictionary.ContainsKey (key)) {
					continue;
				}
				dictionary.Add (key, value);
			}

			return dictionary;
		}

		public static T LoadFromJsonFile<T> (string fullPath)
		{
			if (!File.Exists (fullPath)) {
				return default(T);
			}

			var json = File.ReadAllText (fullPath);
			return JsonUtility.FromJson<T> (json);
		}

		public static List<T> CollectAllAssets<T> (string path, bool searchRecursively, params string[] searchPatterns) where T : UnityEngine.Object
		{
			var list = new List<T> ();
			var files = CollectAllFiles (path, searchRecursively, searchPatterns);

			foreach (var file in files) {
				var assetPath = GetAssetRelativePath (file);
				var asset = AssetDatabase.LoadAssetAtPath<T> (assetPath);
				if (asset != null) {
					list.Add (asset);
				}
			}

			return list;
		}

		public static List<string> CollectAllFiles (string path, bool searchRecursively, params string[] searchPatterns)
		{
			if (!Directory.Exists (path)) {
				return new List<string> ();
			}

			var files = new HashSet<string> ();

			if (searchPatterns.Length <= 0) {
				searchPatterns = new[] { "*" };
			}
			var searchOption = searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			foreach (var pattern in searchPatterns) {
				var patternFiles = Directory.GetFiles (path, pattern, searchOption);
				foreach (var file in patternFiles) {
					if (file.Contains (".DS_Store") || file.Contains (".meta")) {
						continue;
					}

					files.Add (file);
				}
			}

			return new List<string> (files);
		}

		#endregion
	}
}