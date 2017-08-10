using System;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public abstract class NativeRewardedVideoManager
	{

		public abstract void LoadRewardedVideo (AdConfig adConfig);

		public abstract void ShowRewardedVideo(AdConfig adConfig);

		public abstract void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback);

	}
}
