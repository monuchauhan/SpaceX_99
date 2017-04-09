﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigGunController : MonoBehaviour {

	public GameObject plane;
	public GameObject bullet;
	public static bool isLocked;
	private bool fired;
	private float rf;

	// Use this for initialization
	void Start () {
		isLocked = false;
		fired = false;
		rf = -1f;
		transform.Rotate (new Vector3 (0f, 0f, -1f));
	}
	
	// Update is called once per frame
	void Update () {

		if (!isLocked) {
			if (transform.eulerAngles.z == 180f)
				rf = 1f;
			else if (transform.eulerAngles.z == 359f)
				rf = -1f;
			transform.Rotate (new Vector3 (0f, 0f, rf));
				
		}
		
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.CompareTag ("Player")) {
			isLocked = true;
			StartCoroutine ("fire");
		}
		
		Debug.Log ("trigger enter");
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.CompareTag ("Player")) {
			isLocked = false;
			StopCoroutine ("fire");
		}
		
	}

	/*void OnTriggerStay2D(Collider2D col){
		if (col.CompareTag ("Player")) {
			if (!fired) {
				Debug.Log ("fired");
				StartCoroutine ("fire");
			}
		}
	}*/

	private IEnumerator fire(){

		Vector2 dir;

		while (isLocked) {
			fired = true;
			GameObject obj = Instantiate (bullet, transform.position, Quaternion.identity);
			Debug.Log ("firing");
			dir = (Vector2)(plane.gameObject.transform.position-transform.position);
			dir.Normalize ();
			obj.GetComponent<Rigidbody2D>().AddForce(new Vector2 (
				dir.x,
				dir.y
			) * 5f,
				ForceMode2D.Impulse
			);
			yield return new WaitForSeconds (1f);
			fired = false;
		}

	}
}
