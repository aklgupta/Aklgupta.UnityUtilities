using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Aklgupta.Utils.Logger {
	public static class Logger {

		public static bool prefixSourceType = true;

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

		private static string GetPrefix(object source) {
			var prefixes = new List<string>();

			if (prefixSourceType)
				prefixes.Add(source.GetType().Name);


			return prefixes.Count > 0 ? $"{string.Join(",", prefixes.Select(x => $"[{x}]"))} : " : "";
		}

	}
}