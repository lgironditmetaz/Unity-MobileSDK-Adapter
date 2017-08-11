﻿using System;
using System.Collections.Generic;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Constants;
using SmartAdServer.Unity.Library;
using SmartAdServer.Unity.Library.Utils;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	#if UNITY_ANDROID
	/// <summary>
	/// Android native rewarded video manager implementation.
	/// </summary>
	public class AndroidNativeRewardedVideoManager : NativeRewardedVideoManager
	{
		/// <summary>
		/// The Java rewarded video manager.
		/// </summary>
		private AndroidJavaObject _rewardedVideoManager;

		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");

			AndroidUtils.Instance.RunOnJavaUiThread (() => {
				var rewardedVideoPlacement = new AndroidJavaObject (JavaClass.SASRewardedVideoPlacement, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
				GetRewardedVideoManager ().Call (JavaMethod.SetRewardedVideoListener, new RewardedVideoListener(this));
				GetRewardedVideoManager ().Call (JavaMethod.LoadRewardedVideo, rewardedVideoPlacement);
			});
		}

		/// <summary>
		/// Shows a rewarded video interstitial for a given placement if available.
		/// Rewarded video availability for a placement can be checked using the method <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager.CheckRewardedVideoAvailability"/>.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");

			AndroidUtils.Instance.RunOnJavaUiThread (() => {
				var rewardedVideoPlacement = new AndroidJavaObject (JavaClass.SASRewardedVideoPlacement, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
				GetRewardedVideoManager ().Call (JavaMethod.SetRewardedVideoListener, new RewardedVideoListener(this));
				GetRewardedVideoManager ().Call (JavaMethod.ShowRewardedVideo, rewardedVideoPlacement, AndroidUtils.Instance.GetUnityActivity());
			});
		}

		/// <summary>
		/// Checks if a rewarded video interstitial is available for for a given placement.
		/// This operation is asynchronous and requires a callback function send its result.
		/// </summary>
		/// <param name="adConfig">Ad config.</param>
		/// <param name="callback">The callback that will be called with the availability status.</param>
		override public void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback)
		{
			Debug.Log ("AndroidNativeRewardedVideoManager > CheckRewardedVideoAvailability(" + adConfig + ")");

			AndroidUtils.Instance.RunOnJavaUiThread (() => {
				var rewardedVideoPlacement = new AndroidJavaObject (JavaClass.SASRewardedVideoPlacement, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
				GetRewardedVideoManager ().Call (JavaMethod.SetRewardedVideoListener, new RewardedVideoListener(this));
				var available = GetRewardedVideoManager ().Call<bool> (JavaMethod.HasRewardedVideo, rewardedVideoPlacement);

				callback (available);
			});
		}

		/// <summary>
		/// Retrieve the Java rewarded video manager from the Android SDK.
		/// </summary>
		/// <returns>The rewarded video manager.</returns>
		private AndroidJavaObject GetRewardedVideoManager()
		{
			if (_rewardedVideoManager == null) {
				_rewardedVideoManager = new AndroidJavaClass (JavaClass.SASRewardedVideoManager).CallStatic<AndroidJavaObject> (JavaMethod.GetInstance, AndroidUtils.Instance.GetUnityActivity ());
			}
			return _rewardedVideoManager;
		}

		/// <summary>
		/// Class that will act as a Java listener to handle ad call success & failure and playback events.
		/// </summary>
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
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoAdLoadingFailed(" + exception.Call<String>(JavaMethod.ToStringJava) + ")");
			}

			void onRewardedVideoPlaybackError(AndroidJavaObject placement, AndroidJavaObject exception) {
				Debug.Log ("AndroidNativeRewardedVideoManager.RewardedVideoListener > onRewardedVideoPlaybackError(" + exception.Call<String>(JavaMethod.ToStringJava) + ")");
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

			void onRewardedVideoEvent(AndroidJavaObject placement, Int32 videoEvent) {
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

	}
	#endif
}
