using System.Collections.Generic;
using UnityEngine;

namespace LD43 {
	[RequireComponent(typeof(PlatformEffector2D))]
	public class DropThroughPlatform : MonoBehaviour {

		private struct DropThroughAllowedCollider {
			public Collider2D collider;
			public float duration;
			public float startTime;

			public DropThroughAllowedCollider(Collider2D collider, float duration, float startTime) {
				this.collider = collider;
				this.duration = duration;
				this.startTime = startTime;
			}
		}

		private Collider2D col;
		private PlatformEffector2D platform;

		private List<DropThroughAllowedCollider> droppingColliders = new List<DropThroughAllowedCollider>();

		private void Awake() {
			platform = GetComponent<PlatformEffector2D>();
			col = GetComponent<Collider2D>();
		}

		private void Update() {
			for (int i = 0; i < droppingColliders.Count; i++) {
				if (droppingColliders[i].startTime + droppingColliders[i].duration < Time.time) {
					Physics2D.IgnoreCollision(droppingColliders[i].collider, col, false);
					droppingColliders.Remove(droppingColliders[i]);
					i--;
				}
			}
		}

		public void AllowDropThrough(Collider2D toAllow, float duration) {
			Physics2D.IgnoreCollision(toAllow, col);
			droppingColliders.Add(new DropThroughAllowedCollider(toAllow, duration, Time.time));
		}

	}
}
