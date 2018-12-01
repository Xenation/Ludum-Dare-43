using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43 {
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharController : MonoBehaviour {

		public float speed = 5f;
		public float pushSpeed = 2f;
		public float baseGravity = 2f;
		public float fallGravity = 6f;

		private bool isActive = true;
		private bool inAir = true;
		private bool pushing = false;
		private Rigidbody2D rb;
		private Collider2D col;
		private Vector2 velocity = Vector2.zero;
		private Collider2D pushZone;
		private ContactFilter2D pushZoneFilter;
		private Collider2D landZone;
		private ContactFilter2D landZoneFilter;

		private void Awake() {
			rb = GetComponent<Rigidbody2D>();
			col = GetComponent<Collider2D>();
			pushZone = transform.Find("PushZone").GetComponent<Collider2D>();
			pushZoneFilter = new ContactFilter2D();
			pushZoneFilter.SetLayerMask(LayerMask.GetMask("Movable"));
			landZone = transform.Find("LandZone").GetComponent<Collider2D>();
			landZoneFilter = new ContactFilter2D();
		}

		private void Update() {
			if (!isActive) return;

			// Input
			velocity.x = Input.GetAxisRaw("Horizontal");
			// Look Side
			if (velocity.x > 0) {
				transform.localScale = new Vector3(1f, 1f, 1f);
			} else if (velocity.x < 0) {
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}

			// Pushable Object Check
			Collider2D[] colliders = new Collider2D[4];
			int colCount = pushZone.OverlapCollider(pushZoneFilter, colliders);
			if (colCount == 0) { // no pushable
				velocity.x *= speed;
			} else { // pushable
				velocity.x *= pushSpeed;
			}

			// Ground Check
			inAir = true;
			colCount = landZone.OverlapCollider(landZoneFilter, colliders);
			for (int i = 0; i < colCount; i++) {
				if (colliders[i] != col) {
					inAir = false;
				}
			}

			// Jump
			if (Input.GetButtonDown("Jump") && !inAir) {
				rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
			}

			// Gravity
			if (rb.velocity.y < 3f) {
				rb.gravityScale = fallGravity;
			} else {
				rb.gravityScale = baseGravity;
			}
		}

		private void FixedUpdate() {
			velocity.y = rb.velocity.y;
			rb.velocity = velocity;
		}

	}
}
