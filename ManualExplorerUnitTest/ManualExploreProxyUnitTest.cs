using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Net;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class ManualExploreProxyUnitTest
	{
		[TestMethod]
		public void Test_NetworkSettings_ProxyUsesAProxy()
		{
			MockProxy mockProxy;
			string testRequest = "GET http://site.com/ HTTP/1.1\r\n\r\n";
			string testResponse = "HTTP/1.1 200 OK\r\n\r\n";

			TrafficViewerFile dataStore = new TrafficViewerFile();

			mockProxy = SetupMockProxy(testRequest, testResponse, dataStore);


			mockProxy.Start();

			

			ManualExploreProxy meProxy = new ManualExploreProxy("127.0.0.1", 0, null); //use a random port
			meProxy.NetworkSettings.WebProxy = new WebProxy(mockProxy.Host, mockProxy.Port);
			meProxy.Start();

			WebRequestClient client = new WebRequestClient();
			INetworkSettings networkSettings = new DefaultNetworkSettings();
			
			networkSettings.WebProxy = new WebProxy(meProxy.Host,meProxy.Port);

			client.SetNetworkSettings(networkSettings);

			HttpRequestInfo testReqInfo = new HttpRequestInfo(testRequest);

			Assert.AreEqual(0, dataStore.RequestCount);

			HttpResponseInfo respInfo = client.SendRequest(testReqInfo);


			meProxy.Stop();
			mockProxy.Stop();

			//test that the request goes through the mock proxy by checking the data store
			
			Assert.AreEqual(200, respInfo.Status);
			Assert.AreEqual(1, dataStore.RequestCount);
		}

		

		private static MockProxy SetupMockProxy(string testRequest, string testResponse, TrafficViewerFile dataStore)
		{
			MockProxy mockProxy;
			TrafficViewerFile mockSite = new TrafficViewerFile();
			mockSite.AddRequestResponse(testRequest, testResponse);

			mockProxy = new MockProxy(dataStore, mockSite);
			return mockProxy;
		}


		
	}
}
