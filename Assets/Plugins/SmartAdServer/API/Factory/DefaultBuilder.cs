using System;
using UnityEngine;

using SmartAdServer.Unity.Library.UI.Native;
using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Rewarded.Native;

namespace SmartAdServer.Unity.Library.Factory
{
	/// <summary>
	/// Class responsible for building native instances for unsupported platforms.
	/// </summary>
	public class DefaultBuilder : PlatformBuilder
	{
		/// <summary>
		/// Build an instance of a native class for the current platform.
		/// </summary>
		/// <returns>The newly built instance.</returns>
		/// <param name="constructorParameters">Parameters that must be sent to the constructor.</param>
		/// <typeparam name="T">The base type of the instance that will be built. For instance, using NativeAdView 
		/// will build a DefaultNativeAdView, an AndroidNativeAdView or a iOSNativeAdView depending of the current platform.</typeparam>
		public T BuildInstance<T> (params object[] constructorParameters) where T : class
		{
			if (typeof(T) == typeof(NativeAdView)) {
				return new DefaultNativeAdView ((AdType)constructorParameters[0]) as T;
			} else if (typeof(T) == typeof(NativeRewardedVideoManager)) {
				return new DefaultNativeRewardedVideoManager () as T;
			} else {
				Debug.Log ("DefaultBuilder > Unable to load native class '" + typeof(T).ToString() + "'");
				throw new NativeClassNotFoundException ("Native class '" + typeof(T).ToString() + "' cannot be found!");
			}
		}
	}
}
