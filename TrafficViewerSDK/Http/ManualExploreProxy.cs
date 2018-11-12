using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Creates a http proxy to be used for traffic recording purposes
	/// </summary>
	public class ManualExploreProxy : BaseProxy
	{

		ITrafficDataAccessor _trafficDataStore;
		/// <summary>
		/// Gets the current traffic data store
		/// </summary>
		public ITrafficDataAccessor TrafficDataStore
		{
			get { return _trafficDataStore; }
		}

		/// <summary>
		/// Creates a http proxy to be used for traffic recording purposes
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="dataStore">Where the traffic data will be stored</param>
		/// <param name="forcePort">Whether to enforce the specified port. By default the proxy will try the next available port</param>
		public ManualExploreProxy(string host, int port, ITrafficDataAccessor dataStore, bool forcePort = false) : base(host, port, 0, forcePort) //the secure port will be set to 0 since is not something we plan to support
		{
			_trafficDataStore = dataStore;
		}

		/// <summary>
		/// Gets the proxy connection to be used when a request comes in
		/// </summary>
		/// <param name="clientInfo">Information about the tcp client</param>
		/// <returns></returns>
		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new ManualExploreProxyConnection(clientInfo.Client, clientInfo.IsSecure, _trafficDataStore, Resources.ManualExploreRequestDescription, NetworkSettings);
		}
	}
}
