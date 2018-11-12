using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficServer;
using TrafficViewerSDK.Http;
using System.Net;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class ReverseProxyTest
	{
		//[TestMethod]
		public void Test_ReverseProxy()
		{
			string testRequest = "GET / HTTP/1.1\r\n";
			string site1Response = "HTTP/1.1 200 OK\r\n\r\nThis is site1";
			string site2Response = "HTTP/1.1 200 OK\r\n\r\nThis is site2";
			//create two mock sites each on a different port and a http client that send a request to the first but in fact gets redirected to the other
			TrafficViewerFile site1Source = new TrafficViewerFile();
			site1Source.AddRequestResponse(testRequest, site1Response);
			TrafficViewerFile site2Source = new TrafficViewerFile();
			site2Source.AddRequestResponse(testRequest, site2Response);

			TrafficStoreProxy mockSite1 = new TrafficStoreProxy(
				site1Source, null, "127.0.0.1", 0, 0);

			mockSite1.Start();

			TrafficStoreProxy mockSite2 = new TrafficStoreProxy(
				site2Source, null, "127.0.0.1", 0, 0);

			mockSite2.Start();

			HttpRequestInfo reqInfo = new HttpRequestInfo(testRequest);

			//request will be sent to site 1
			reqInfo.Host = mockSite1.Host;
			reqInfo.Port = mockSite1.Port;

			ReverseProxy revProxy = new ReverseProxy("127.0.0.1", 0, 0, null);
            revProxy.ExtraOptions[ReverseProxy.FORWARDING_HOST_OPT] = mockSite2.Host;
            revProxy.ExtraOptions[ReverseProxy.FORWARDING_PORT_OPT] = mockSite2.Port.ToString();
			revProxy.Start();

			//make an http client
			IHttpClient client = new WebRequestClient();
			DefaultNetworkSettings settings = new DefaultNetworkSettings();
			settings.WebProxy = new WebProxy(revProxy.Host, revProxy.Port);

			client.SetNetworkSettings(settings);

			//send the request Http and verify the target site received it

			HttpResponseInfo respInfo = client.SendRequest(reqInfo);
			string respBody = respInfo.ResponseBody.ToString();


			Assert.IsTrue(respBody.Contains("This is site2"));

			//check over ssl

			reqInfo.IsSecure = true;
			respInfo = client.SendRequest(reqInfo);
			respBody = respInfo.ResponseBody.ToString();
			Assert.IsTrue(respBody.Contains("This is site2"));

			mockSite1.Stop();
			mockSite2.Stop();
			revProxy.Stop();
		}
	}
}
