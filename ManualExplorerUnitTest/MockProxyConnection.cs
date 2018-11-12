using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Net.Sockets;

namespace ManualExplorerUnitTest
{
	public class MockProxyConnection : BaseProxyConnection
	{
		ITrafficDataAccessor _mockSite;
		IHttpClient _httpClient;

		protected override IHttpClient HttpClient
		{
			get { return _httpClient; }
		}

		public MockProxyConnection(TcpClient client, bool isSecure, ITrafficDataAccessor dataStore, string description, ITrafficDataAccessor mockSite):base(client, isSecure, dataStore, description)
		{
			_mockSite = mockSite;
			_httpClient = new MockHttpClient(_mockSite);
		}

	}
}
