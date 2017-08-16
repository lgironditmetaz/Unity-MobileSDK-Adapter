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
		/// </summary>
		/// <returns><c>true</c> if a rewarded video interstitial is available for this adConfig; otherwise, <c>false</c>.</returns>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public bool HasRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("iOSNativeRewardedVideoManager > CheckRewardedVideoAvailability(" + adConfig + ")");
			int result = _HasRewardedVideo (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
			return result == 1 ? true : false; 
		}
	}
}

