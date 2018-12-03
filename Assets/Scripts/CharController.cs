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
        public GameObject OverlayPosition;
        [SerializeField] private AudioSource m_deadSound;
        [SerializeField] private AudioSource m_jumpSound;
        [SerializeField] private AudioSource m_landSound;
        private bool m_lastFrameInAir = false;

		[SerializeField] private Gradient outlineAnimGradient;
		[SerializeField] private float outlineAnimDuration = 1f;
		[SerializeField] private Animator animator;
		private int isRunningId;
		private int isPushingId;
		private int isJumpingId;
		private int isDeadId;

		[SerializeField] private GameObject bloodParticlesPrefab;

		private float gravity = 1f;
		private float ascentGravity = 1f;
		private float coastGravity = 1f;
		private float descentGravity = 1f;
		private float jumpVelocity = 0f;
		private bool dropThrough = false;

		public bool isActive = false;
		public bool isDead = false;
		private bool inAir = true;
		private Rigidbody2D rb;
		private Collider2D col;
		private Vector2 velocity = Vector2.zero;
		private float inputHorizVel = 0f;
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
        private bool m_firstLandIgnore = false;
        private bool m_firstAirIgnore = false;

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
			float ascentDist = jumpDistance * 0.6666666f;
			float descentDist = jumpDistance * 0.3333333f;
			ascentGravity = (-2f * jumpHeight * jumpAirSpeed * jumpAirSpeed) / (ascentDist * ascentDist);
			descentGravity = (-2f * jumpHeight * jumpAirSpeed * jumpAirSpeed) / (descentDist * descentDist);
			coastGravity = descentGravity;
			jumpVelocity = (2f * jumpHeight * jumpAirSpeed) / ascentDist;
			gravity = descentGravity;

			subRenderers = new List<SpriteRenderer>();
			GetComponentsInChildren(subRenderers);
			material = new Material(Shader.Find("Custom/SpriteOutline"));
			material.SetColor("_OutlineColor", Color.clear);
			foreach (SpriteRenderer rend in subRenderers) {
				List<Material> sharedMats = new List<Material>();
				rend.GetSharedMaterials(sharedMats);
				sharedMats.Add(material);
				rend.sharedMaterials = sharedMats.ToArray();
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
			isDeadId = Animator.StringToHash("isDead");

			OverlayPosition overlayPositionComp = GetComponentInChildren<OverlayPosition>();
			if (overlayPositionComp)
				OverlayPosition = overlayPositionComp.gameObject;
		}

		private void Update() {
			procManager.UpdateProcesses(Time.deltaTime);
			if (isDead) return;

			if (isActive && DialogController.I.CharactersDisplaying == 0) {
				// Input
				inputHorizVel = Input.GetAxisRaw("Horizontal");
				// Look Side
				if (inputHorizVel > 0) {
					transform.localScale = new Vector3(1f, 1f, 1f);
				} else if (inputHorizVel < 0) {
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				
				// Jump
				if (Input.GetButtonDown("Jump") && !inAir) {
					rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
					gravity = ascentGravity;
					platform.enabled = false;
				}
				if (Input.GetButtonUp("Jump") && inAir && rb.velocity.y > 0f) {
					gravity = coastGravity;
				}

				if (Input.GetButtonDown("DropThrough")) {
					dropThrough = true;
				}
			}
		}
		
		private void FixedUpdate() {
			if (isDead) return;
			Collider2D[] colliders = new Collider2D[8];
			int colCount;
			velocity.x = inputHorizVel;

			if (isActive) {
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

					if (dropThrough) {
						dropThrough = false;
						DropThroughPlatform plat = colliders[i].GetComponent<DropThroughPlatform>();
						if (plat) {
							plat.AllowDropThrough(col, .2f);
						}
					}

					inAir = false;
					animator.SetBool(isJumpingId, false);

                    if(m_lastFrameInAir) // if last frame char was in air and now he is not : he is landing
                    {
						platform.enabled = true;
						if (m_landSound && m_firstLandIgnore)
                            m_landSound.Play();

                        if (!m_firstLandIgnore)
                            m_firstLandIgnore = true;
                    }
				}
			}
			if (inAir) {
				animator.SetBool(isJumpingId, true);

                if(!m_lastFrameInAir)
                {
                    if (m_jumpSound && m_firstLandIgnore)
                        m_jumpSound.Play();

                    if (!m_firstAirIgnore)
                        m_firstAirIgnore = true;
                }
			}

            m_lastFrameInAir = inAir;

			velocity.y = rb.velocity.y + gravity * Time.fixedDeltaTime;

			// Gravity
			if (rb.velocity.y > 0 && velocity.y < 0) {
				gravity = descentGravity;
			}
			rb.velocity = velocity;
		}

		public void Activate() {
			procManager.LaunchProcess(new OutlineAnimProcess(outlineAnimDuration, outlineAnimGradient, material));
			GameManager.UpdatePlayerIndicator(this, height + .5f);
			isActive = true;
		}

		public void Desactivate() {
			inputHorizVel = 0f;
			isActive = false;
		}

		public void LayDead(float layY) {
			if (isDead) return;
            if (PlayerType == PlayerTypesFlag.Leader)
                GameManager.ResetLevel();

			CharactersManager.I.RemoveCharacter(this);
			isDead = true;
			animator.SetBool(isRunningId, false);
			animator.SetBool(isJumpingId, false);
			animator.SetBool(isDeadId, true);
			if (transform.localScale.x > 0) {
				transform.position = new Vector3(transform.position.x - height / 2f, layY, transform.position.z);
				transform.rotation = Quaternion.Euler(0f, 0f, -90f);
			} else {
				transform.position = new Vector3(transform.position.x + height / 2f, layY, transform.position.z);
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
			Instantiate(bloodParticlesPrefab, col.bounds.center, Quaternion.identity, transform);
		}

	}
}
