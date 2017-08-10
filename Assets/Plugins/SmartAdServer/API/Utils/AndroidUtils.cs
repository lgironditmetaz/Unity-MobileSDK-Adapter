using System;
using UnityEngine;

using SmartAdServer.Unity.Library.Models;
using SmartAdServer.Unity.Library.Constants;

namespace SmartAdServer.Unity.Library.Utils
{
	/// <summary>
	/// Utils method to handle Android activities and threading.
	/// </summary>
	class AndroidUtils
	{
		/// <summary>
		/// The java object representing the activity displaying the game.
		/// </summary>
		private AndroidJavaObject _unityActivity;

		/// <summary>
		/// The unique instance of AndroidUtils.
		/// </summary>
		private static readonly AndroidUtils _instance = new AndroidUtils();

		/// <summary>
		/// Initializes a new instance of the <see cref="SmartAdServer.Unity.Library.Utils.AndroidUtils"/> class.
		/// This constructor is private since AndroidUtils is a singleton.
		/// </summary>
		private AndroidUtils() {
			_unityActivity = new AndroidJavaClass (JavaClass.UnityPlayer).GetStatic<AndroidJavaObject> (JavaMethod.CurrentActivity);
		}

		/// <summary>
		/// Gets the unique instance of AndroidUtils.
		/// </summary>
		/// <value>The instance.</value>
		public static AndroidUtils Instance
		{
			get
			{
				return _instance; 
			}
		}
		
		/// <summary>
		/// Runs some code on Android UI thread.
		/// </summary>
		/// <param name="method">The method to run on UI thread.</param>
		public void RunOnJavaUiThread (Action method)
		{
			GetUnityActivity ().Call (JavaMethod.RunOnUiThread, new AndroidJavaRunnable (method));
		}

		/// <summary>
		/// Gets the activity displaying the game.
		/// </summary>
		/// <returns>The activity displaying the game.</returns>
		public AndroidJavaObject GetUnityActivity ()
		{
			return _unityActivity;
		}

	}
}

