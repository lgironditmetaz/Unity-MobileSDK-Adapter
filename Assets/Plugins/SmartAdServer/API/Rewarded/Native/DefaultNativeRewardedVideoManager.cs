using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	/// <summary>
	/// Default native rewarded video manager implementation.
	/// </summary>
	public class DefaultNativeRewardedVideoManager : NativeRewardedVideoManager
	{
		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");
		}

		/// <summary>
		/// Shows a rewarded video interstitial for a given placement if available.
		/// Rewarded video availability for a placement can be checked using the method <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager.CheckRewardedVideoAvailability"/>.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");
		}

		/// <summary>
		/// Checks if a rewarded video interstitial is available for for a given placement.
		/// This operation is asynchronous and requires a callback function send its result.
		/// </summary>
		/// <param name="adConfig">Ad config.</param>
		/// <param name="callback">The callback that will be called with the availability status.</param>
		override public void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > CheckRewardedVideoAvailability(" + adConfig + ")");

			// The callback is called immediately with default value in this implementation so there is no need
			// for a special case in the user app for Default native rewarded video manager implementation
			callback (false); 
		}

	}
}
