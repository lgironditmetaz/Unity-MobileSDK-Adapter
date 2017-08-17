using UnityEngine;
using System;
using System.Collections;

using SmartAdServer.Unity.Library.Constants;
using SmartAdServer.Unity.Library.Events;
using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Utils;

namespace SmartAdServer.Unity.Library.UI.Native
{
	#if UNITY_ANDROID
	/// <summary>
	/// Android native ad view implementation.
	/// </summary>
	public class AndroidNativeAdView : NativeAdView
	{
		/// <summary>
		/// NOT YET IMPLEMENTED.
		/// </summary>
		private bool _isLoggingEnabled = false;

		/// <summary>
		/// The current placement configuration.
		/// </summary>
		private AdConfig _currentAdConfig;

		/// <summary>
		/// The position where the ad needs to be displayed.
		/// </summary>
		private AdPosition _currentAdPosition;

		/// <summary>
		/// The java object representing the ad view.
		/// </summary>
		private AndroidJavaObject _adViewObject;

		/// <summary>
		/// The java class representing the ad view.
		/// </summary>
		private static AndroidJavaClass _adViewClass;

		/// <summary>
		/// The Android frame layout used to display the ad as an interstitial.
		/// </summary>
		private AndroidJavaObject _interstitialFrameLayout;


		////////////////////////////////////
		// Public overriden methods
		////////////////////////////////////

		public AndroidNativeAdView (AdType type) : base(type)
		{
			Debug.Log ("AndroidNativeAdView > DefaultNativeAdView(" + type + ")");
			AndroidUtils.Instance.RunOnJavaUiThread (InitializeBannerViewOnUiThread);
		}
		
		override public void LoadAd (AdConfig adConfig)
		{
			Debug.Log ("AndroidNativeAdView > LoadAd(" + adConfig + ")");
			_currentAdConfig = adConfig;
			AndroidUtils.Instance.RunOnJavaUiThread (LoadAdOnUiThread);
		}
		
		override public void Destroy ()
		{
			Debug.Log ("AndroidNativeAdView > Destroy()");
			AndroidUtils.Instance.RunOnJavaUiThread (DestroyOnUiThread);
		}
		
		override public int GetDefaultAdLoadingTimeout ()
		{
			Debug.Log ("AndroidNativeAdView > GetDefaultAdLoadingTimeout()");
			return -1; // NOT YET IMPLEMENTED
		}
		
		override public void SetDefaultAdLoadingTimeout (int timeout)
		{
			Debug.Log ("AndroidNativeAdView > SetDefaultAdLoadingTimeout(" + timeout + ")");
			// NOT YET IMPLEMENTED
		}
		
		override public bool GetIsLoggingEnabled ()
		{
			Debug.Log ("AndroidNativeAdView > GetIsLoggingEnabled()");
			return _isLoggingEnabled;
		}
		
		override public void SetIsLoggingEnabled (bool enableLogging)
		{
			Debug.Log ("AndroidNativeAdView > SetIsLoggingEnabled(" + enableLogging + ")");

			if (enableLogging) {
				AndroidJNIHelper.debug = true;
				GetAdViewClass ().CallStatic (JavaMethod.EnableLogging);
				_isLoggingEnabled = true;
			} else if (_isLoggingEnabled == true && enableLogging == false) {
				Debug.LogWarning ("Android SDK Debugging can't be deactivated once it has been enabled!");
			}
		}

		override public void SetVisible (bool visible)
		{
			Debug.Log ("AndroidNativeAdView > SetVisible(" + visible + ")");

			AndroidUtils.Instance.RunOnJavaUiThread (() => {
				SetAdViewVisibilityOnUiThread (visible);
			});
		}
		
		override public void DisplayBanner (AdPosition adPosition)
		{
			Debug.Log ("AndroidNativeAdView > DisplayBanner(" + adPosition + ")");
			_currentAdPosition = adPosition;
			AndroidUtils.Instance.RunOnJavaUiThread (AddBannerToHierarchyOnUiThread);
		}

		
		////////////////////////////////////
		// UI interaction methods
		////////////////////////////////////

		/// <summary>
		/// Instantiate the ad view object.
		/// </summary>
		void InitializeBannerViewOnUiThread ()
		{
			Debug.Log ("SmartAdServer.Unity.Library.UI.Native.AndroidNativeAdView: initializing AdView");

			GetAdViewClass().CallStatic (JavaMethod.SetUnityModeEnabled, true);
			_adViewObject = new AndroidJavaObject (Type == AdType.Banner ? JavaClass.SASBannerView : JavaClass.SASInterstitialView, AndroidUtils.Instance.GetUnityActivity ());

			if (Type == AdType.Interstitial) {
				var loader = new AndroidJavaObject (JavaClass.SASRotatingImageLoader, AndroidUtils.Instance.GetUnityActivity ());
				var blackColor = new AndroidJavaClass (JavaClass.Color).CallStatic<int> (JavaMethod.ParseColor, "#aa000000");
				loader.Call (JavaMethod.SetBackgroundColor, blackColor); // Set a black overlay for the loader

				_adViewObject.Call (JavaMethod.SetLoaderView, loader);
			}
		}

