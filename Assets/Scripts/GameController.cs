﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.UI;

public class GameController : MonoBehaviour
{
	public GameObject Hazard;
	public Vector3 SpawnValues;
	public int HazardCount;

	public float SpawnWait;
	public float WaveWait;
	public float AdWait;

	public Text ScoreText;
	private int _score = 0;
	public Text ReloadText;
	public Button ReloadButton;
	public Text EndText;

	private bool _gameOver = false;
	private bool _reload = false;
	private bool _isAdReady = false;

	private BannerView _bannerView;
	private InterstitialView _interstitialView;

	void Start ()
	{
		HideGameOverText ();
		UpdateScore ();
		StartCoroutine (SpawnWaves ());

		LoadBanner (); // The banner is loaded as soon as the game starts but isn't displayed immediately
	}
	
	public void GameOver ()
	{
		DisplayAds(); // Advertisment is displayed when the game is over

		_gameOver = true;
		EndText.enabled = true;
		
		StartCoroutine (ActivateReload ());
	}

	void LoadBanner ()
	{
		Debug.Log ("GameController: LoadAd");

		// Create a new banner view instance if needed
		if (_bannerView == null) {
			_bannerView = new BannerView ();
		}

		// Create an adconfig object that will store informations about the ad placement and use it to load the ad
		AdConfig adConfig = new AdConfig ("https://mobile.smartadserver.com", 104808, "663262", 15140, true, "");
		_bannerView.LoadAd (adConfig);

		// Register success & failure events
		_bannerView.AdViewLoadingSuccess += BannerViewSuccess;
		_bannerView.AdViewLoadingFailure += BannerViewFailure;
	}

	void DisplayAds ()
	{
		// The already loaded banner is simply attached to the screen (only if ready).
		if (_bannerView != null && _isAdReady) {
			_bannerView.DisplayBanner (AdPosition.Bottom);
		}

		// The interstitial is loaded and then displayed.
		DisplayInterstitial ();
	}

	void DisplayInterstitial ()
	{
		Debug.Log ("GameController: DisplayInterstitial");

		// Destroy the old interstitialview if needed
		if (_interstitialView != null) {
			_interstitialView.Destroy ();
		}
		// Create a new interstitialview instance
		_interstitialView = new InterstitialView ();

		// Create an adconfig object that will store informations about the ad placement and use it to load the ad
		AdConfig adConfig = new AdConfig ("https://mobile.smartadserver.com", 104808, "663264", 12167, true, "");
		_interstitialView.LoadAd (adConfig); // The interstitial is displayed automatically when loaded

		// Register success & failure events
		_interstitialView.AdViewLoadingSuccess += InterstitialViewSuccess;
		_interstitialView.AdViewLoadingFailure += InterstitialViewFailure;
	}
	
	void BannerViewSuccess (object sender, System.EventArgs e)
	{
		// Event called when the banner is loaded successfuly
		Debug.Log ("GameController: BannerViewSuccess");
		_isAdReady = true;
	}
	
	void BannerViewFailure (object sender, System.EventArgs e)
	{
		// Event called when the banner fails to load
		Debug.Log ("GameController: BannerViewFailure");
	}

	void InterstitialViewSuccess (object sender, System.EventArgs e)
	{
		// Event called when the interstitial is loaded successfuly
		Debug.Log ("GameController: InterstitialViewSuccess");
	}

	void InterstitialViewFailure (object sender, System.EventArgs e)
	{
		// Event called when the interstitial fails to load
		Debug.Log ("GameController: InterstitialViewFailure");
	}

	public void ActualReloading ()
	{
		// Do not forget to destroy any instanciated ad view when you don't need them anymore
		// to avoid any memory leaks that can appear on some platforms.
		if (_bannerView != null) {
			_bannerView.Destroy ();
		}
		if (_interstitialView != null) {
			_interstitialView.Destroy ();
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void AddScore (int newScoreValue)
	{
		_score += newScoreValue;
		UpdateScore ();
	}

	void Update ()
	{
		HandleReload ();
	}

	IEnumerator SpawnWaves ()
	{
		while (!_gameOver) {
			yield return StartCoroutine (SpawnWave ());
			yield return new WaitForSeconds (WaveWait);
		}
	}

	IEnumerator SpawnWave ()
	{
		for (int i = 0; i < HazardCount; i++) {
			SpawnHazard ();
			yield return new WaitForSeconds (SpawnWait);
		}
	}

	void SpawnHazard ()
	{
		Vector3 spawnPosition = new Vector3 (
			Random.Range (-SpawnValues.x, SpawnValues.x), 
			SpawnValues.y, 
			SpawnValues.z
		);
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (Hazard, spawnPosition, spawnRotation);
	}

	IEnumerator ActivateReload ()
	{
		yield return new WaitForSeconds (AdWait);

		if (SystemInfo.deviceType == DeviceType.Desktop) {
			ReloadText.enabled = true;
		} else {
			ReloadButton.gameObject.SetActive (true);
		}
		_reload = true;
	}

	void HandleReload ()
	{
		if (_reload && Input.GetKeyDown (KeyCode.R)) {
			ActualReloading ();
		}
	}

	void UpdateScore ()
	{
		ScoreText.text = "SCORE: " + _score;
	}

	void HideGameOverText ()
	{
		ReloadText.enabled = false;
		ReloadButton.gameObject.SetActive (false);
		EndText.enabled = false;
	}
}
