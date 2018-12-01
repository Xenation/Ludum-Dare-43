using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD43 {
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharController : MonoBehaviour {

		public float speed = 5f;
		public float pushSpeed = 2f;
		public float jumpForce = 12f;
		public float baseGravity = 2f;
		public float fallGravity = 6f;
		public PhysicsMaterial2D matDead;

		private bool isActive = false;
		private bool isDead = false;
		private bool inAir = true;
		private bool pushing = false;
		private Rigidbody2D rb;
		private Collider2D col;
		private Vector2 velocity = Vector2.zero;
		private Collider2D pushZone;
		private ContactFilter2D pushZoneFilter;
		private Collider2D landZone;
		private ContactFilter2D landZoneFilter;
		private EdgeCollider2D platform;
		private Transform layingPlatformLocation;
		private float height = 0f;
		private float width = 0f;

		private void Awake() {
			rb = GetComponent<Rigidbody2D>();
			col = GetComponent<Collider2D>();
			pushZone = transform.Find("PushZone").GetComponent<Collider2D>();
			pushZoneFilter = new ContactFilter2D();
			pushZoneFilter.SetLayerMask(LayerMask.GetMask("Movable"));
			landZone = transform.Find("LandZone").GetComponent<Collider2D>();
			landZoneFilter = new ContactFilter2D();
			landZoneFilter.SetLayerMask(~LayerMask.GetMask("Character"));
			platform = transform.Find("Platform").GetComponent<EdgeCollider2D>();
			layingPlatformLocation = transform.Find("LayingPlatformLocation");
			height = col.bounds.extents.y * 2f;
			width = col.bounds.extents.x * 2f;
		}

		private void Update() {
			if (isDead) return;
			Collider2D[] colliders = new Collider2D[4];
			int colCount;
			velocity.x = 0f;

			if (isActive) {
				// Input
				velocity.x = Input.GetAxisRaw("Horizontal");
				// Look Side
				if (velocity.x > 0) {
					transform.localScale = new Vector3(1f, 1f, 1f);
				} else if (velocity.x < 0) {
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}

				// Pushable Object Check
				colCount = pushZone.OverlapCollider(pushZoneFilter, colliders);
				if (colCount == 0) { // no pushable
					velocity.x *= speed;
				} else { // pushable
					velocity.x *= pushSpeed;
				}
			}

			// Ground Check
			inAir = true;
			colCount = landZone.OverlapCollider(landZoneFilter, colliders);
			for (int i = 0; i < colCount; i++) {
				if (colliders[i] != col) {
					inAir = false;
				}
			}

			if (isActive) {
				// Jump
				if (Input.GetButtonDown("Jump") && !inAir) {
					rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
				}
			}

			// Gravity
			if (rb.velocity.y < 3f) {
				rb.gravityScale = fallGravity;
			} else {
				rb.gravityScale = baseGravity;
			}
		}

		private void FixedUpdate() {
			if (isDead) return;
			velocity.y = rb.velocity.y;
			rb.velocity = velocity;
		}

		public void Activate() {
			isActive = true;
		}

		public void Desactivate() {
			velocity.x = 0f;
			isActive = false;
		}

		public void LayDead() {
			if (isDead) return;
			CharactersManager.I.RemoveCharacter(this);
			isDead = true;
			if (transform.localScale.x > 0) {
				transform.rotation = Quaternion.Euler(0f, 0f, -90f);
			} else {
				transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			}
			platform.transform.localPosition = Vector3.up * (height / 2f) + Vector3.left * (width / 2f);
			platform.transform.rotation = Quaternion.identity;
			Vector2[] pts = platform.points;
			pts[0].x = -height / 2f;
			pts[1].x = height / 2f;
			platform.points = pts;
			rb.bodyType = RigidbodyType2D.Static;
			rb.sharedMaterial = matDead;
			gameObject.layer = LayerMask.NameToLayer("DeadCharacter");
		}

	}
}
