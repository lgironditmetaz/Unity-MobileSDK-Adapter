using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Constants;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public class AndroidNativeRewardedVideoManager : NativeRewardedVideoManager {

		/// <summary>
		/// The java object representing the activity displaying the game.
		/// </summary>
		private AndroidJavaObject _unityActivity;

		private AdConfig _currentAdConfig; // TODO this can't work if the user call several placement at the same time which is a legit use case

		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");
			_currentAdConfig = adConfig;
			RunOnJavaUiThread (LoadOnUiThread);
		}

		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");
			// TODO
		}

		override public bool HasRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > HasRewardedVideo(" + adConfig + ")");
			return false;
		}

		class RewardedVideoListener : AndroidJavaProxy
		{
			AndroidNativeRewardedVideoManager callerManager;

			public RewardedVideoListener (AndroidNativeRewardedVideoManager manager) : base(JavaClass.SASRewardedVideoListener)
			{
				callerManager = manager;
			}

			void onRewardedVideoAdLoadingCompleted(AndroidJavaObject placement, AndroidJavaObject adElement) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoAdLoadingCompleted");
			}

			void onRewardedVideoAdLoadingFailed(AndroidJavaObject placement, AndroidJavaObject exception) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoAdLoadingFailed(" + exception.Call<String>(JavaMethod.ToString) + ")");
			}

			void onRewardedVideoPlaybackError(AndroidJavaObject placement, AndroidJavaObject exception) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoPlaybackError(" + exception.Call<String>(JavaMethod.ToString) + ")");
			}

			void onRewardedVideoClosed(AndroidJavaObject placement) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoClosed");
			}

			void onRewardedVideoClicked(AndroidJavaObject placement, AndroidJavaObject url) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoClicked");
			}

			void onRewardReceived(AndroidJavaObject placement, AndroidJavaObject reward) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardReceived");
			}

			void onRewardedVideoEvent(AndroidJavaObject placement, AndroidJavaObject videoEvent) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoEvent");
			}

			void onRewardedVideoMediaPlayerPrepared(AndroidJavaObject placement, AndroidJavaObject mediaPlayer, AndroidJavaObject playerView, AndroidJavaObject adElement) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoMediaPlayerPrepared");
			}

			void onRewardedVideoMediaPlayerCompleted(AndroidJavaObject placement, AndroidJavaObject mediaPlayer, AndroidJavaObject playerView, AndroidJavaObject adElement) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoMediaPlayerCompleted");
			}

			void onRewardedVideoEndCardDisplayed(AndroidJavaObject placement, AndroidJavaObject endCardView, AndroidJavaObject adElement) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoEndCardDisplayed");
			}

		}

		void LoadOnUiThread()
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > loadRewardedVideo()");
			var adConfig = _currentAdConfig;
			var rewardedVideoManager = new AndroidJavaClass (JavaClass.SASRewardedVideoManager).CallStatic<AndroidJavaObject> (JavaMethod.GetInstance, GetUnityActivity ());
			var rewardedVideoPlacement = new AndroidJavaObject (JavaClass.SASRewardedVideoPlacement, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
			rewardedVideoManager.Call (JavaMethod.SetRewardedVideoListener, new RewardedVideoListener(this));
			rewardedVideoManager.Call (JavaMethod.LoadRewardedVideo, rewardedVideoPlacement);
		}

		/// <summary>
		/// Runs some code on Android UI thread.
		/// </summary>
		/// <param name="method">The method to run on UI thread.</param>
		void RunOnJavaUiThread (Action method)
		{
			GetUnityActivity ().Call (JavaMethod.RunOnUiThread, new AndroidJavaRunnable (method));
		}

		/// <summary>
		/// Gets the activity displaying the game.
		/// </summary>
		/// <returns>The activity displaying the game.</returns>
		AndroidJavaObject GetUnityActivity ()
		{
			if (_unityActivity == null) {
				var unityPlayer = new AndroidJavaClass (JavaClass.UnityPlayer); 
				_unityActivity = unityPlayer.GetStatic<AndroidJavaObject> (JavaMethod.CurrentActivity);
			}
			return _unityActivity;
		}

	}
}
