using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartAdServer.Unity.Library.Factory
{
	/// <summary>
	/// Abstract native instances builder.
	/// </summary>
	public interface PlatformBuilder
	{
		/// <summary>
		/// Build an instance of a native class for the current platform.
		/// </summary>
		/// <returns>The newly built instance.</returns>
		/// <param name="constructorParameters">Parameters that must be sent to the constructor.</param>
		/// <typeparam name="T">The base type of the instance that will be built. For instance, using NativeAdView 
		/// will build a DefaultNativeAdView, an AndroidNativeAdView or a iOSNativeAdView depending of the current platform.</typeparam>
		T BuildInstance<T> (params object[] constructorParameters) where T : class;
	}
}
