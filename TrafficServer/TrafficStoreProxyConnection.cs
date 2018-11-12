using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using System.Net.Sockets;

namespace TrafficServer
{
	public class TrafficStoreProxyConnection : BaseProxyConnection
	{
		TrafficStoreHttpClient _httpClient;

		/// <summary>
		/// Creates a proxy connection that retrieves data from a traffic data store
		/// </summary>
		/// <param name="sourceStore">The store to obtain the data from</param>
		/// <param name="matchMode">The way requests will be matched</param>
		/// <param name="ignoreAuth">Whether to ignore authentication</param>
		/// <param name="client">TCP client for the connection</param>
		/// <param name="isSecure">Whether to secure the stream</param>
		/// <param name="saveStore">Data store to seve information to</param>
		/// <param name="requestDescription">Description to be used for the request in the save store</param>
		public TrafficStoreProxyConnection(
			ITrafficDataAccessor sourceStore, 
			TrafficServerMode matchMode,
			bool ignoreAuth,
			TcpClient client, 
			bool isSecure,
			ITrafficDataAccessor saveStore, 
			string requestDescription
		)
			: base(client, isSecure, saveStore, requestDescription) 
		{
			_httpClient = new TrafficStoreHttpClient(sourceStore, matchMode, ignoreAuth);
		}

		protected override IHttpClient HttpClient
		{
			get { return _httpClient; }
		}
	}
}
