using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUp : MonoBehaviour {

	// Use this for initialization
	private float topBoundary;
	void Start () {
		topBoundary = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).y;
		topBoundary += 1f;	//fail Safe
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate (Vector3.up * Time.deltaTime * 1);
		if (transform.position.y >= topBoundary) {
			Destroy (gameObject);
		}
	}
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag ("Player")) {
			planeController.instance.powerUpCaught (name);
			Destroy (this.gameObject);
		}

	}	
}
