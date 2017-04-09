using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigController : MonoBehaviour {

	private float leftBoundary,rightBoundary,topBoundary,bottomBoundary;

	// Use this for initialization
	void Start () {
		if (transform.position.x < leftBoundary) {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (1f, 0f);
		} else {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (-1f, 0f);
			transform.Rotate(new Vector3(0f,180f,0f));
		}
		Vector3 MaxCamera = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0));
		Vector3 MinCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 0));

		leftBoundary = MinCamera.x;
		rightBoundary = MaxCamera.x;
		bottomBoundary = MinCamera.y;
		topBoundary = MaxCamera.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x > rightBoundary+40f || transform.position.x <leftBoundary-40f) {
			Destroy (gameObject);
		}

	}
}
