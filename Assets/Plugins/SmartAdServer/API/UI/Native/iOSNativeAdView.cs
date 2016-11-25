﻿using UnityEngine;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using System.Timers;

using SmartAdServer.Unity.Library.Constants;
using SmartAdServer.Unity.Library.Models;

namespace SmartAdServer.Unity.Library.UI.Native
{
#if UNITY_IPHONE
	public class iOSNativeAdView : NativeAdView
	{
		private int _currentAdView = -1;
		private System.Timers.Timer _delegatePollingTimer;


		////////////////////////////////////
		/// Native Wrapper 
		////////////////////////////////////

		[DllImport ("__Internal")]
		private static extern int _InitAdView(int type);

		[DllImport ("__Internal")]
		private static extern void _ReleaseAdView(int adId);

		[DllImport ("__Internal")]
		private static extern void _LoadAd(int adId, string baseUrl, int siteId, string pageId, int formatId, int master, string target);

		[DllImport ("__Internal")]
		private static extern int _CheckForLoadedDelegate(int adId);

		[DllImport ("__Internal")]
		private static extern int _CheckForFailedDelegate(int adId);
		
		[DllImport ("__Internal")]
		private static extern int _DisplayBanner(int adId, int position);


		public iOSNativeAdView (AdType type) : base(type)
		{
			Debug.Log ("iOSNativeAdView > DefaultNativeAdView(" + type + ")");
			_currentAdView = _InitAdView ((int)type);

			// Delegate polling timer used to check if the success or fail delegate has been called
			// in the native wrapper.
			_delegatePollingTimer = new System.Timers.Timer();
			_delegatePollingTimer.Elapsed += new ElapsedEventHandler(OnDelegateTimerEvent);
			_delegatePollingTimer.Interval = 100;
		}

		override public void LoadAd (AdConfig adConfig)
		{
			Debug.Log ("iOSNativeAdView > LoadAd(" + adConfig + ")");
			if (_currentAdView != -1) {
				// After launching the ad loading using the native wrapper, the polling timer is enabled to
				// enable notification at the end of the ad loading.
				_LoadAd (_currentAdView, adConfig.BaseUrl, adConfig.SiteId, adConfig.PageId, adConfig.FormatId, adConfig.Master ? 1 : 0, adConfig.Target);
				_delegatePollingTimer.Enabled = true;
			}
		}
		
		override public void Destroy ()
		{
			Debug.Log ("iOSNativeAdView > Destroy()");
			if (_currentAdView != -1) {
				_ReleaseAdView (_currentAdView);
			}
		}
		
		override public int GetDefaultAdLoadingTimeout ()
		{
			Debug.Log ("iOSNativeAdView > GetDefaultAdLoadingTimeout()");
			return -1; // NOT YET IMPLEMENTED
		}
		
		override public void SetDefaultAdLoadingTimeout (int timeout)
		{
			Debug.Log ("iOSNativeAdView > SetDefaultAdLoadingTimeout(" + timeout + ")");
			// NOT YET IMPLEMENTED
		}
		
		override public bool GetIsLoggingEnabled ()
		{
			Debug.Log ("iOSNativeAdView > GetIsLoggingEnabled()");
			return false; // NOT YET IMPLEMENTED
		}
		
		override public void SetIsLoggingEnabled (bool enableLogging)
		{
			Debug.Log ("iOSNativeAdView > SetIsLoggingEnabled(" + enableLogging + ")");
			// NOT YET IMPLEMENTED
		}
		
		override public void DisplayBanner (AdPosition adPosition)
		{
			Debug.Log ("iOSNativeAdView > DisplayBanner(" + adPosition + ")");
			_DisplayBanner (_currentAdView, (int)adPosition);
		}

		/// <summary>
		/// Timer called until either the success or fail delegate are called in the native wrapper.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="e">Arguments.</param>
		private void OnDelegateTimerEvent(object source, ElapsedEventArgs e) 
		{
			// Checking in the native wrapper if a delegate has been called
			bool isLoaded = _CheckForLoadedDelegate (_currentAdView) == 1 ? true : false;
			bool isFailed = _CheckForFailedDelegate (_currentAdView) == 1 ? true : false;

			// In this case, the timer is stopped and the event handler notified.
			if (isLoaded || isFailed) {
				Debug.Log ("iOSNativeAdView > OnDelegateTimerEvent() > Loading complete");
				_delegatePollingTimer.Enabled = false;

				if (isLoaded) {
					NotifyLoadingSuccess ();
				} else {
					NotifyLoadingFailure ();
				}
			}
		}

	}
#endif
}