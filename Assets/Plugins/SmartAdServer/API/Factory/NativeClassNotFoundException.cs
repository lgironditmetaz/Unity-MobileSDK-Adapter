using System;

namespace SmartAdServer.Unity.Library.Factory
{
	/// <summary>
	/// Exception thrown when the PlatformFactory is asked to build a native class that can't be found.
	/// </summary>
	[Serializable]
	public class NativeClassNotFoundException : Exception
	{
		public NativeClassNotFoundException ()
		{}

		public NativeClassNotFoundException (string message) : base(message)
		{}

		public NativeClassNotFoundException (string message, Exception innerException) : base (message, innerException)
		{} 
	}
}
