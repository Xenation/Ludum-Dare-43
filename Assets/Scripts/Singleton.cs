using UnityEngine;

namespace LD43 {
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		private static T instance;
		public static T I {
			get {
				if (instance == null) {
					instance = FindObjectOfType<T>();
				}
				return instance;
			}
		}

	}
}
