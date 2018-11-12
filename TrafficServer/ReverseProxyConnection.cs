using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using System.Net.Sockets;
using TrafficViewerSDK;
using TrafficServer.Properties;

namespace TrafficServer 
{
	/// <summary>
	/// Proxy connection that forwards a request to a different site than the one it was intended for
	/// </summary>
	public class ReverseProxyConnection : AdvancedExploreProxyConnection
	{
		private string _fwHost;
		private int _fwPort;

		/// <summary>
		/// Creates a connection that will forward the request to the specified host and port
		/// </summary>
		/// <param name="fwHost"></param>
		/// <param name="fwPort"></param>
		/// <param name="client"></param>
		/// <param name="isSecure"></param>
		/// <param name="dataStore"></param>
		/// <param name="networkSettings"></param>
		public ReverseProxyConnection(string fwHost, int fwPort, TcpClient client, bool isSecure, ITrafficDataAccessor dataStore, INetworkSettings networkSettings, Dictionary<string, string> replacements)
			: base(client, isSecure, dataStore, Resources.ReverseProxyDescription, networkSettings, false)
		{
			_fwHost = fwHost;
			_fwPort = fwPort;
		}

        /// <summary>
        /// Secures the current connection stream
        /// </summary>
        /// <returns>True if the operation is successful</returns>
        protected override bool TrySecureStream()
        {
            
            bool success = ClientStreamWrapper.SecureStream(_fwHost);
            if (!success)
            {
                ClientStreamWrapper.Close();
            }
            return success;
        }

		

		protected override HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
		{
			//replace the host and port
			requestInfo.Host = _fwHost;
			requestInfo.Port = _fwPort;
			return base.OnBeforeRequestToSite(requestInfo);
		}


	}
}
