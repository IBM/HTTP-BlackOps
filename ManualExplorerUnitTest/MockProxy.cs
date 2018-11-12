using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;

namespace ManualExplorerUnitTest
{
	public class MockProxy : BaseProxy
	{
		ITrafficDataAccessor _trafficDataStore;
		/// <summary>
		/// The data store that requests will be added to
		/// </summary>
		public ITrafficDataAccessor TrafficDataStore
		{
			get { return _trafficDataStore; }
		}

		ITrafficDataAccessor _mockSite;
		/// <summary>
		/// The mock site that will be used to simulate traffic
		/// </summary>
		public ITrafficDataAccessor MockSite
		{
			get { return _mockSite; }
		}


		private IProxyConnection _currentConnection;
		/// <summary>
		/// Holds a reference to the last connection added
		/// </summary>
		public IProxyConnection CurrentConnection
		{
			get { return _currentConnection; }
		}


        public MockProxy()
            : this(new TrafficViewerFile(), new TrafficViewerFile())
        { 
        
        }

		/// <summary>
		/// Creates the proxy with a single request and response
		/// </summary>
		/// <param name="testRequest"></param>
		/// <param name="testResponse"></param>
		public MockProxy(string testRequest, string testResponse):this(new TrafficViewerFile(), new TrafficViewerFile())
		{
			_mockSite.AddRequestResponse(testRequest, testResponse);
		}
				
		/// <summary>
		/// Create the proxy on local host on whatever port is open
		/// </summary>
		public MockProxy(ITrafficDataAccessor trafficDataStore, ITrafficDataAccessor mockSite) : this(trafficDataStore, mockSite, "127.0.0.1", 0, 0) 
		{ 
		
		}

		/// <summary>
		/// Create the proxy on specific ports
		/// </summary>
		public MockProxy(ITrafficDataAccessor trafficDataStore, ITrafficDataAccessor mockSite, string host, int port, int securePort) 
			: base("127.0.0.1", port, securePort) 
		{
			_trafficDataStore = trafficDataStore;
			_mockSite = mockSite;
		}

		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			_currentConnection = new MockProxyConnection(clientInfo.Client, clientInfo.IsSecure, _trafficDataStore, "Mock request", _mockSite);
			return _currentConnection;
		}
	}
}
