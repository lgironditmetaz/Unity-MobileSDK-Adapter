using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmartAdServer.Unity.Library.Rewarded.Native;
using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Factory;

namespace SmartAdServer.Unity.Library.Rewarded
{
	/// <summary>
	/// This class is used to load and display rewarded video interstitials.
	/// It is a singleton class and cannot be instantiated by the user.
	/// </summary>
	public class RewardedVideoManager
	{
		/// <summary>
		/// The unique instance of the rewarded video manager.
		/// </summary>
		private static readonly RewardedVideoManager _instance = new RewardedVideoManager();

		/// <summary>
		/// A native rewarded video manager instance (platform dependant class) used to interact with the ad SDKs.
		/// </summary>
		private NativeRewardedVideoManager _nativeRewardedVideoManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager"/> class.
		/// </summary>
		private RewardedVideoManager() {
			_nativeRewardedVideoManager = PlatformFactory.Instance.BuildInstance<NativeRewardedVideoManager> ();
		}

		/// <summary>
		/// Gets the unique instance of the rewarded video manager.
		/// </summary>
		/// <value>The instance.</value>
		public static RewardedVideoManager Instance
		{
			get
			{
				return _instance; 
			}
		}

		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		public void LoadRewardedVideo(AdConfig adConfig) {
			_nativeRewardedVideoManager.LoadRewardedVideo (adConfig);
		}

		/// <summary>
		/// Shows a rewarded video interstitial for a given placement if available.
		/// Rewarded video availability for a placement can be checked using the method <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager.CheckRewardedVideoAvailability"/>.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		public void ShowRewardedVideo(AdConfig adConfig) {
			_nativeRewardedVideoManager.ShowRewardedVideo (adConfig);
		}

		/// <summary>
		/// Checks if a rewarded video interstitial is available for for a given placement.
		/// This operation is asynchronous and requires a callback function send its result.
		/// </summary>
		/// <param name="adConfig">Ad config.</param>
		/// <param name="callback">The callback that will be called with the availability status.</param>
		public void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback) {
			_nativeRewardedVideoManager.CheckRewardedVideoAvailability (adConfig, callback);
		}
		
	}
}
