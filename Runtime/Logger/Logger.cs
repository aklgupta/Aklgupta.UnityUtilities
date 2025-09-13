using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Aklgupta.Utils.Logger {
	public static class Logger {

		public static bool PrefixObjectName { get; set; } = true;
		public static bool PrefixSourceType { get; set; } = true;

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		public static void LogStatic(object message) {
			Debug.Log($"{GetPrefix()}{message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		public static void LogWarningStatic(object message) {
			Debug.LogWarning($"{GetPrefix()}{message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[Conditional("DEBUG_LOG_ERROR")]
		public static void LogErrorStatic(object message) {
			Debug.LogError($"{GetPrefix()}{message}");
		}


		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		public static void Log(this object source, object message) {
			Debug.Log($"{GetPrefix(source)}{message}", source as Object);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		public static void LogWarning(this object source, object message) {
			Debug.LogWarning($"{GetPrefix(source)}{message}", source as Object);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[Conditional("DEBUG_LOG_ERROR")]
		public static void LogError(this object source, object message) {
			Debug.LogError($"{GetPrefix(source)}{message}", source as Object);
		}


		private static string GetPrefix() {
			var prefixes = new List<string>();

			if (PrefixObjectName || PrefixSourceType)
				prefixes.Add("<i>null</i>");


			return prefixes.Count > 0 ? $"{string.Join(" ", prefixes.Select(x => $"[{x}]"))} : " : null;
		}

		private static string GetPrefix(object source) {
			var prefixes = new List<string>();

			if (PrefixObjectName && source is Object o)
				prefixes.Add(o.name);

			if (PrefixSourceType)
				prefixes.Add(source.GetType().Name);

			return prefixes.Count > 0 ? $"{string.Join(" ", prefixes.Select(x => $"[{x}]"))} : " : null;
		}

	}
}