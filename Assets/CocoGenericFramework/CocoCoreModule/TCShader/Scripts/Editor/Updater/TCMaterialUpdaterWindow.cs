using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TC.Shader.MaterialUpdater
{
	public class TCMaterialUpdaterWindow : EditorWindow
	{
		#region Window

		[MenuItem ("CocoPlay/Shader/Run Material Updater...", false, 50)]
		private static void Init ()
		{
			if (EditorApplication.isPlaying) {
				EditorUtility.DisplayDialog ("Play Mode", "The game is running -_-", "OK");
				return;
			}

			var window = GetWindow<TCMaterialUpdaterWindow> ("Material Updater", true);
			window.minSize = new Vector2 (600, 400);

			window._isConfigChanged = true;
		}

		private void OnGUI ()
		{
			BeginWindows ();

			OnGUIConfig ();
			OnGUIMaterial ();
			OnGUILogMessage ();

			EndWindows ();
		}

		#endregion


		#region Shader Config

		private const string CONFIG_DEFAULT_ASSET_PATH = "Assets/CocoGenericFramework/CocoCoreModule/TCShader/Configs/material_updater_config.json";

		private UpdaterConfigData _configData;
		private static string _configDataAssetPath;
		private static string _configDataFullPath;
		private bool _isConfigChanged;

		private void OnGUIConfig ()
		{
			if (string.IsNullOrEmpty (_configDataAssetPath)) {
				UpdaterHelper.UpdateAssetPath (out _configDataAssetPath, out _configDataFullPath, CONFIG_DEFAULT_ASSET_PATH);
				_isConfigChanged = true;
			}

			var originalColor = GUI.contentColor;
			GUI.contentColor = Color.green;
			if (GUILayout.Button ("Select Config Path...")) {
				_configDataFullPath = EditorUtility.OpenFilePanel ("Config File Path", _configDataFullPath, "json");
				_configDataAssetPath = UpdaterHelper.GetAssetRelativePath (_configDataFullPath);
				_isConfigChanged = true;
			}
			GUI.contentColor = originalColor;

			EditorGUILayout.LabelField ("Config File Path: ", _configDataAssetPath);

			LoadConfig ();
		}

		private void LoadConfig ()
		{
			if (!_isConfigChanged) {
				return;
			}

			_isConfigChanged = false;

			if (!File.Exists (_configDataFullPath)) {
				return;
			}

			_configData = UpdaterHelper.LoadFromJsonFile<UpdaterConfigData> (_configDataFullPath);
		}

		#endregion


		#region Material Info

		private static string _materialAssetRootDirectory;
		private static string _materialFullRootDirectory;

		private void OnGUIMaterial ()
		{
			if (string.IsNullOrEmpty (_materialAssetRootDirectory)) {
				UpdaterHelper.UpdateAssetPath (out _materialAssetRootDirectory, out _materialFullRootDirectory, "Assets");
			}

			var originalColor = GUI.contentColor;
			GUI.contentColor = Color.yellow;
			if (GUILayout.Button ("Select Root Directory...")) {
				var materialPaths = OnGUISelectMaterialPaths ();

				if (materialPaths != null && materialPaths.Count > 0) {
					ResetLogMessage ();
					UpdateMaterials (materialPaths);
					GenerateLogMessage ();
				}
			}
			GUI.contentColor = originalColor;

			EditorGUILayout.LabelField ("Material Root Directory:", _materialAssetRootDirectory);
		}

		private void UpdateMaterials (List<string> materialPaths)
		{
			foreach (var materialPath in materialPaths) {
				UpdateMaterial (materialPath);
			}

			AssetDatabase.SaveAssets ();
		}

		private List<string> OnGUISelectMaterialPaths ()
		{
			// select root directory
			_materialFullRootDirectory = EditorUtility.OpenFolderPanel ("Select Material Root Directory", _materialFullRootDirectory, "");
			_materialAssetRootDirectory = UpdaterHelper.GetAssetRelativePath (_materialFullRootDirectory);
			if (string.IsNullOrEmpty (_materialFullRootDirectory)) {
				return null;
			}

			// collect material paths
			var materialPaths = UpdaterHelper.CollectAllFiles (_materialFullRootDirectory, true, "*.mat", "*.MAT");

			// show confirm dialog
			var message = string.Format ("There are about [({0})] materials will be updated in directory [\"{1}\"]. this process can NOT be undo.",
				materialPaths.Count, _materialAssetRootDirectory);
			if (!EditorUtility.DisplayDialog ("Warning !!", message, "Cancel", "Still Continue")) {
				return materialPaths;
			}

			Debug.LogWarning ("Updating Cancelled !!");
			return null;
		}

		private void UpdateMaterial (string materialPath)
		{
			var materialAssetPath = UpdaterHelper.GetAssetRelativePath (materialPath);
			var material = AssetDatabase.LoadAssetAtPath<Material> (materialAssetPath);
			if (material == null) {
				return;
			}

			_logger.LogStart (materialAssetPath);
			_logger.LogOriginShader (material.shader.name);

			var shaderConfigData = _configData.GetShaderConfigData (material.shader.name);
			if (shaderConfigData == null) {
				_logger.LogEnd (UpdaterLogType.Skip);
				return;
			}

			UpdateMaterialContent (material, shaderConfigData);

			_logger.LogEnd (shaderConfigData.NeedCheck ? UpdaterLogType.NeedCheck : UpdaterLogType.Done);
		}

		#endregion


		#region Material Update

		private static bool _shouldUpdatePropertyFactor;

		private static void UpdateMaterialContent (Material material, ShaderConfigData shaderConfigData)
		{
			var shader = UnityEngine.Shader.Find (shaderConfigData.NewShaderName);
			if (shader == null) {
				return;
			}

			_logger.LogNewShader (shaderConfigData.NewShaderName);

			var originalMaterial = new Material (material);

			material.shader = shader;
			_shouldUpdatePropertyFactor = false;

			UpdateMaterialRendering (material, shaderConfigData);
			UpdateMaterialBase (material, originalMaterial);

			UpdateMaterialSpecular (material, shaderConfigData, originalMaterial);
			UpdateMaterialReflection (material, shaderConfigData, originalMaterial);
			UpdateMaterialRimWrap (material, shaderConfigData, originalMaterial);

			UpdateMaterialDiscolor (material, shaderConfigData, originalMaterial);

			UpdateMaterialNormalmap (material, shaderConfigData, originalMaterial);
			UpdateMaterialToonmap (material, shaderConfigData, originalMaterial);

			UpdateMaterialBottomLayer (material, shaderConfigData, originalMaterial);
			UpdateMaterialDecalLayer (material, shaderConfigData, originalMaterial);

			UpdateMaterialPropertyFactor (material, originalMaterial);

			DestroyImmediate (originalMaterial);
		}

		private static void UpdateMaterialRendering (Material material, ShaderConfigData shaderConfigData)
		{
			material.SetFloat (TCShaderUtil.PROPERTY_RENDING_MODE, (float)shaderConfigData.RenderingMode);
			TCShaderUtil.SetupMaterialWithRenderingMode (material, shaderConfigData.RenderingMode, true);
			if (shaderConfigData.RenderQueueAdd != 0) {
				material.renderQueue += shaderConfigData.RenderQueueAdd;
			}

			_logger.LogRenderingMode (shaderConfigData.RenderingMode, shaderConfigData.RenderQueueAdd);
		}

		private static void UpdateMaterialBase (Material material, Material originalMaterial)
		{
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialColor, TCShaderUtil.PROPERTY_MAIN_COLOR);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_MAIN_TEX);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_COLOR_FACTOR, "_BaseColorFactor");
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_CUTOFF);
		}

		private static void UpdateMaterialSpecular (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.SpecularPropertyGroupConfigData, 1, shaderConfigData.IsSpecularEnabled)) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialColor, TCShaderUtil.PROPERTY_SPEC_COLOR);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_SHININESS);

			_logger.LogFeature (UpdaterLogFeature.Specular);
		}

		private static void UpdateMaterialReflection (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.ReflectionPropertyGroupConfigData, 1, shaderConfigData.IsReflectionEnabled)) {
				return;
			}


			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialColor, TCShaderUtil.PROPERTY_REFLECT_COLOR);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_REFLECT_CUBE, "_Cube");

			_shouldUpdatePropertyFactor = true;

			_logger.LogFeature (UpdaterLogFeature.Reflection);
		}

		private static void UpdateMaterialRimWrap (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.RimWrapPropertyGroupConfigData, 1, shaderConfigData.IsRimWrapEnabled)) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialColor, TCShaderUtil.PROPERTY_RIM_COLOR);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_RIM_POWER);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_WRAP_POWER);
			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_LIGHT_POWER);

			_logger.LogFeature (UpdaterLogFeature.RimWrap);
		}

		private static void UpdateMaterialDiscolor (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			var shouldEnabled = true;
			var originalHuePropertyName = "_Hue";

			// check hue alpha
			if (shaderConfigData.DiscolorMode == TCShaderDiscolorMode.HueReplace) {
				var originalHuePropertyId = UnityEngine.Shader.PropertyToID (originalHuePropertyName);
				if (originalMaterial.HasProperty (originalHuePropertyId)) {
					var hueColor = originalMaterial.GetColor (originalHuePropertyId);
					shouldEnabled = hueColor.a > 0.01f;
				}
			}

			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.DiscolorPropertyGroupConfigData, (int)shaderConfigData.DiscolorMode, shouldEnabled)) {
				return;
			}

			switch (shaderConfigData.DiscolorMode) {
			case TCShaderDiscolorMode.HslBlend:
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_HUE);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_SATURATION);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_LIGHTNESS);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_MIXING_FACTOR);

				_logger.LogFeature (UpdaterLogFeature.HslBlend);
				break;
			case TCShaderDiscolorMode.HueReplace:
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialColor, TCShaderUtil.PROPERTY_HUE_COLOR, originalHuePropertyName);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_SATUR_MIN);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_SATUR_RATIO);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_SATUR_ADD);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_LIGHT_MAX);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_LIGHT_RATIO);
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialFloat, TCShaderUtil.PROPERTY_LIGHT_ADD);

				_logger.LogFeature (UpdaterLogFeature.HueReplace);
				break;
			}
		}

		private static void UpdateMaterialNormalmap (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			var textureExists = ExistsMaterialTexture (material, TCShaderUtil.PROPERTY_NORMAL_BUMP_MAP);
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.NormalmapPropertyGroupConfigData, 1,
				shaderConfigData.IsNormalmapEnabled && textureExists)) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_NORMAL_BUMP_MAP);

			_logger.LogFeature (UpdaterLogFeature.Normalmap);
		}

		private static void UpdateMaterialToonmap (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			var textureExists = ExistsMaterialTexture (material, TCShaderUtil.PROPERTY_TOONMAP_CUBE);
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.ToonmapPropertyGroupConfigData, 1, shaderConfigData.IsToonmapEnabled && textureExists)) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_TOONMAP_CUBE, "_ToonShade");

			_logger.LogFeature (UpdaterLogFeature.Toonmap);
		}

		private static void UpdateMaterialBottomLayer (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			var textureExists = ExistsMaterialTexture (material, TCShaderUtil.PROPERTY_BOTTOM_TEX);
			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.BottomLayerPropertyGroupConfigData, 1,
				shaderConfigData.IsBottomLayerEnabled && textureExists)) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_BOTTOM_TEX);

			_shouldUpdatePropertyFactor = true;

			_logger.LogFeature (UpdaterLogFeature.BottomLayer);
		}

		private static Dictionary<string, bool> CollectOriginalMaterialDecalLayers (Material originalMaterial, OriginalDecalLayerType layerType)
		{
			var decalLayers = new Dictionary<string, bool> ();

			if (layerType >= OriginalDecalLayerType.Layer1 && ExistsMaterialTexture (originalMaterial, "_LayerTex1")) {
				decalLayers.Add ("_LayerTex1", false);
			}

			if (layerType >= OriginalDecalLayerType.Layer2 && ExistsMaterialTexture (originalMaterial, "_LayerTex2")) {
				decalLayers.Add ("_LayerTex2", layerType == OriginalDecalLayerType.Layer3UV2);
			}

			if (layerType >= OriginalDecalLayerType.Layer3 && ExistsMaterialTexture (originalMaterial, "_LayerTex3")) {
				decalLayers.Add ("_LayerTex3", false);
			}

			return decalLayers;
		}


		private static void UpdateMaterialDecalLayer (Material material, ShaderConfigData shaderConfigData, Material originalMaterial)
		{
			var decalLayers = CollectOriginalMaterialDecalLayers (originalMaterial, shaderConfigData.DecalLayerType);

			if (!UpdaterHelper.EnableMaterialPropertyGroup (material, UpdaterHelper.DecalLayerPropertyGroupConfigData, decalLayers.Count, true)) {
				return;
			}

			bool useUv2 = false;
			var index = 0;
			var decalTextureConfigDatas = UpdaterHelper.DecalLayerTextureConfigDatas;
			foreach (var decalLayer in decalLayers) {
				if (index >= decalTextureConfigDatas.Count) {
					break;
				}

				var decalTextureConfigData = decalTextureConfigDatas [index];
				UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, decalTextureConfigData.TexturePropertyName, decalLayer.Key);
				if (decalLayer.Value) {
					material.SetFloat (decalTextureConfigData.UVPropertyName, 1);
					material.EnableKeyword (decalTextureConfigData.UVKeyword);
					useUv2 = true;
				} else {
					material.SetFloat (decalTextureConfigData.UVPropertyName, 0);
					material.DisableKeyword (decalTextureConfigData.UVKeyword);
				}

				index++;
			}


			if (index == 1) {
				_logger.LogFeature (UpdaterLogFeature.DecalLayer1);
			} else if (index == 2) {
				_logger.LogFeature (UpdaterLogFeature.DecalLayer2);
			} else if (index > 2) {
				_logger.LogFeature (UpdaterLogFeature.DecalLayer3);
			}
			if (useUv2) {
				_logger.LogFeature (UpdaterLogFeature.DecalLayerUv2);
			}
		}

		private static void UpdateMaterialPropertyFactor (Material material, Material originalMaterial)
		{
			if (!_shouldUpdatePropertyFactor) {
				return;
			}

			UpdateMaterialProperty (material, originalMaterial, UpdateMaterialTexture, TCShaderUtil.PROPERTY_FACTOR_TEX, "_PropertyTex");
			_shouldUpdatePropertyFactor = false;
		}

		#endregion


		#region Update Property Function

		private static void UpdateMaterialProperty (Material material, Material originalMaterial, Action<Material, Material, int, int> updateAction,
			string property, string originalProperty = null
		)
		{
			var propertyId = UnityEngine.Shader.PropertyToID (property);
			if (!material.HasProperty (propertyId)) {
				return;
			}

			var originalPropertyId = string.IsNullOrEmpty (originalProperty) ? propertyId : UnityEngine.Shader.PropertyToID (originalProperty);
			if (!originalMaterial.HasProperty (originalPropertyId)) {
				return;
			}

			updateAction (material, originalMaterial, propertyId, originalPropertyId);
		}

		private static void UpdateMaterialColor (Material material, Material originalMaterial, int propertyId, int originalPropertyId)
		{
			var color = originalMaterial.GetColor (originalPropertyId);
			material.SetColor (propertyId, color);
		}

		private static void UpdateMaterialFloat (Material material, Material originalMaterial, int propertyId, int originalPropertyId)
		{
			var value = originalMaterial.GetFloat (originalPropertyId);
			material.SetFloat (propertyId, value);
		}

		private static void UpdateMaterialTexture (Material material, Material originalMaterial, int propertyId, int originalPropertyId)
		{
			var texture = originalMaterial.GetTexture (originalPropertyId);
			material.SetTexture (propertyId, texture);

			var offset = originalMaterial.GetTextureOffset (originalPropertyId);
			material.SetTextureOffset (propertyId, offset);

			var scale = originalMaterial.GetTextureScale (originalPropertyId);
			material.SetTextureScale (propertyId, scale);
		}

		private static bool ExistsMaterialTexture (Material material, string property)
		{
			var propertyId = UnityEngine.Shader.PropertyToID (property);
			if (!material.HasProperty (propertyId)) {
				return false;
			}

			return material.GetTexture (propertyId) != null;
		}

		#endregion


		#region Update Log

		private static readonly UpdaterLogger _logger = new UpdaterLogger ();
		private UpdaterLogger.Output _loggerOutput;
		private Vector2 _scrollPosition;

		private void OnGUILogMessage ()
		{
			if (_loggerOutput == null) {
				return;
			}

			_scrollPosition = EditorGUILayout.BeginScrollView (_scrollPosition, GUI.skin.box);

			OnGUILogBlock (_loggerOutput.NeedCheckedBlock);
			OnGUILogBlock (_loggerOutput.SkippedBlock);
			OnGUILogBlock (_loggerOutput.DoneBlock);

			EditorGUILayout.EndScrollView ();
		}

		private void OnGUILogBlock (UpdaterLogger.Output.Block logBlock)
		{
			var originalEditing = EditorGUIUtility.editingTextField;
			var originalColor = GUI.contentColor;

			string headerLabel;
			Color textColor;

			switch (logBlock.LogType) {
			case UpdaterLogType.Done:
				headerLabel = "Finished :";
				textColor = Color.white;
				break;

			case UpdaterLogType.NeedCheck:
				headerLabel = "Need Checked :";
				textColor = Color.yellow;
				break;
			default:
				headerLabel = "Skipped :";
				textColor = Color.green;
				break;
			}

			// header
			EditorGUILayout.LabelField (headerLabel, string.Format ("count [{0}]", logBlock.LogCount));

			// scroll view
			EditorGUIUtility.editingTextField = false;
			GUI.contentColor = textColor;

			// log area
			foreach (var log in logBlock.Logs) {
				EditorGUILayout.TextArea (log);
			}

			EditorGUILayout.Space ();
			EditorGUIUtility.editingTextField = originalEditing;
			GUI.contentColor = originalColor;
		}

		private void ResetLogMessage ()
		{
			_logger.Reset ();
			_loggerOutput = null;
		}

		private void GenerateLogMessage ()
		{
			_loggerOutput = _logger.GenerateOutput ();
		}

		#endregion
	}
}