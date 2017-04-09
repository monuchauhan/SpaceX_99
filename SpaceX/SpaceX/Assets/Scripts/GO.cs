using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GO : MonoBehaviour {
	public Text scoreText;
	public GameObject confetti;
	void Start () {
		MenuSound.Instance.asource.enabled = PreLoader.Instance.currentSoundState;
		MenuSound.Instance.sond ();
		PreLoader.Instance.startGame = false;
		scoreText.text += " " + PreLoader.Instance.currentScore;
		if (PreLoader.Instance.highScore <= PreLoader.Instance.currentScore) {
			confetti.gameObject.SetActive (true);
		}
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			back ();
		}
	}

	public void restart()
	{
		PreLoader.Instance.currentScore = 0;
		FX.Instance.play();
		SceneManager.LoadScene ("Game");
	}

	public void back()
	{
		FX.Instance.play();
		SceneManager.LoadScene ("Main Screen");
	}

	public void LeaderBoard()
	{
		FX.Instance.play();
	}
}
