using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerInstance;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficServer
{
	/// <summary>
	/// A reverse proxy that allows tracking of urls using unchanging aliases 
	/// </summary>
	public class TrackingReverseProxy:BaseProxy
	{
		private Dictionary<string, string> _replacements;
		private ITrafficDataAccessor _dataStore;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="forwardingHost">The hosts thar requests will be sent to</param>
		/// <param name="forwardingPort">The forwarding port</param>
		/// <param name="host">The host of the proxy</param>
		/// <param name="port">The port of the proxy</param>
		/// <param name="dataStore">Data store where the requests will be saved</param>
		public TrackingReverseProxy( string host, int port, int securePort, ITrafficDataAccessor dataStore) :
			base(host, port, securePort)
		{
			_dataStore = dataStore;
			PatternTracker.Instance.PatternsToTrack = dataStore.Profile.GetTrackingPatterns();
		}

		/// <summary>
		/// Gets a connection
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new TrackingReverseProxyConnection(clientInfo.Client, clientInfo.IsSecure, _dataStore, NetworkSettings, this);
		}
	}
}
