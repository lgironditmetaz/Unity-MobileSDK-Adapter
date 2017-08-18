using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Events;
using SmartAdServer.Unity.Library.UI;
using SmartAdServer.Unity.Library.Rewarded;

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
	public Button RewardButton;
	public Text EndText;

	public GameObject _rewardedPanel;
	public Text _rewardedDescription;

	private bool _gameOver = false;
	private bool _reload = false;
	private bool _isAdReady = false;

	private BannerView _bannerView;
	private InterstitialView _interstitialView;

	private AdConfig _rewardedVideoInterstitialAdConfig = new AdConfig ("https://mobile.smartadserver.com", 104808, "795153", 12167, true, "rewardedvideo");

	void Start ()
	{
		HideGameOverText ();
		HideRewardedPanel ();
		UpdateScore ();
		StartCoroutine (SpawnWaves ());

		LoadBanner (); // The banner is loaded as soon as the game starts but isn't displayed immediately
		LoadRewardedVideoInterstitial (); // The rewarded video interstitial is loaded as soon as the game starts but isn't displayed immediately
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

	void LoadRewardedVideoInterstitial()
	{
		// Loading the rewarded video interstitial using the rewarded video manager and an ad config
		RewardedVideoManager.Instance.LoadRewardedVideo (_rewardedVideoInterstitialAdConfig);

		// Register success, failure & reward events
		RewardedVideoManager.Instance.RewardedVideoLoadingSuccess += RewardedVideoLoadingSuccess;
		RewardedVideoManager.Instance.RewardedVideoLoadingFailure += RewardedVideoLoadingFailure;
		RewardedVideoManager.Instance.RewardedVideoPlaybackFailure += RewardedVideoPlaybackFailure;
		RewardedVideoManager.Instance.RewardedVideoRewardReceived += RewardedVideoRewardReceived;
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

	void DisplayRewardedInterstitial ()
	{
		RewardedVideoManager.Instance.ShowRewardedVideo (_rewardedVideoInterstitialAdConfig);
		RewardButton.gameObject.SetActive (false); // There will be no rewarded video ad remaining so we can deactivate the reward button
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

	void RewardedVideoLoadingSuccess (object sender, System.EventArgs e)
	{
		// Event called when the rewarded video interstitial is loaded
		Debug.Log ("GameController: RewardedVideoLoadingSuccess");
	}

	void RewardedVideoLoadingFailure (object sender, System.EventArgs e)
	{
		// Event called when the rewarded video interstitial fails to load
		Debug.Log ("GameController: RewardedVideoLoadingFailure");
	}

	void RewardedVideoPlaybackFailure (object sender, System.EventArgs e)
	{
		// Event called when the rewarded video interstitial fails to play the video
		Debug.Log ("GameController: RewardedVideoPlaybackFailure");
	}

	void RewardedVideoRewardReceived (object sender, System.EventArgs e)
	{
		var rewardReceivedEventArgs = (RewardedVideoRewardReceivedArgs)e;

		// Event called when the user has collected a reward by watching the rewarded video interstitial.
		// You can get more information about the reward (the currency and the amount) using the event args object.
		Debug.Log ("GameController: RewardedVideoRewardReceived");
		Debug.Log ("Reward Info: " + rewardReceivedEventArgs.Amount + " " + rewardReceivedEventArgs.Currency);

		ShowRewardedPanel (rewardReceivedEventArgs.Currency, rewardReceivedEventArgs.Amount);
	}

	public void ActualReloading ()
	{
		// Do not forget to destroy any instantiated ad view when you don't need them anymore
		// to avoid any memory leaks that can appear on some platforms.
		if (_bannerView != null) {
			_bannerView.Destroy ();
		}
		if (_interstitialView != null) {
			_interstitialView.Destroy ();
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void GetReward ()
	{
		DisplayRewardedInterstitial ();
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

		var rewardedVideoInterstitialLoaded = RewardedVideoManager.Instance.HasRewardedVideo (_rewardedVideoInterstitialAdConfig);
		Debug.Log ("GameController: ActivateReload - rewarded video interstitial loaded: " + rewardedVideoInterstitialLoaded);

		if (SystemInfo.deviceType == DeviceType.Desktop) {
			ReloadText.enabled = true;
		} else {
			ReloadButton.gameObject.SetActive (true);
			RewardButton.gameObject.SetActive (rewardedVideoInterstitialLoaded); // the 'get reward' button is enabled only if we have an ad
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
		RewardButton.gameObject.SetActive (false);
		EndText.enabled = false;
	}

	void ShowRewardedPanel(string currency, double amount)
	{
		_rewardedDescription.text = "You won " + amount + " " + currency + "!";
		_rewardedPanel.SetActive (true);

		// The banner is hidden when the panel is displayed because the banner is on top of the game
		if (_bannerView != null) {
			_bannerView.SetVisible (false);
		}
	}

	public void HideRewardedPanel()
	{
		_rewardedPanel.SetActive (false);

		// The banner visibility is restored if it exists
		if (_bannerView != null) {
			_bannerView.SetVisible (true);
		}
	}

}
