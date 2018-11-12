using System;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Interface fot the proxy connection
	/// </summary>
	public interface IProxyConnection
	{
		/// <summary>
		/// Whether the connection and stream are still active
		/// </summary>
		bool Closed { get; }
		/// <summary>
		/// Whether the connection is processing a request
		/// </summary>
		bool IsBusy { get; }
		/// <summary>
		/// Starts listening to bytes coming on the connection stream
		/// </summary>
		void Start();
		/// <summary>
		/// Stops the connection closing the associated network resources
		/// </summary>
		void Stop();
	}
}
