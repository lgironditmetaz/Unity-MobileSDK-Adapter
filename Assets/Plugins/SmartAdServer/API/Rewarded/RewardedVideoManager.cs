using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmartAdServer.Unity.Library.Rewarded.Native;
using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Factory;

namespace SmartAdServer.Unity.Library.Rewarded
{
	public class RewardedVideoManager {
		
		private static readonly RewardedVideoManager _instance = new RewardedVideoManager();

		private RewardedVideoManager _builder;

		private NativeRewardedVideoManager _nativeRewardedVideoManager;

		private RewardedVideoManager() {
			_nativeRewardedVideoManager = PlatformFactory.Instance.BuildInstance<NativeRewardedVideoManager> ();
		}

		public static RewardedVideoManager Instance
		{
			get
			{
				return _instance; 
			}
		}

		public void LoadRewardedVideo(AdConfig adConfig) {
			_nativeRewardedVideoManager.LoadRewardedVideo (adConfig);
		}

		public void ShowRewardedVideo(AdConfig adConfig) {
			_nativeRewardedVideoManager.ShowRewardedVideo (adConfig);
		}

		public bool HasRewardedVideo(AdConfig adConfig) {
			return false;
		}
		
	}
}
