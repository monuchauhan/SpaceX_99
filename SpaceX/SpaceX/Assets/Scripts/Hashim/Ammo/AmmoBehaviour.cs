using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehaviour : MonoBehaviour {

	public GameObject explosionFire;
	public GameObject explosionSmoke;
	public GameObject explosionMoab;

	private float leftBoundary,rightBoundary,topBoundary,bottomBoundary;
	private bool scriptActive;

	void Start () {
		Vector3 MaxCamera = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0));
		Vector3 MinCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 0));
		scriptActive = true;

		leftBoundary = MinCamera.x;
		rightBoundary = MaxCamera.x;
		bottomBoundary = MinCamera.y;
		topBoundary = MaxCamera.y;

	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y < bottomBoundary||transform.position.y>topBoundary||
			transform.position.x < leftBoundary||transform.position.x>rightBoundary	) {
			DestroyAmmo();
		}

	}
	void OnTriggerEnter2D(Collider2D col){
		if (scriptActive) {
			if (col.tag == "Player" && tag == "enemyAmmo") {
				KillPlayer (col.gameObject);
				DestroyAmmo ();
			} else if (col.tag == "Ground" && tag!="enemyAmmo") {
				DestroyNear (col);
				TerrainDestruction.instance.MyDestroyGround (transform.position.x, transform.position.y);
				MissileBulletFxPlay ();
				DestroyAmmo ();
			} else if ((col.tag == "enemy" || col.tag == "building") && gameObject.tag != "enemyAmmo") {
				if (gameObject.name.Contains ("Bullet")) {
					GameManager.ScoreIncrease ();
					if (col.tag == "building") {
						GameManager.instance.createPowerUp (col.transform.position);
					}
				}
				if (col.gameObject.name == "eSTAMGround") {
					var temp = col.transform.GetChild (0);
					eSTAMDestroy (temp);
				}
				if (col.tag == "building") {
					Destroy (Instantiate (explosionSmoke, col.gameObject.transform.position, Quaternion.identity), 1.5f);
				}else{
					Destroy(Instantiate(explosionFire,col.gameObject.transform.position,Quaternion.identity),0.5f);
				}
				MissileBulletFxPlay ();
				DestroyNear (col);
				Destroy (col.gameObject,0.5f);

				DestroyAmmo ();
			} else if (col.tag == "terrainObj" && gameObject.name.Contains ("Bullet")) {
				Destroy (col.gameObject);
				DestroyAmmo ();
			} else if (col.tag == "GroundLimit") {
				DestroyAmmo ();
			}
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		//when the bullet collides with anotherbody other than the Player
		if (scriptActive) {
			if (coll.collider.tag == "Player" && tag == "enemyAmmo") {
				KillPlayer (coll.gameObject);
				DestroyAmmo ();
			} else if (coll.collider.tag == "Ground" && tag!="enemyAmmo") {
				DestroyNear (coll);
				TerrainDestruction.instance.MyDestroyGround (transform.position.x, transform.position.y);
				MissileBulletFxPlay ();
				DestroyAmmo ();
			} else if ((coll.collider.tag == "enemy" || coll.collider.tag == "building") && gameObject.tag != "enemyAmmo") {
				if (gameObject.name.Contains ("Bullet")) {
					GameManager.ScoreIncrease ();

					if (coll.gameObject.tag == "building") {
						GameManager.instance.createPowerUp (coll.gameObject.transform.position);
					}
				}
				if (coll.gameObject.name == "eSTAMGround") {
					var temp = coll.transform.GetChild (0);
					eSTAMDestroy (temp);
				}
				if (coll.collider.CompareTag("building")) {
					Destroy (Instantiate (explosionSmoke, coll.gameObject.transform.position, Quaternion.identity), 1.5f);
				}else{
					Destroy(Instantiate(explosionFire,coll.gameObject.transform.position,Quaternion.identity),0.5f);
				}
				MissileBulletFxPlay ();
				DestroyNear (coll);

				Destroy (coll.gameObject,0.5f);

				DestroyAmmo ();
			} else if (coll.collider.tag == "terrainObj"&&gameObject.name.Contains("Bullet")) {
				Destroy (coll.gameObject);
				DestroyAmmo ();
			}else if (coll.collider.tag == "GroundLimit") {
				DestroyAmmo ();
			}

		}
	}
	void DestroyAmmo(){
		if (this.name.Contains("P_Missile")|| this.name.Contains("P_Bullet")) {
			planeController.instance.IncAmmoCount();
		}
		if (transform.parent != null && transform.parent.name == "eSTAMGround") {
			Destroy (transform.parent.gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	IEnumerator shake(){
		int t = 1000;
		Quaternion camData = Camera.main.transform.rotation;
		while (t != 0) {
			Camera.main.transform.rotation = Quaternion.Slerp (Camera.main.transform.rotation, Random.rotation,
				0.1f * Time.deltaTime);
			t--;
			yield return new WaitForSeconds(0.1f);
		}
		Camera.main.transform.rotation = camData;
	}

	void DestroyNear(Collision2D coll){
		if (gameObject.name.Contains ("Missile")) {
			GameManager.instance.destroyNearByEnemies (coll.contacts [0].point, 1.5f);
		}else if (gameObject.name.Contains ("MOAB")) {
			GameManager.instance.destroyNearByEnemies (coll.contacts [0].point, 4f);
			CameraShake.shakeDuration = 1f;
			Destroy (Instantiate (explosionMoab, gameObject.transform.position, Quaternion.identity), 1f);
			FX.Instance.play (PreLoader.Instance.moabExp);
		}
	}
	void DestroyNear(Collider2D col){
		if (gameObject.name.Contains ("Missile")) {
			GameManager.instance.destroyNearByEnemies (col.transform.position, 1.5f);
		} else if (gameObject.name.Contains ("MOAB")) {
			GameManager.instance.destroyNearByEnemies (col.transform.position, 4f);
			CameraShake.shakeDuration = 1f;
			Destroy (Instantiate (explosionMoab, gameObject.transform.position, Quaternion.identity), 1f);
			FX.Instance.play (PreLoader.Instance.moabExp);
		}
	}

	void MissileBulletFxPlay(){
		if(this.name.Contains("ssile"))
			FX.Instance.play (PreLoader.Instance.missExp);
		else if(this.name.Contains("llet"))
			FX.Instance.play (PreLoader.Instance.bullExp);
	}	

	void KillPlayer(GameObject go){
		planeController.instance.Kill ();
	}

	void eSTAMDestroy(Transform temp){
		temp.parent = null;
		if (!temp.GetComponent<eSTAM> ().enabled) {
			Destroy (temp.gameObject);
		} else {
			temp.GetComponent<eSTAM> ().isDead ();
		}
	}
}
