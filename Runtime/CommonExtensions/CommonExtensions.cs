using System;
using System.Collections.Generic;

namespace Aklgupta.Utils.CommonExtensions {
	public static class CommonExtensions {

		private static readonly Random random = new();

		public static T Random<T>(this IList<T> list) {
			if (list == null)
				throw new ArgumentNullException(nameof(list), "Can't get random element from null");

			if (list.Count == 0)
				throw new ArgumentException("List has 0 elements. Can't return a random value");

			return list[random.Next(list.Count)];
		}

		public static T RandomOrDefault<T>(this IList<T> list) {
			if (list == null || list.Count == 0)
				return default;

			return list[random.Next(list.Count)];
		}
		
	}
}