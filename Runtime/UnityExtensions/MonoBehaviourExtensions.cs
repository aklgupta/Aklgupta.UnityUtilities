using UnityEngine;

namespace Aklgupta.Utils.UnityExtensions {
	public static class MonoBehaviourExtensions {

		public static bool TryAddComponent<T>(this MonoBehaviour behaviour, out T component) where T : Component {
			if (behaviour.TryGetComponent(out component))
				return false;

			component = behaviour.gameObject.AddComponent<T>();
			return true;
		}

	}
}