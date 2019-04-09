using System;
using System.Collections.Generic;

namespace TC.Shader.MaterialUpdater
{
	[Flags]
	public enum UpdaterLogFeature
	{
		None = 0x00,

		// light
		Specular = 0x01,
		Reflection = 0x02,
		RimWrap = 0x04,

		// cubemap
		Normalmap = 0x08,
		Toonmap = 0x10,

		// discolor
		HslBlend = 0x20,
		HueReplace = 0x40,

		// bottom layer
		BottomLayer = 0x80,

		// decal layer
		DecalLayer1 = 0x0100,
		DecalLayer2 = 0x0200,
		DecalLayer3 = 0x0400,

		DecalLayerUv2 = 0x0800
	}


	public enum UpdaterLogType
	{
		Done,
		Skip,
		NeedCheck
	}


	public class UpdaterLogger
	{
		private class LogInfo
		{
			public string MaterialPath;
			public string OriginShaderName;
			public string NewShaderName;
			public TCShaderRenderingMode RenderingMode;
			public int RenderingQueueAdd;

			public UpdaterLogFeature Feature = UpdaterLogFeature.None;

			public UpdaterLogType LogType;
		}


		public class Output
		{
			public class Block
			{
				public UpdaterLogType LogType;
				public int LogCount;
				public readonly List<string> Logs = new List<string> ();

				public Block (UpdaterLogType logType)
				{
					LogType = logType;
				}
			}


			public readonly Block DoneBlock = new Block (UpdaterLogType.Done);
			public readonly Block SkippedBlock = new Block (UpdaterLogType.Skip);
			public readonly Block NeedCheckedBlock = new Block (UpdaterLogType.NeedCheck);
		}


		#region Log Info List

		private const int OUTPUT_MAX_LENGTH = 65536 / 4 - 10;

		private readonly List<LogInfo> _logInfos = new List<LogInfo> ();


		public void Reset ()
		{
			_logInfos.Clear ();
		}

		public Output GenerateOutput ()
		{
			var outputMsg = new Output ();

			var doneLog = string.Empty;
			var skippedLog = string.Empty;
			var needCheckedLog = string.Empty;

			foreach (var logInfo in _logInfos) {
				var log = OutputInfo (logInfo);

				switch (logInfo.LogType) {
				case UpdaterLogType.Done:
					AddOutputLog (ref doneLog, outputMsg.DoneBlock, log);
					break;
				case UpdaterLogType.NeedCheck:
					AddOutputLog (ref needCheckedLog, outputMsg.NeedCheckedBlock, log);
					break;
				default:
					AddOutputLog (ref skippedLog, outputMsg.SkippedBlock, log);
					break;
				}
			}

			// add final
			AddOutputLog (ref doneLog, outputMsg.DoneBlock, string.Empty);
			AddOutputLog (ref needCheckedLog, outputMsg.NeedCheckedBlock, string.Empty);
			AddOutputLog (ref skippedLog, outputMsg.SkippedBlock, string.Empty);

			return outputMsg;
		}

		private void AddOutputLog (ref string currLog, Output.Block block, string newLog)
		{
			if (string.IsNullOrEmpty (newLog)) {
				block.Logs.Add (currLog);
				return;
			}

			block.LogCount++;
			if (currLog.Length + newLog.Length < OUTPUT_MAX_LENGTH) {
				currLog += newLog;
				return;
			}

			block.Logs.Add (currLog);
			currLog = newLog;
		}

		private string OutputInfo (LogInfo logInfo)
		{
			switch (logInfo.LogType) {
			case UpdaterLogType.Done:
			case UpdaterLogType.NeedCheck:
				// rendering mode
				var renderingMsg = logInfo.RenderingMode.ToString ();
				if (logInfo.RenderingQueueAdd > 0) {
					renderingMsg = string.Format ("{0}+{1}", renderingMsg, logInfo.RenderingQueueAdd);
				} else if (logInfo.RenderingQueueAdd < 0) {
					renderingMsg = string.Format ("{0}{1}", renderingMsg, logInfo.RenderingQueueAdd);
				}

				return string.Format ("{0}|{1}|{2}|{3}|{4}\n", logInfo.MaterialPath, logInfo.OriginShaderName,
					logInfo.NewShaderName, renderingMsg, logInfo.Feature);

			default: // skipped
				return string.Format ("{0}|{1}\n", logInfo.MaterialPath, logInfo.OriginShaderName);
			}
		}

		#endregion


		#region Curr Log Info

		private LogInfo _currInfo;

		// start / end

		public void LogStart (string materialPath)
		{
			_currInfo = new LogInfo { MaterialPath = materialPath };
		}

		public void LogEnd (UpdaterLogType logType)
		{
			if (_currInfo == null) {
				return;
			}

			_currInfo.LogType = logType;

			_logInfos.Add (_currInfo);
			_currInfo = null;
		}

		// shader

		public void LogOriginShader (string shaderName)
		{
			if (_currInfo == null) {
				return;
			}

			_currInfo.OriginShaderName = shaderName;
		}

		public void LogNewShader (string shaderName)
		{
			if (_currInfo == null) {
				return;
			}

			_currInfo.NewShaderName = shaderName;
		}

		public void LogRenderingMode (TCShaderRenderingMode renderingMode, int renderQueueAdd)
		{
			if (_currInfo == null) {
				return;
			}

			_currInfo.RenderingMode = renderingMode;
			_currInfo.RenderingQueueAdd = renderQueueAdd;
		}

		// feature

		public void LogFeature (UpdaterLogFeature feature)
		{
			if (_currInfo == null) {
				return;
			}

			_currInfo.Feature |= feature;
		}

		#endregion
	}
}