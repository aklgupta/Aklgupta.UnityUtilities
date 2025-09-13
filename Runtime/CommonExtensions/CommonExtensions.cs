using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Aklgupta.Utils.CommonExtensions {
	public static class CommonExtensions {

		public static T RandomValue<T>(this IList<T> list) {
			if (list == null)
				throw new ArgumentNullException(nameof(list), "Can't get random element from null");

			if (list.Count == 0)
				throw new ArgumentException("List has 0 elements. Can't return a random value");

			return list[Random.Range(0, list.Count)];
		}

		public static T RandomOrDefault<T>(this IList<T> list) {
			if (list == null || list.Count == 0)
				return default;

			return list[Random.Range(0, list.Count)];
		}

		public static float RandomRange(this Vector2 vector2) {
			if (vector2.x < vector2.y)
				return Random.Range(vector2.x, vector2.y);

			Logger.Logger.LogWarningStatic($"x should be less than y, reversing the values ({vector2})");
			return Random.Range(vector2.y, vector2.x);
		}

		public static float RandomRange(this Vector2Int vector2) {
			if (vector2.x < vector2.y)
				return Random.Range(vector2.x, vector2.y);

			Logger.Logger.LogWarningStatic($"x should be less than y, reversing the values ({vector2})");
			return Random.Range(vector2.y, vector2.x);
		}
		
	}
}