		/// <summary>
		/// Load an ad using the native SDK and set an AdResponseHandler.
		/// </summary>
		void LoadAdOnUiThread ()
		{
			Debug.Log ("SmartAdServer.Unity.Library.UI.Native.AndroidNativeAdView: loading ad…");

			// Setting the baseUrl
			new AndroidJavaClass(JavaClass.SASAdView).CallStatic (JavaMethod.SetBaseUrl, _currentAdConfig.BaseUrl);

			// Make the actual ad call with a listener to retrieve the response
			GetAdViewObject ().Call (
				JavaMethod.LoadAd,
				_currentAdConfig.SiteId,
				_currentAdConfig.PageId,
				_currentAdConfig.FormatId,
				_currentAdConfig.Master,
				_currentAdConfig.Target,
				new AdResponseHandler (this)
			);

			// Add a reward listener in case the ad wants to send one the user
			GetAdViewObject ().Call (
				JavaMethod.AddRewardListener,
				new OnRewardHandler (this)
			);
		}

		/// <summary>
		/// Destroy properly the current ad view object.
		/// </summary>
		void DestroyOnUiThread ()
		{
			GetAdViewObject ().Call (JavaMethod.OnDestroy);
		}

		/// <summary>
		/// Adds the view to the top or the bottom of the Unity activity.
		/// </summary>
		void AddBannerToHierarchyOnUiThread ()
		{
			var gravityString = _currentAdPosition == AdPosition.Top ? JavaFlag.GravityTop : JavaFlag.GravityBottom;

			int matchParentObject = -1; // MATCH_PARENT equals -1 according to https://developer.android.com/reference/android/view/ViewGroup.LayoutParams.html#MATCH_PARENT
			
			var density = AndroidUtils.Instance.GetUnityActivity ().Call<AndroidJavaObject> (JavaMethod.GetResources).Call<AndroidJavaObject> (JavaMethod.GetDisplayMetrics).Get<float> (JavaField.Density);

			var frameLayoutParamObject = new AndroidJavaObject (JavaClass.FrameLayoutLayoutParam, matchParentObject, (int)(50 * density));

			var gravity = new AndroidJavaClass (JavaClass.Gravity).GetStatic<int> (gravityString);
			var gravityCenterHorizontal = new AndroidJavaClass (JavaClass.Gravity).GetStatic<int> (JavaFlag.GravityCenterHorizontal);

			frameLayoutParamObject.Set (JavaField.Gravity, gravity | gravityCenterHorizontal);
			
			AndroidUtils.Instance.GetUnityActivity ().Call (JavaMethod.AddContentView, GetAdViewObject (), frameLayoutParamObject);
		}

		/// <summary>
		/// Sets the ad view visibility.
		/// </summary>
		/// <param name="visible">The ad view is visible if <c>true</c>.</param>
		void SetAdViewVisibilityOnUiThread (bool visible)
		{
			// Hide or show the banner
			var visibilityString = visible ? JavaFlag.Visible : JavaFlag.Invisible;
			var visibilityFlag = new AndroidJavaClass (JavaClass.View).GetStatic<int> (visibilityString);
			GetAdViewObject ().Call (JavaMethod.SetVisibility, visibilityFlag);

			// Add the banner to the hierarchy again to prevent an issue where the banner stays hidden
			if (visible) {
				DisplayBanner (_currentAdPosition);
			}
		}

		
		////////////////////////////////////
		// SDK ad response handler
		////////////////////////////////////

		/// <summary>
		/// Class that will act as a java listener to handle ad call success & failure.
		/// </summary>
		class AdResponseHandler : AndroidJavaProxy
		{
			AndroidNativeAdView callerAdView;

			public AdResponseHandler (AndroidNativeAdView adView) : base(JavaClass.SASAdViewAdResponseHandler)
			{
				callerAdView = adView;
			}
			
			void adLoadingCompleted (AndroidJavaObject adElement)
			{
				callerAdView.NotifyLoadingSuccess ();
			}
			
			void adLoadingFailed (AndroidJavaObject exception)
			{
				callerAdView.NotifyLoadingFailure ();
			}
		}

		class OnRewardHandler : AndroidJavaProxy
		{
			AndroidNativeAdView callerAdView;

			public OnRewardHandler (AndroidNativeAdView adView) : base(JavaClass.SASAdViewOnRewardListener)
			{
				callerAdView = adView;
			}

			void onReward (AndroidJavaObject reward)
			{
				// The reward needs to be converted into a C# event args before being sent back to the app
				Double amount = reward.Call<Double> (JavaMethod.GetAmount);
				String currency = reward.Call<String> (JavaMethod.GetCurrency);

				callerAdView.NotifyRewardReceived (new RewardReceivedEventArgs (currency, amount));
			}
		}

		/// <summary>
		/// Gets the java object representing the ad view.
		/// </summary>
		/// <returns>The java object representing the ad view.</returns>
		AndroidJavaObject GetAdViewObject ()
		{
			return _adViewObject;
		}

		/// <summary>
		/// Gets the java class representing the ad view.
		/// </summary>
		/// <returns>The java class representing the ad view.</returns>
		static AndroidJavaClass GetAdViewClass ()
		{
			if (_adViewClass == null) {
				_adViewClass = new AndroidJavaClass (JavaClass.SASAdView);
			}
			return _adViewClass;
		}
	}
	#endif
}
