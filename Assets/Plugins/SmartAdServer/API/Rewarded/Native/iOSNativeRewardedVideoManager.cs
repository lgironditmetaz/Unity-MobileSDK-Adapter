using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	/// <summary>
	/// iOS native rewarded video manager implementation.
	/// </summary>
	public class iOSNativeRewardedVideoManager : NativeRewardedVideoManager
	{

		////////////////////////////////////
		/// Native Wrapper 
		////////////////////////////////////

		[DllImport ("__Internal")]
		private static extern void _LoadRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern void _ShowRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _HasRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);


		////////////////////////////////////
		/// Native rewarded video manager override 
		////////////////////////////////////

		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("iOSNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");
			_LoadRewardedVideo (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
		}

		/// <summary>
		/// Shows a rewarded video interstitial for a given placement if available.
		/// Rewarded video availability for a placement can be checked using the method <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager.CheckRewardedVideoAvailability"/>.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("iOSNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");
			_ShowRewardedVideo (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
		}

		/// <summary>
		/// Checks if a rewarded video interstitial is available for for a given placement.
		/// This operation is asynchronous and requires a callback function send its result.
		/// </summary>
		/// <param name="adConfig">Ad config.</param>
		/// <param name="callback">The callback that will be called with the availability status.</param>
		override public void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback)
		{
			Debug.Log ("iOSNativeRewardedVideoManager > CheckRewardedVideoAvailability(" + adConfig + ")");
			int result = _HasRewardedVideo (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);

			// The callback is called immediately with default value in this implementation so there is no need
			// for a special case in the user app for Default native rewarded video manager implementation
			callback (result == 1 ? true : false); 
		}
	}
}

