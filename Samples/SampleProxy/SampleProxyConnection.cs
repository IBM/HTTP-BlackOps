using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace SampleProxy
{
    public class SampleProxyConnection:BaseProxyConnection
    {
        private WebRequestClient _httpClient;
        /// <summary>
		/// Constructor for the manual proxy connection
		/// </summary>
		/// <param name="tcpClient">TCP client being used</param>
		/// <param name="isSecure">Whether the connection is secure or not</param>
		/// <param name="dataStore">The data store being used</param>
		/// <param name="description">Description to add to the data store for each request</param>
		/// <param name="networkSettings">Network settings to be used for the connection</param>
        public SampleProxyConnection(TcpClient tcpClient, 
			bool isSecure, 
			ITrafficDataAccessor dataStore, 
			string description, 
			INetworkSettings networkSettings)
			: base(tcpClient, isSecure, dataStore, description)
		{
			_httpClient = new WebRequestClient();
			_httpClient.SetNetworkSettings(networkSettings);
		}

        

        protected override IHttpClient HttpClient
        {
            get { return new WebRequestClient(); }
        }
    }
}
