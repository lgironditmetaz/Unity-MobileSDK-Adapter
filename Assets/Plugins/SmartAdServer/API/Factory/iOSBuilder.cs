using System;
using SmartAdServer.Unity.Library.UI.Native;
using SmartAdServer.Unity.Library.Models;

namespace SmartAdServer.Unity.Library.Factory
{
	#if UNITY_IOS
	/// <summary>
	/// Class responsible for building native instances for iOS.
	/// </summary>
	public class iOSBuilder : PlatformBuilder
	{
		/// <summary>
		/// Build an instance of a native class for the current platform.
		/// </summary>
		/// <returns>The newly built instance.</returns>
		/// <param name="constructorParameters">Parameters that must be sent to the constructor.</param>
		/// <typeparam name="T">The base type of the instance that will be built. For instance, using NativeAdView 
		/// will build a DefaultNativeAdView, an AndroidNativeAdView or a iOSNativeAdView depending of the current platform.</typeparam>
		public T BuildInstance<T> (params object[] constructorParameters) where T : class {
			if (typeof(T) == typeof(NativeAdView)) {
				return new iOSNativeAdView ((AdType)constructorParameters[0]) as T;
			} else {
				throw new NativeClassNotFoundException ("Native class '" + typeof(T).ToString() + "' cannot be found!");
			}
		}
	}
	#endif
}
