using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xenon;

namespace LD43 {
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharController : MonoBehaviour {

		public float speed = 5f;
		public float pushSpeed = 2f;
		public float jumpHeight = 5f;
		public float jumpDistance = 4f;
		public float jumpAirSpeed = 7f;
		public PhysicsMaterial2D matDead;

        public PlayerTypesFlag PlayerType;

        [SerializeField] private AudioSource m_deadSound;

		[SerializeField] private Gradient outlineAnimGradient;
		[SerializeField] private float outlineAnimDuration = 1f;
		[SerializeField] private Animator animator;
		private int isRunningId;
		private int isPushingId;
		private int isJumpingId;

		private float gravity = 1f;
		private float ascentGravity = 1f;
		private float coastGravity = 1f;
		private float descentGravity = 1f;
		private float jumpVelocity = 0f;
		private float prevVertVel = 0f;

		private bool isActive = false;
		private bool isDead = false;
		private bool inAir = true;
		private Rigidbody2D rb;
		private Collider2D col;
		private Vector2 velocity = Vector2.zero;
		private Collider2D pushZone;
		private ContactFilter2D pushZoneFilter;
		private Collider2D landZone;
		private ContactFilter2D landZoneFilter;
		private EdgeCollider2D platform;
		private float height = 0f;
		private float width = 0f;

		private List<SpriteRenderer> subRenderers;
		private Material material;
		private ProcessManager procManager;

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
			height = col.bounds.extents.y * 2f;
			width = col.bounds.extents.x * 2f;

			rb.gravityScale = 0f;
			float ascentDist = jumpDistance * 0.666f;
			float descentDist = jumpDistance * 0.333f;
			ascentGravity = (-2f * jumpHeight * jumpAirSpeed * jumpAirSpeed) / (ascentDist * ascentDist);
			descentGravity = (-2f * jumpHeight * jumpAirSpeed * jumpAirSpeed) / (descentDist * descentDist);
			coastGravity = descentGravity;
			jumpVelocity = (2f * jumpHeight * jumpAirSpeed) / ascentDist;
			prevVertVel = rb.velocity.y;
			gravity = descentGravity;

			subRenderers = new List<SpriteRenderer>();
			GetComponentsInChildren(subRenderers);
			material = new Material(Shader.Find("Custom/SpriteOutline"));
			material.SetColor("_OutlineColor", Color.clear);
			foreach (SpriteRenderer rend in subRenderers) {
				rend.sharedMaterial = material;
			}
			procManager = new ProcessManager();

			if (animator == null) {
				animator = GetComponent<Animator>();
				if (animator == null) {
					Debug.LogWarning("Character " + gameObject.name + " has no animator");
					animator = gameObject.AddComponent<Animator>();
				}
			}
			isRunningId = Animator.StringToHash("isRunning");
			isPushingId = Animator.StringToHash("isPushing");
			isJumpingId = Animator.StringToHash("isJumping");
		}

		private void Update() {
			procManager.UpdateProcesses(Time.deltaTime);
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
					if (inAir) {
						velocity.x *= jumpAirSpeed;
					} else {
						velocity.x *= speed;
					}
					animator.SetBool(isPushingId, false);
				} else { // pushable
					if (velocity.x != 0f) {
						animator.SetBool(isPushingId, true);
					} else {
						animator.SetBool(isPushingId, false);
					}
					velocity.x *= pushSpeed;
				}
			}

			if (velocity.x != 0f) {
				animator.SetBool(isRunningId, true);
			} else {
				animator.SetBool(isRunningId, false);
			}

			// Ground Check
			inAir = true;
			colCount = landZone.OverlapCollider(landZoneFilter, colliders);
			for (int i = 0; i < colCount; i++) {
				if (colliders[i] != col) {
					inAir = false;
					animator.SetBool(isJumpingId, false);
				}
			}

			if (isActive) {
				// Jump
				if (Input.GetButtonDown("Jump") && !inAir) {
					rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
					gravity = ascentGravity;
					animator.SetBool(isJumpingId, true);
				}
				if (Input.GetButtonUp("Jump") && inAir && rb.velocity.y > 0f) {
					gravity = coastGravity;
				}
			}

			// Gravity
			if (prevVertVel > 0 && rb.velocity.y < 0) {
				gravity = descentGravity;
			}

			prevVertVel = rb.velocity.y;
		}

		private void FixedUpdate() {
			if (isDead) return;
			velocity.y = rb.velocity.y + gravity * Time.fixedDeltaTime;
			rb.velocity = velocity;
		}

		public void Activate() {
			procManager.LaunchProcess(new OutlineAnimProcess(outlineAnimDuration, outlineAnimGradient, material));
			isActive = true;
		}

		public void Desactivate() {
			velocity.x = 0f;
			isActive = false;
		}

		public void LayDead(float layY) {
			if (isDead) return;
			CharactersManager.I.RemoveCharacter(this);
			isDead = true;
			transform.position = new Vector3(transform.position.x, layY, transform.position.z);
			if (transform.localScale.x > 0) {
				transform.rotation = Quaternion.Euler(0f, 0f, -90f);
			} else {
				transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			}
			platform.transform.position = col.bounds.center + Vector3.up * (width / 2f);
			platform.transform.rotation = Quaternion.identity;
			Vector2[] pts = platform.points;
			pts[0].x = -height / 2f;
			pts[1].x = height / 2f;
			platform.points = pts;
			rb.bodyType = RigidbodyType2D.Static;
			rb.sharedMaterial = matDead;
			gameObject.layer = LayerMask.NameToLayer("DeadCharacter");
            if (m_deadSound)
                m_deadSound.Play();
		}

	}
}
