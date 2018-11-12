using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// A HttpProxyConnection used to record traffic
	/// </summary>
	public class ManualExploreProxyConnection : BaseProxyConnection
	{
		/// <summary>
		/// Constructor for the manual proxy connection
		/// </summary>
		/// <param name="tcpClient">TCP client being used</param>
		/// <param name="isSecure">Whether the connection is secure or not</param>
		/// <param name="dataStore">The data store being used</param>
		/// <param name="description">Description to add to the data store for each request</param>
		/// <param name="networkSettings">Network settings to be used for the connection</param>
		public ManualExploreProxyConnection(TcpClient tcpClient, 
			bool isSecure, 
			ITrafficDataAccessor dataStore, 
			string description, 
			INetworkSettings networkSettings)
			: base(tcpClient, isSecure, dataStore, description)
		{
			_httpClient = new WebRequestClient();
			_httpClient.SetNetworkSettings(networkSettings);
		}

		IHttpClient _httpClient;
		/// <summary>
		/// Gets the http client to be used for sending requests
		/// </summary>
		protected override IHttpClient HttpClient
		{
			get { return _httpClient; } 
		}

		/// <summary>
		/// Remove request headers that can cause issues with the recording
		/// </summary>
		/// <param name="reqInfo">The request info to be processed</param>
		/// <param name="isNonEssential">Whether this has deemed to be non-essential</param>
		/// <returns></returns>
		protected override HttpRequestInfo ProcessHeaders(HttpRequestInfo reqInfo, bool isNonEssential)
		{
			
			if (!isNonEssential) //prevent caching
			{
				reqInfo.Headers.Remove("Accept-Encoding"); //remove accept encoding to prevent gzip responses
				reqInfo.Headers.Remove("If-Modified-Since");
				reqInfo.Headers.Remove("If-None-Match");
			}
			return base.ProcessHeaders(reqInfo, isNonEssential);
		}



        
	}
}
