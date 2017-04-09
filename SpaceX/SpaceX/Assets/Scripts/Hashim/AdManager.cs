using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour {

	public static AdManager Instance { set; get; }
	public string insterstialId;
	private InterstitialAd interstitial;   //insert  your id
	private void Start(){

		Instance = this;
		DontDestroyOnLoad (gameObject);

		#if UNITY_EDITOR
//		print("Testing is on");
		#endif
		this.RequestInterstitial ();
	
	}


	// Returns an ad request with custom ad targeting.
	private AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator) //uncomment for real ads 
					.Build();
	
	}
	private void RequestInterstitial()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = insterstialId;		
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create an interstitial.
		this.interstitial = new InterstitialAd (adUnitId);

		// Load an interstitial ad.
		this.interstitial.LoadAd (this.CreateAdRequest ());
	}
	
	public void ShowInterstitial()
	{
		if (this.interstitial.IsLoaded ()) {
			this.interstitial.Show ();
		} else {
			MonoBehaviour.print ("Interstitial is not ready yet");
		}

	}

	
}
