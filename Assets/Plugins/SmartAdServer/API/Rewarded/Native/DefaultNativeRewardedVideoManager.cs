using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public class DefaultNativeRewardedVideoManager : NativeRewardedVideoManager {

		override public void LoadRewardedVideo(AdConfig adConfig) {
			Debug.Log ("DefaultNativeRewardedVideoManager > loadRewardedVideo(" + adConfig + ")");
		}

		override public void ShowRewardedVideo(AdConfig adConfig) {
			Debug.Log ("DefaultNativeRewardedVideoManager > showRewardedVideo(" + adConfig + ")");
		}

		override public bool HasRewardedVideo(AdConfig adConfig) {
			Debug.Log ("DefaultNativeRewardedVideoManager > hasRewardedVideo(" + adConfig + ")");
			return false;
		}

	}
}
