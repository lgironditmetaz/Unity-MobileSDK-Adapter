﻿using System;
using SmartAdServer.Unity.Library.Models;

namespace SmartAdServer.Unity.Library.Events
{
	public class RewardedVideoLoadingSuccessArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SmartAdServer.Unity.Library.Events.RewardedVideoLoadingSuccessArgs"/> class.
		/// </summary>
		/// <param name="adConfig">The ad config corresponding to the placement that has triggered the event.</param>
		public RewardedVideoLoadingSuccessArgs (AdConfig adConfig) : base()
		{
			this.AdConfig = adConfig;
		}

		/// <summary>
		/// The ad config corresponding to the placement that has triggered the event.
		/// </summary>
		/// <value>The ad config corresponding to the placement that has triggered the event.</value>
		public AdConfig AdConfig { get; set; }
	}
}
