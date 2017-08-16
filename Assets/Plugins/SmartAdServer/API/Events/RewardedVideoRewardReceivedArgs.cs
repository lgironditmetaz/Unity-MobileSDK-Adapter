using System;
using SmartAdServer.Unity.Library.Models;

namespace SmartAdServer.Unity.Library.Events
{
	public class RewardedVideoRewardReceivedArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SmartAdServer.Unity.Library.Events.RewardedVideoRewardReceivedArgs"/> class.
		/// </summary>
		/// <param name="adConfig">The ad config corresponding to the placement that has triggered the event.</param>
		/// <param name="currency">The currency of the reward (e.g coins, lives, points…).</param>
		/// <param name="amount">The amount of currency to be rewarded.</param>
		public RewardedVideoRewardReceivedArgs (AdConfig adConfig, String currency, Double amount) : base()
		{
			this.AdConfig = adConfig;
			this.Currency = currency;
			this.Amount = amount;
		}

		/// <summary>
		/// The ad config corresponding to the placement that has triggered the event.
		/// </summary>
		/// <value>The ad config corresponding to the placement that has triggered the event.</value>
		public AdConfig AdConfig { get; set; }

		/// <summary>
		/// Let you know which currency should be rewarded. Useful to know what kind of behavior to trigger in your code if you use different currencies.
		/// </summary>
		/// <value>The currency of the reward (e.g coins, lives, points…).</value>
		public String Currency { get; set; }

		/// <summary>
		/// The amount of currency to be rewarded.
		/// </summary>
		/// <value>The amount of currency to be rewarded.</value>
		public Double Amount { get; set; }
	}
}
