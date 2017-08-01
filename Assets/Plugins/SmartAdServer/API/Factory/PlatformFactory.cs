using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;

namespace SmartAdServer.Unity.Library.Factory
{
	/// <summary>
	/// Factory class used to build native classes depending on the current platform.
	/// </summary>
	public sealed class PlatformFactory
	{
		/// <summary>
		/// Private PlatformFactory instance.
		/// </summary>
		private static readonly PlatformFactory _instance = new PlatformFactory();

		/// <summary>
		/// Builder used by the factory.
		/// </summary>
		private PlatformBuilder _builder;

		/// <summary>
		/// Initializes the only instance of PlatformFactory.
		/// </summary>
		private PlatformFactory() {
			var platformType = PlatformType.Default;
			if (Application.platform == RuntimePlatform.Android) {
				platformType = PlatformType.Android;
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				platformType = PlatformType.iOS;
			}
			ConfigureBuilder (platformType);
		}

		/// <summary>
		/// Configures the builder used by the factory depending on the current platform.
		/// </summary>
		/// <param name="platform">Platform.</param>
		private void ConfigureBuilder (PlatformType platform)
		{
			switch (platform) {
			#if UNITY_ANDROID
			case PlatformType.Android:
				_builder = new AndroidBuilder ();
				break;
			#endif
			#if UNITY_IOS
			case PlatformType.iOS:
				_builder = new iOSBuilder ();
				break;
			#endif
			default:
				_builder = new DefaultBuilder ();
				break;
			}
		}

		/// <summary>
		/// Build an instance of a native class for the current platform.
		/// </summary>
		/// <returns>The newly built instance.</returns>
		/// <param name="constructorParameters">Parameters that must be sent to the constructor.</param>
		/// <typeparam name="T">The base type of the instance that will be built. For instance, using NativeAdView 
		/// will build a DefaultNativeAdView, an AndroidNativeAdView or a iOSNativeAdView depending of the current platform.</typeparam>
		public T BuildInstance<T> (params object[] constructorParameters) where T : class {
			return _builder.BuildInstance<T> (constructorParameters);
		}

		/// <summary>
		/// Get the PlatformFactory instance.
		/// </summary>
		/// <value>The PlatformFactory instance.</value>
		public static PlatformFactory Instance
		{
			get
			{
				return _instance; 
			}
		}

	}
}
