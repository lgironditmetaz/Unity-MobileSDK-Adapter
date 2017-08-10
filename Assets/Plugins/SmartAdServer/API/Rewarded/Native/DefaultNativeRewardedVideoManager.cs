using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library;

namespace SmartAdServer.Unity.Library.Rewarded.Native
{
	public class DefaultNativeRewardedVideoManager : NativeRewardedVideoManager
	{

		override public void LoadRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > LoadRewardedVideo(" + adConfig + ")");
		}

		override public void ShowRewardedVideo(AdConfig adConfig)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > ShowRewardedVideo(" + adConfig + ")");
		}

		override public void CheckRewardedVideoAvailability(AdConfig adConfig, Action<bool> callback)
		{
			Debug.Log ("DefaultNativeRewardedVideoManager > CheckRewardedVideoAvailability(" + adConfig + ")");

			// The callback is called immediately with default value in this implementation so there is no need
			// for a special case in the user app for Default native rewarded video manager implementation
			callback (false); 
		}

	}
}
