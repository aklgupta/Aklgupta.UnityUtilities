using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Aklgupta.Utils.Logger {
	public static class Logger {

		// TODO: Replace with a better way to confirm the logger, and maybe also change the config temporality
		// Maybe use a list of prefix/suffix method list
		public static bool PrefixObjectName { get; set; } = true;

		public static bool PrefixSourceType { get; set; } = true;

		public static bool PrefixLogTime { get; set; } = true;

		#region Log Methods

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		public static void Log(object message) {
			Debug.Log($"{GetPrefix(null)}{message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		public static void LogWarning(object message) {
			Debug.LogWarning($"{GetPrefix(null)}{message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEBUG_LOG")]
		[Conditional("DEBUG_LOG_WARNING")]
		[Conditional("DEBUG_LOG_ERROR")]
		public static void LogError(object message) {
			Debug.LogError($"{GetPrefix(null)}{message}");
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

		#endregion

		private static string GetPrefix(object source) {
			var prefixes = new List<string>();

			if (PrefixObjectName) {
				if (source is Object o)
					prefixes.Add(o.name);
				else
					prefixes.Add("<i>null</i>");
			}

			if (PrefixSourceType)
				prefixes.Add(source != null ? source.GetType().Name : "<i>null</i>");

			if (PrefixLogTime)
				prefixes.Add($"{Time.time}");

			return prefixes.Count > 0 ? $"{string.Join(" ", prefixes.Select(x => $"[{x}]"))} : " : null;
		}

	}
}