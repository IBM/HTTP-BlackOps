using System;
using System.Net;
namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Interface used for extending Traffic Viewer with a different HTTP client
	/// </summary>
	public interface IHttpClient : ITrafficViewerExtension
	{
		/// <summary>
		/// Sends the request
		/// </summary>
		/// <returns>Response or null in case of communication error</returns>
		HttpResponseInfo SendRequest(HttpRequestInfo requestInfo);

		/// <summary>
		/// Sets proxy, behavior when using ssl certificates and credentials for a specific host
		/// </summary>
		/// <param name="networkSettings"></param>
		void SetNetworkSettings(INetworkSettings networkSettings);
	}
}