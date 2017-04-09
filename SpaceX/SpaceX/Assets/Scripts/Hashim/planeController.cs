using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class planeController : MonoBehaviour {
	private const float BULLET_THROW_FORCE = 5f;
	private const float BOMB_THROW_FORCE = 2f;
	private const float CLOSE_TOUCH_DISTANCE = 1f;
	private const float BOMB_X_FORCE_MULTIPLIER = 2f;
	private const float DURATION_BETWEEN_BULLETS = 0.1f;
	private const int NO_OF_BULLETS = 3;

	public static planeController instance;
	public GameObject plane, LifeTracker;

	public float speed;
	public float deltaAngle;
	public float angle;

	private int AmmoCount;
	private int MoabCount;

	public Text ammoLeft, moabLeft;
	public Joystick joystick;

	public GameObject missile,bullet,bulletChota,moab;
	struct fireDetails{
		public float fireTime;
		public float fireDuration;
		public fireDetails(float dur){
			fireTime=Time.time;
			fireDuration=dur;
		}
	};

	fireDetails primaryFD,secondaryFD,moabFD;

	private float leftBoundary,rightBoundary,topBoundary,bottomBoundary;
	private BoxCollider2D planeCollider;
	private Vector3 startPos;
	private Quaternion startRot;

	private bool rotatePlane;
	Vector3 newTouch;

	private float justBounce;
	private float bounceBetweenDuration=0.1f;

	private Rigidbody2D rb;

	void Start () {
		instance = this;
		Vector3 MaxCamera = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0));
		Vector3 MinCamera = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 0));

		rb = GetComponent<Rigidbody2D> ();

		if (PreLoader.Instance.aircraftType == 0) {
			gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipX;
		} else {
			gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipY;
		}
		if (PreLoader.Instance.levelNo == 1) {
			PreLoader.Instance.ammoTotal = 1;
			PreLoader.Instance.moabTotal = 0;
			if (PreLoader.Instance.aircraftType == 0) {
				AmmoCount = 1;
			} else {
				AmmoCount = 3;
			}
		} else {
			if (PreLoader.Instance.aircraftType == 0) {
				AmmoCount = 1*PreLoader.Instance.ammoTotal;
			} else {
				AmmoCount = 3*PreLoader.Instance.ammoTotal;
			}
			MoabCount = PreLoader.Instance.moabTotal;
		}

		Transform t = LifeTracker.transform;

		switch (PreLoader.Instance.lifeCount) {
		case 1:
			t.GetChild (2).gameObject.SetActive (true);
			break;
		case 2:
			t.GetChild (2).gameObject.SetActive (true);
			t.GetChild (1).gameObject.SetActive (true);
			break;
		case 3:
			t.GetChild (2).gameObject.SetActive (true);
			t.GetChild (1).gameObject.SetActive (true);
			t.GetChild (0).gameObject.SetActive (true);
			break;
		}


		primaryFD = new fireDetails(0.2f) ;
		secondaryFD = new fireDetails(0.5f);
		moabFD = new fireDetails(0.2f);

		leftBoundary = MinCamera.x;
		rightBoundary = MaxCamera.x;
		bottomBoundary = MinCamera.y;
		topBoundary = MaxCamera.y;

		startPos = transform.GetChild(0).transform.position;
		startRot = transform.GetChild(0).transform.rotation;

		planeCollider = transform.GetChild (0).GetComponent<BoxCollider2D> ();

	}

	// Update is called once per frame
	void Update () {
		if (PreLoader.Instance.aircraftType == 0) {
			ammoLeft.text = ": " + AmmoCount;
		} else {
			ammoLeft.text = ": " + AmmoCount/3;

		}
		moabLeft.text = ": " + MoabCount;
		if (PreLoader.Instance.dpadState) {
			JoystickControl ();
		} else {
			MouseControl ();
			TouchControl ();
		}
	CommonControl ();


		//angle += deltaAngle;
		transform.GetChild(0).rotation = Quaternion.Euler (new Vector3 (0, 0, angle+90));

		Vector3 toMove = new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), Mathf.Sin (Mathf.Deg2Rad * angle), 0) * Time.deltaTime*speed;
		transform.position += toMove;

		PlayerRestriction ();
		if (transform.GetChild (0).eulerAngles.z > 180) {
			if (PreLoader.Instance.aircraftType == 0) {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipXinv;
			} else {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipYinv;
			}
		} else {
			if (PreLoader.Instance.aircraftType == 0) {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipX;
			} else {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = PreLoader.Instance.shipY;
			}
		}
			


	}
	public void IncAmmoCount(){
		AmmoCount++;
	}
	public void FirePrimary(){
		print (AmmoCount);
		if (Time.time - primaryFD.fireTime > primaryFD.fireDuration) {
			if (PreLoader.Instance.aircraftType==0) {
				if (AmmoCount > 0) {
					FireMissiles ();
					AmmoCount--;
				}
			} else {
				if (AmmoCount  > 0) {
					StartCoroutine (FireGun ());
					AmmoCount -= 3;
				}
			}
			primaryFD.fireTime = Time.time;
		}
	}
	public void FireSecondary(){
		if (Time.time - secondaryFD.fireTime > secondaryFD.fireDuration) {
				Firing (bulletChota, 90, BULLET_THROW_FORCE);
				secondaryFD.fireTime = Time.time;

		}

	}

	public void FireMOAB(){
		
		if (Time.time - moabFD.fireTime > moabFD.fireDuration && MoabCount>0) {
			Firing (moab, 90, BULLET_THROW_FORCE, 1/BOMB_X_FORCE_MULTIPLIER);
			moabFD.fireTime = Time.time;
			MoabCount--;
			PreLoader.Instance.moabTotal--;
		}
	}

	private void FireMissiles(){
			Firing (missile, 90, BOMB_THROW_FORCE,BOMB_X_FORCE_MULTIPLIER);
			
	}
	IEnumerator FireGun(){
		for (int i = 0; i < NO_OF_BULLETS; i++) {

			Firing (bullet,90, BULLET_THROW_FORCE);

			yield return new WaitForSeconds(DURATION_BETWEEN_BULLETS);
		}
	}


	private void Firing(GameObject ammo,float orirntationAngle,float forwardForce,float horizontalMultiplier=1){
		var ammoObj=Instantiate (ammo, transform.position,Quaternion.Euler(0,0,angle-orirntationAngle));
		ammoObj.GetComponent<Rigidbody2D> ().AddForce (
			new Vector2 (
				Mathf.Cos (Mathf.Deg2Rad * angle)*horizontalMultiplier,
				Mathf.Sin (Mathf.Deg2Rad * angle)
			) * forwardForce,
			ForceMode2D.Impulse
		);

	}

	void PlayerRestriction(){
		if (newTouch.x < leftBoundary ||
			newTouch.x > rightBoundary ||
			newTouch.y < bottomBoundary ||			//touch point is out of boundary then dont rotate
			newTouch.y > topBoundary) {
			rotatePlane = false;
		}



		if (transform.position.y >= topBoundary-planeCollider.size.y/2) {		//bounce on top
			angle = 360f-angle;
			justBounce = Time.time;
		}

		if (transform.position.x < leftBoundary-planeCollider.size.x/2) {
			transform.position = new Vector3 (rightBoundary, transform.position.y, 0);
		}else if (transform.position.x > rightBoundary+planeCollider.size.x/2) {
			transform.position = new Vector3 (leftBoundary, transform.position.y, 0);
		}
	}

	void TouchControl(){
		if (Input.touchCount > 0) {
			// The screen has been touched so store the touch
			Touch touch = Input.GetTouch (0);

			if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
				// If the finger is on the screen, move the object smoothly to the touch position
				newTouch=new Vector3(touch.position.x,touch.position.y,0);
				newTouch = Camera.main.ScreenToWorldPoint (newTouch);
				newTouch = new Vector3 (newTouch.x, newTouch.y, 0);
				rotatePlane = true;
			}
			if (EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
				rotatePlane = false;
				return;
			}

		}


		/*if (Input.touchCount > 1) {
//			FireBomb ();
		}*/

	}

	void MouseControl(){
		if (Input.GetButton ("Fire1")) {
			newTouch=new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
			newTouch = Camera.main.ScreenToWorldPoint (newTouch);
			newTouch = new Vector3 (newTouch.x, newTouch.y, 0);
			rotatePlane = true;

		}


		if (Input.GetButton ("Fire2")) {
//			FireBomb ();

		}
	}
	void JoystickControl(){

		if (Mathf.Abs(joystick.Horizontal ()) > 0.05 || Mathf.Abs(joystick.Vertical ()) > 0.05) {

			newTouch = new Vector3 (joystick.Horizontal () + transform.position.x, joystick.Vertical () + transform.position.y, 0);
			rotatePlane = true;
		} 
	}

	void CommonControl(){
		if (rotatePlane&&Time.time-justBounce>bounceBetweenDuration) {
//			print (newTouch + " " + transform.position);;
			Vector3 targetDir = newTouch - transform.position;
			float newAngle = Angle360 (Vector3.left, targetDir, Vector3.forward);

			newAngle -= angle;		//reference to angle
			newAngle = (int)newAngle;
			float dAngle = 2f;
			if (newAngle > 0+dAngle && newAngle <= 180f || newAngle <= -180f) {
				angle += deltaAngle;	
			} else if (newAngle >=180f || newAngle < 0-dAngle && newAngle >= -180f) {
				angle -= deltaAngle;
			} else {
				rotatePlane = false;
			}
			if (Vector3.Distance (newTouch, transform.position) < 1f) {
				rotatePlane = false;
			}
			//angle = angle < 0 ? angle + 360f : angle;
			angle += 360f;
			angle %= 360f;
		}
	}

	float Angle360(Vector3 v1, Vector3 v2, Vector3 n)
	{
		//  Acute angle [0,180]
		float angle = Vector3.Angle(v1,v2);

		//  -Acute angle [180,-179]
		float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(v1, v2)));
		float signed_angle = angle * sign;

		//  360 angle
		return (signed_angle + 180) % 360;
	}


	public void powerUpCaught(string name){
		if (name == "PU_missileInc") {
			if (PreLoader.Instance.aircraftType == 0) {
				AmmoCount++;
			} else {
				AmmoCount += 3;
			}
			PreLoader.Instance.ammoTotal +=1;
		} else if (name == "PU_moabInc") {
			MoabCount++;
			PreLoader.Instance.moabTotal +=1;
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "enemy" || col.tag == "Ground" || col.tag == "building" || col.tag == "terrain") {
			if (PreLoader.Instance.lifeCount > 1) {
				PreLoader.Instance.lifeCount--;
				StopAllCoroutines ();
				transform.position = startPos;
				transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 0f));
				transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
				LifeTracker.transform.GetChild (3 - PreLoader.Instance.lifeCount-1).gameObject.SetActive (false);
				angle = 0f;
				StartCoroutine ("blink");
			} else {
				//kill player
				GameManager.instance.GameOver ();
				Destroy (this.gameObject);
			}
		}
	}
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "enemy" || coll.gameObject.tag == "Ground" ||
			coll.gameObject.tag == "building" || coll.gameObject.tag == "terrain" || coll.gameObject.tag == "enemyAmmo") {
				if (PreLoader.Instance.lifeCount >= 1) {
					PreLoader.Instance.lifeCount--;
				StopAllCoroutines ();
				transform.position = startPos;
				transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 0f));
				transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
				LifeTracker.transform.GetChild (3 - PreLoader.Instance.lifeCount-1).gameObject.SetActive (false);
				angle = 0f;
				StartCoroutine ("blink");
			} else {
					//kill player
					GameManager.instance.GameOver ();
					Destroy (this.gameObject);
				}
				if (coll.gameObject.tag != "Ground" || coll.gameObject.tag == "enemyAmmo") {
					Destroy (coll.gameObject);
				}
		}
	}

	IEnumerator blink(){
		int i = 5;
		Color tmp = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer> ().color;
		while (i > 0) {
			i--;
			tmp.a = 0;
			gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = tmp;
			yield return new WaitForSeconds (0.1f);
			tmp.a = 255;
			gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = tmp;
			yield return new WaitForSeconds (0.1f);
		}
	}

	public void Kill(){
		if (PreLoader.Instance.lifeCount > 1) {
			PreLoader.Instance.lifeCount--;
			StopAllCoroutines ();
			transform.position = startPos;
			transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 0f));
			transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
			LifeTracker.transform.GetChild (3 - PreLoader.Instance.lifeCount-1).gameObject.SetActive (false);
			angle = 0f;
			StartCoroutine ("blink");
		} else {
			//kill player
			GameManager.instance.GameOver ();
			Destroy (this.gameObject);
		}
	}
}

