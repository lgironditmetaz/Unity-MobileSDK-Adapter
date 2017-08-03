using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public class AndroidNativeRewardedVideoManager : NativeRewardedVideoManager {

		override public void LoadRewardedVideo(AdConfig adConfig) {
			Debug.Log ("AndroidNativeRewardedVideoManager > loadRewardedVideo(" + adConfig + ")");
			// TODO
		}

		override public void ShowRewardedVideo(AdConfig adConfig) {
			Debug.Log ("AndroidNativeRewardedVideoManager > showRewardedVideo(" + adConfig + ")");
			// TODO
		}

		override public bool HasRewardedVideo(AdConfig adConfig) {
			Debug.Log ("AndroidNativeRewardedVideoManager > hasRewardedVideo(" + adConfig + ")");
			// TODO
			return false;
		}

	}
}
