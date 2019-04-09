using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TC.Shader
{
	public abstract class TCBaseShaderGUI : ShaderGUI
	{
		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			materialEditor.SetDefaultGUIWidths ();

			OnGUIRenderingMode (materialEditor, properties);

			OnGUIBasicProperties (materialEditor, properties);

			foreach (var propertyGroup in PropertyGroups) {
				if (OnGUIPropertyGroup (propertyGroup, materialEditor, properties)) {
					EditorGUILayout.Space ();
				}
			}

			EditorGUILayout.Space ();
			materialEditor.RenderQueueField ();
			materialEditor.EnableInstancingField ();
			materialEditor.DoubleSidedGIField ();
		}

		public override void AssignNewShaderToMaterial (Material material, UnityEngine.Shader oldShader, UnityEngine.Shader newShader)
		{
			base.AssignNewShaderToMaterial (material, oldShader, newShader);

			ResetRenderingMode ();
		}

		private MaterialProperty[] GetProperties (ICollection<string> propertyNames, MaterialProperty[] inProperties)
		{
			var properties = new List<MaterialProperty> (propertyNames.Count);
			foreach (var subName in propertyNames) {
				var property = FindProperty (subName, inProperties, false);
				if (property != null) {
					properties.Add (property);
				}
			}
			return properties.ToArray ();
		}

		private bool OnGUIProperties (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (properties == null) {
				return false;
			}

			var hasContent = false;
			foreach (var property in properties) {
				if (property == null) {
					continue;
				}

				materialEditor.ShaderProperty (property, property.displayName);
				hasContent = true;
			}

			return hasContent;
		}


		#region Rendering Mode

		private static readonly string _renderingModeName = "_Mode";
		private static readonly string _renderingModeLabel = "Rendering Mode";
		private static readonly string[] _renderingModeNames = Enum.GetNames (typeof(TCShaderRenderingMode));

		private bool _isRenderingModeInited;

		protected abstract bool IsRenderingModeEnabled { get; }

		private void OnGUIRenderingMode (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (!IsRenderingModeEnabled) {
				return;
			}

			var renderingMode = FindProperty (_renderingModeName, properties);
			if (renderingMode == null) {
				return;
			}

			if (!_isRenderingModeInited) {
				OnRenderingModeChanged (renderingMode, false);
				_isRenderingModeInited = true;
			}

			EditorGUI.showMixedValue = renderingMode.hasMixedValue;
			var mode = (TCShaderRenderingMode)renderingMode.floatValue;

			EditorGUI.BeginChangeCheck ();
			mode = (TCShaderRenderingMode)EditorGUILayout.Popup (_renderingModeLabel, (int)mode, _renderingModeNames);
			if (EditorGUI.EndChangeCheck ()) {
				materialEditor.RegisterPropertyChangeUndo (_renderingModeLabel);
				renderingMode.floatValue = (float)mode;
				OnRenderingModeChanged (renderingMode, true);
			}

			EditorGUI.showMixedValue = false;

			EditorGUILayout.Space ();
		}

		private void ResetRenderingMode ()
		{
			_isRenderingModeInited = false;
		}

		private static void OnRenderingModeChanged (MaterialProperty renderingMode, bool resetQueue)
		{
			foreach (var target in renderingMode.targets) {
				var targetMaterial = target as Material;
				if (targetMaterial == null || !targetMaterial.HasProperty (_renderingModeName)) {
					continue;
				}

				var mode = (TCShaderRenderingMode)targetMaterial.GetFloat (_renderingModeName);
				TCShaderUtil.SetupMaterialWithRenderingMode ((Material)target, mode, resetQueue);
			}
		}

		#endregion


		#region Property Basic

		protected abstract List<string> BasicPropertyNames { get; }

		private void OnGUIBasicProperties (MaterialEditor materialEditor, MaterialProperty[] inProperties)
		{
			var basicProperties = GetProperties (BasicPropertyNames, inProperties);
			if (OnGUIProperties (materialEditor, basicProperties)) {
				EditorGUILayout.Space ();
			}
		}

		#endregion


		#region Property Group

		protected abstract List<TCPropertyGroup> PropertyGroups { get; }

		private bool OnGUIPropertyGroup (TCPropertyGroup group, MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			var targetMaterial = materialEditor.target as Material;
			if (targetMaterial == null) {
				return false;
			}

			var hasContent = false;

			// main property
			if (!string.IsNullOrEmpty (group.MainName) && targetMaterial.HasProperty (group.MainName)) {
				var mainProperty = FindProperty (group.MainName, properties);
				if (mainProperty == null) {
					return false;
				}

				materialEditor.ShaderProperty (mainProperty, mainProperty.displayName);
				hasContent = true;
			}

			// sub properties
			var subNames = group.GetEnabledSubNames (targetMaterial);
			if (subNames == null) {
				return hasContent;
			}

			var subProperties = GetProperties (subNames, properties);
			return OnGUIProperties (materialEditor, subProperties);
		}

		#endregion
	}
}