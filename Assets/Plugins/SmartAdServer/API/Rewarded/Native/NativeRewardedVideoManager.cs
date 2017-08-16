using System;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	/// <summary>
	/// Abstract class presenting a native rewarded video manager.
	/// 
	/// The native rewarded video manager is a manager class handled by a native SDK and is 
	/// only valid for one particular platform type, thus there should be one 
	/// NativeRewardedVideoManager subclass per supported platform.
	/// </summary>
	public abstract class NativeRewardedVideoManager
	{
		/// <summary>
		/// Loads a rewarded video interstitial for a given placement.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		public abstract void LoadRewardedVideo (AdConfig adConfig);

		/// <summary>
		/// Shows a rewarded video interstitial for a given placement if available.
		/// Rewarded video availability for a placement can be checked using the method <see cref="SmartAdServer.Unity.Library.Rewarded.RewardedVideoManager.CheckRewardedVideoAvailability"/>.
		/// </summary>
		/// <param name="adConfig">The ad config representing the placement.</param>
		public abstract void ShowRewardedVideo(AdConfig adConfig);

		/// <summary>
		/// Checks if a rewarded video interstitial is available for for a given placement.
		/// </summary>
		/// <returns><c>true</c> if a rewarded video interstitial is available for this adConfig; otherwise, <c>false</c>.</returns>
		/// <param name="adConfig">The ad config representing the placement.</param>
		public abstract bool HasRewardedVideo(AdConfig adConfig);

	}
}
