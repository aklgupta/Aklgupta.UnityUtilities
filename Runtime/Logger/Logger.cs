using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace Aklgupta.Utils.Logger {

	public enum LogLevel {
		Log,
		Warning,
		Error,
	}

	public class LogEntry {

		public LogLevel Level { get; }
		public string Message { get; }
		public DateTime Timestamp { get; }
		public float TimeElapsed { get; }
		public int FrameCount { get; }
		public object ContextObject { get; }
		public string ObjectName { get; }
		public string SceneName { get; }

		// The below are intentionally omitted since they are present in the call stack, and use of attributes like
		// [CallerFilePath] & [CallerMemberName] can leak information about the code to the user
		// public string FileName { get; }
		// public int LineNumber { get; }
		// public string MethodName { get; }


		public LogEntry(LogLevel level, string message, object contextObject) {
			Level = level;
			Message = message;
			ContextObject = contextObject;

			Timestamp = DateTime.Now;
			TimeElapsed = Time.time;
			FrameCount = Time.frameCount;

			if (contextObject is Object o)
				ObjectName = o.name;
			else if (contextObject != null)
				ObjectName = $"`object:{contextObject}`";

			if (contextObject is GameObject go)
				SceneName = go.scene.name;
			else if (contextObject is Component cc)
				SceneName = cc.gameObject.scene.name;
		}
	}

	public static class Logger {

		public static event Action<LogEntry> OnLog;

		// TODO: Replace with a better way to confirm the logger, and maybe also change the config temporarily
		// Maybe use a list of prefix/suffix method list
		public static bool PrefixRealTimestamp { get; set; } = true;
		public static bool PrefixObjectName { get; set; } = true;
		public static bool PrefixSourceType { get; set; } = true;
		public static bool PrefixLogTime { get; set; } = false;
		public static bool PrefixFrameCount { get; set; } = false;
		public static bool PrefixSceneName { get; set; } = false;

		public static string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
		public static string TimeElapsedFormat { get; set; } = "00000.000000s";

		#region Log Methods

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[HideInCallstack]
		public static void Log(object message) {
			var log = new LogEntry(LogLevel.Log, message.ToString(), null);
			Debug.Log(FormatMessage(log));
			OnLog?.Invoke(log);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[HideInCallstack]
		public static void LogWarning(object message) {
			var log = new LogEntry(LogLevel.Warning, message.ToString(), null);
			Debug.LogWarning(FormatMessage(log));
			OnLog?.Invoke(log);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[Conditional("DEBUG_LOG_ERROR")]
		[HideInCallstack]
		public static void LogError(object message) {
			var log = new LogEntry(LogLevel.Error, message.ToString(), null);
			Debug.LogError(FormatMessage(log));
			OnLog?.Invoke(log);
		}


		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[HideInCallstack]
		public static void Log(this object source, object message) {
			var log = new LogEntry(LogLevel.Log, message.ToString(), source);
			Debug.Log(FormatMessage(log), source as Object);
			OnLog?.Invoke(log);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[HideInCallstack]
		public static void LogWarning(this object source, object message) {
			var log = new LogEntry(LogLevel.Warning, message.ToString(), source);
			Debug.LogWarning(FormatMessage(log), source as Object);
			OnLog?.Invoke(log);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[Conditional("DEBUG_LOG_ERROR")]
		[HideInCallstack]
		public static void LogError(this object source, object message) {
			var log = new LogEntry(LogLevel.Error, message.ToString(), source);
			Debug.LogError(FormatMessage(log), source as Object);
			OnLog?.Invoke(log);
		}

		#endregion

		private static string FormatMessage(LogEntry entry) {
			var tags = new StringBuilder();

			if (PrefixRealTimestamp)
				AddTag(tags, entry.Timestamp.ToString(TimestampFormat));

			if (PrefixLogTime)
				AddTag(tags, entry.TimeElapsed.ToString(TimeElapsedFormat));

			if (PrefixFrameCount)
				AddTag(tags, entry.FrameCount);

			if (PrefixSceneName)
				AddTag(tags, entry.SceneName);

			if (PrefixObjectName)
				AddTag(tags, entry.ObjectName ?? "<i>null</i>");

			if (PrefixSourceType)
				AddTag(tags, entry.ContextObject?.GetType().Name);

			return (tags.Length > 0 ? $"{tags} : " : null) + entry.Message;
		}

		private static void AddTag(StringBuilder sb, object tag) {
			if (sb.Length > 0)
				sb.Append(' ');
			sb.Append("[").Append(tag).Append("]");
		}

	}
}