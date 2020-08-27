using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W)) {
			anim.SetInteger ("State", 1);
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			anim.SetInteger ("State", 0);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			anim.SetInteger ("State", 2);
		}
		if (Input.GetKeyUp (KeyCode.D)) {
			anim.SetInteger ("State", 3);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			anim.SetInteger ("State", 4);
		}
		if (Input.GetKeyUp (KeyCode.A)) {
			anim.SetInteger ("State", 5);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			anim.SetInteger ("State", 6);
		}
		if (Input.GetKeyUp (KeyCode.S)) {
			anim.SetInteger ("State", 7);
		}
	}
}