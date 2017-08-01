using UnityEngine;
using System.Collections;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.UI.Native;
using SmartAdServer.Unity.Library.Factory;

namespace SmartAdServer.Unity.Library.UI
{
	/// <summary>
	/// This class represents an interstitial view.
	/// </summary>
	public class InterstitialView : AdView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SmartAdServer.Unity.Library.UI.InterstitialView"/> class.
		/// </summary>
		public InterstitialView () : base()
		{
		}

		/// <summary>
		/// Creates the native view.
		/// This method should only be called by the base class.
		/// </summary>
		override protected void CreateNativeView ()
		{
			NativeAdView = PlatformFactory.Instance.BuildInstance<NativeAdView> (AdType.Interstitial);
		}
	}
}
