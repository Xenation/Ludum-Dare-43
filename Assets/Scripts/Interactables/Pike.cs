using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD43;

[RequireComponent(typeof(Collider2D))]
public class Pike : MonoBehaviour {

	private Collider2D col;

	private void Awake() {
		col = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		CharController controller = collision.gameObject.GetComponent<CharController>();
		if (controller != null) {
			controller.LayDead(transform.position.y + col.bounds.extents.y);
		}
	}

}
