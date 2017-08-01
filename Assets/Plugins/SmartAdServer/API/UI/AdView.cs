using UnityEngine;
using System.Collections;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.UI.Native;
using System;

namespace SmartAdServer.Unity.Library.UI
{
	/// <summary>
	/// This class represents an ad view.
	/// 
	/// The main logic of the ad logic is similar between a banner and an interstitial and is handled
	/// this class.
	/// It acts as a base class for BannerView or InterstitialView and cannot be instantiated.
	/// </summary>
	public abstract class AdView
	{
		/// <summary>
		/// Occurs when the ad view is successfully loaded.
		/// </summary>
		public event EventHandler AdViewLoadingSuccess;

		/// <summary>
		/// Occurs when the ad view has failed to load.
		/// </summary>
		public event EventHandler AdViewLoadingFailure;

		/// <summary>
		/// Occurs when ad view received a reward after a completed video playback.
		/// </summary>
		public event EventHandler AdViewRewardReceived;

		/// <summary>
		/// Instance representing a native ad view.
		/// </summary>
		protected NativeAdView NativeAdView;

		/// <summary>
		/// Creates the native view.
		/// This method should not be called outside of the GetNativeAdView() method.
		/// </summary>
		protected abstract void CreateNativeView ();

		////////////////////////////////////
		// Public API
		////////////////////////////////////

		/// <summary>
		/// Request an ad.
		/// </summary>
		/// <param name="adConfig">The configuration of the ad call.</param>
		public void LoadAd (AdConfig adConfig)
		{
			GetNativeAdView ().LoadAd (adConfig);
		}

		/// <summary>
		/// Destroy the AdView instance.
		/// This method must be called when the ad view is not used anymore to avoid crashes.
		/// </summary>
		public void Destroy ()
		{
			GetNativeAdView ().Destroy ();
		}

		/// <summary>
		/// Handle the ad call timeout.
		/// </summary>
		/// <value>Ad call loading timeout.</value>
		public int DefaultAdLoadingTimeout {
			get {
				return GetNativeAdView ().GetDefaultAdLoadingTimeout ();
			}
			set {
				GetNativeAdView ().SetDefaultAdLoadingTimeout (value);
			}
		}

		/// <summary>
		/// Enable or disable native SDK logging.
		/// </summary>
		/// <value><c>true</c> to activate SDK logging; otherwise, <c>false</c>.</value>
		public bool IsLoggingEnabled {
			get {
				return GetNativeAdView ().GetIsLoggingEnabled ();
			}
			set {
				GetNativeAdView ().SetIsLoggingEnabled (value);
			}
		}

		/// <summary>
		/// Show or hide an ad view.
		/// </summary>
		/// <param name="visible"><c>true</c> to show the ad view, <c>false</c> to hide it.</param>
		public void SetVisible(bool visible)
		{
			GetNativeAdView ().SetVisible (visible);
		}

		
		////////////////////////////////////
		// Private fields
		////////////////////////////////////

		// Native SDK events

		/// <summary>
		/// Event called when the ad call is successful.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		void NativeAdViewLoadingSuccess (object sender, EventArgs e)
		{
			Debug.Log ("SmartAdServer.Unity.Library.UI.AdView: NativeAdViewLoadingSuccess");
			if (AdViewLoadingSuccess != null) {
				AdViewLoadingSuccess (this, e);
			}
		}

		/// <summary>
		/// Event called when the ad call is failed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		void NativeAdViewLoadingFailure (object sender, EventArgs e)
		{
			Debug.Log ("SmartAdServer.Unity.Library.UI.AdView: NativeAdViewLoadingFailure");
			if (AdViewLoadingFailure != null) {
				AdViewLoadingFailure (this, e);
			}
		}

		/// <summary>
		/// Event called when the ad has sent a reward for the user
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">The description of the reward (currency, amount).</param>
		void NativeAdViewRewardReceived(object sender, EventArgs e)
		{
			Debug.Log ("SmartAdServer.Unity.Library.UI.AdView: NativeAdViewRewardReceived");
			if (AdViewRewardReceived != null) {
				AdViewRewardReceived (this, e);
			}
		}

		
		////////////////////////////////////
		// NativeAdView management
		////////////////////////////////////

		/// <summary>
		/// Gets the native ad view and instantiate it if necessary.
		/// </summary>
		/// <returns>The native ad view.</returns>
		protected NativeAdView GetNativeAdView ()
		{
			if (NativeAdView == null) {
				CreateNativeView ();

				// Registering events
				NativeAdView.NativeAdViewLoadingSuccess += NativeAdViewLoadingSuccess;
				NativeAdView.NativeAdViewLoadingFailure += NativeAdViewLoadingFailure;
				NativeAdView.NativeAdViewRewardReceived += NativeAdViewRewardReceived;
			}
			return NativeAdView;
		}
	}
}
