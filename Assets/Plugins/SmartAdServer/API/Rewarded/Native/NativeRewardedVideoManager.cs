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
		/// Occurs when the rewarded video is successfully loaded.
		/// </summary>
		public event EventHandler RewardedVideoLoadingSuccess;

		/// <summary>
		/// Occurs when the rewarded video loading fails.
		/// </summary>
		public event EventHandler RewardedVideoLoadingFailure;

		/// <summary>
		/// Occurs when the rewarded video cannot play the video.
		/// </summary>
		public event EventHandler RewardedVideoPlaybackFailure;

		/// <summary>
		/// Occurs when the rewarded video receives a reward after a completed video playback.
		/// </summary>
		public event EventHandler RewardedVideoRewardReceived;

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

		/// <summary>
		/// Event called when the rewarded video is successfully loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		protected void NotifyRewardedVideoLoadingSuccess(EventArgs e)
		{
			if (RewardedVideoLoadingSuccess != null) {
				RewardedVideoLoadingSuccess (this, e);
			}
		}

		/// <summary>
		/// Event called when the rewarded video loading fails.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		protected void NotifyRewardedVideoLoadingFailure(EventArgs e)
		{
			if (RewardedVideoLoadingFailure != null) {
				RewardedVideoLoadingFailure (this, e);
			}
		}

		/// <summary>
		/// Event called when the rewarded video cannot play the video.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		protected void NotifyRewardedVideoPlaybackFailure(EventArgs e)
		{
			if (RewardedVideoPlaybackFailure != null) {
				RewardedVideoPlaybackFailure (this, e);
			}
		}

		/// <summary>
		/// Event called when the rewarded video receives a reward after a completed video playback.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event argument.</param>
		protected void NotifyRewardedVideoRewardReceived(EventArgs e)
		{
			if (RewardedVideoRewardReceived != null) {
				RewardedVideoRewardReceived (this, e);
			}
		}

	}
}
