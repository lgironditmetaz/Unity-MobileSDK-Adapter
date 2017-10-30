using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;
using System.Collections.Generic;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Events;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	/// <summary>
	/// iOS native rewarded video manager implementation.
	/// </summary>
	public class iOSNativeRewardedVideoManager : NativeRewardedVideoManager
	{

        public class PlacementStatus
        {
            public Timer timer;

            public bool loadStatus;
            public bool rewarded;
            
            public PlacementStatus(Timer timer)
            {
                this.timer = timer;

                this.loadStatus = false;
                this.rewarded = false;
            }

        }

		////////////////////////////////////
		/// Native Wrapper 
		////////////////////////////////////

		[DllImport ("__Internal")]
		private static extern void _LoadRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern void _ShowRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _HasRewardedVideo(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckRewardedVideoDidLoad(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckRewardedVideoDidFailToLoad(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckRewardedVideoDidFailToShow(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckRewardedVideoDidDisappear(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckRewardedVideoDidCollectReward(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern string _RetrieveRewardedVideoCurrency(string baseUrl, int siteId, string pageId, int formatId, string target);

		[DllImport ("__Internal")]
		private static extern double _RetrieveRewardedVideoAmount(string baseUrl, int siteId, string pageId, int formatId, string target);


        ////////////////////////////////////
        /// Native rewarded video manager override 
        ////////////////////////////////////

        private Dictionary<AdConfig, PlacementStatus> _placementStatus = new Dictionary<AdConfig, PlacementStatus>();

		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("iOSNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");

            if (_placementStatus.ContainsKey (adConfig)) {
				Debug.Log ("There is already a rewarded video interstitial being loaded or waiting to be shown for this ad config!\nLoading cancelled!");
				return;
			}

			var timer = new System.Timers.Timer();
			timer.Elapsed += (object sender, ElapsedEventArgs e) => OnDelegateTimerEvent(sender, e, adConfig);
			timer.Interval = 100;
			timer.Enabled = true;

            _placementStatus.Add(adConfig, new PlacementStatus(timer));

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

		/// <summary>
		/// Timer called until either the success or fail delegate are called in the native wrapper.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="e">Arguments.</param>
		/// <param name="adConfig">Ad config handled by this timer.</param>
		private void OnDelegateTimerEvent(object source, ElapsedEventArgs e, AdConfig adConfig) 
		{
            if (_placementStatus [adConfig].loadStatus == false) {
				// Waiting for ad loading or failure

				bool isRewardedVideoDidFailToLoad = _CheckRewardedVideoDidFailToLoad (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target) == 1 ? true : false;
				if (isRewardedVideoDidFailToLoad) {
					Debug.Log ("iOSNativeRewardedVideoManager > OnDelegateTimerEvent() > rewarded video interstitial fails to load!");

                    _placementStatus [adConfig].timer.Enabled = false;
                    _placementStatus.Remove(adConfig);
                
					NotifyRewardedVideoLoadingFailure (new RewardedVideoLoadingFailureArgs (adConfig));

					return;
				}

				bool isRewardedVideoDidLoad = _CheckRewardedVideoDidLoad (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target) == 1 ? true : false;
				if (isRewardedVideoDidLoad) {
					Debug.Log ("iOSNativeRewardedVideoManager > OnDelegateTimerEvent() > rewarded video interstitial did load");

                    _placementStatus [adConfig].loadStatus = true;

					NotifyRewardedVideoLoadingSuccess (new RewardedVideoLoadingSuccessArgs (adConfig));

					return;
				}

			} else {
				// Waiting for ad playback failure, rewarded collection or closing
				
				bool isRewardedVideoDidFailToShow = _CheckRewardedVideoDidFailToShow (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target) == 1 ? true : false;
				if (isRewardedVideoDidFailToShow) {
					Debug.Log ("iOSNativeRewardedVideoManager > OnDelegateTimerEvent() > rewarded video interstitial fails to play!");

                    _placementStatus [adConfig].timer.Enabled = false;
                    _placementStatus.Remove(adConfig);

					NotifyRewardedVideoPlaybackFailure (new RewardedVideoPlaybackFailureArgs (adConfig));

					return;
				}
				bool isRewardedVideoDidCollectReward = _CheckRewardedVideoDidCollectReward (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target) == 1 ? true : false;
                if (_placementStatus[adConfig].rewarded == false && isRewardedVideoDidCollectReward) {
					Debug.Log ("iOSNativeRewardedVideoManager > OnDelegateTimerEvent() > rewarded video did collect reward");

					var currency = _RetrieveRewardedVideoCurrency (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
					var amount = _RetrieveRewardedVideoAmount (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target);
					var rewardArgs = new RewardedVideoRewardReceivedArgs (adConfig, currency, amount);

					NotifyRewardedVideoRewardReceived (rewardArgs);

                    _placementStatus[adConfig].rewarded = true;

					return;
				}

				bool isRewardedVideoDidDisappear = _CheckRewardedVideoDidDisappear (adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Target) == 1 ? true : false;
				if (isRewardedVideoDidDisappear) {
					Debug.Log ("iOSNativeRewardedVideoManager > OnDelegateTimerEvent() > rewarded video did disappear");

                    _placementStatus [adConfig].timer.Enabled = false;
                    _placementStatus.Remove(adConfig);

                    // TODO notifiy rewarded video interstitial disappearance

					return;
				}

			}
		}
	}
}

