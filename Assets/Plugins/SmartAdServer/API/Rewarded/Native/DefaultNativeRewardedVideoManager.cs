using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public class DefaultNativeRewardedVideoManager : NativeRewardedVideoManager {

		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");
		}

		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");
		}

		override public bool HasRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > HasRewardedVideo(" + adConfig + ")");
			return false;
		}

	}
}
