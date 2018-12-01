using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD43;

public class Pike : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		CharController controller = collision.gameObject.GetComponent<CharController>();
		if (controller != null) {
			controller.LayDead();
		}
	}

}
