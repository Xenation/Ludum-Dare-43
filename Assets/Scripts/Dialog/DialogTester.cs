using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTester : MonoBehaviour {

    [SerializeField] private KeyCode m_input;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(m_input))
            GameManager.DialogController.NextText();
	}
}
