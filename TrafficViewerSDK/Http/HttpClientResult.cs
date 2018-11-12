using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// The result of an http client send operation
	/// </summary>
	public enum HttpClientResult
	{
		/// <summary>
		/// Successful response
		/// </summary>
		Success,
		/// <summary>
		/// Communication error
		/// </summary>
		Error
	}
}
