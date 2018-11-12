using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpClientMiscUnitTest:BaseHttpClientRequestTest
	{


		[TestMethod]
		public void Test_HTTP_Redirect()
		{

			string testRequest = "GET http://site.com/ HTTP/1.1\r\n";
			string expectedResponse = "HTTP/1.1 302 Redirect\r\nLocation: http://site.com/a\r\n\r\n";

			HttpResponseInfo respInfo = GetResponseFromMockProxy(testRequest, expectedResponse);

			Assert.AreEqual(302, respInfo.Status);

		}


		[TestMethod]
		public void Test_HTTP_ResponseHeaders()
		{

			string testRequest = "GET http://site.com/ HTTP/1.1\r\n";
			string expectedResponse = "HTTP/1.1 302 Redirect\r\nSet-Cookie: a=1\r\nSet-Cookie: b=2\r\nContent-length: 0\r\n";

			HttpResponseInfo respInfo = GetResponseFromMockProxy(testRequest, expectedResponse);

			List<HTTPHeader> headers = respInfo.Headers.GetHeaders("Set-Cookie");

			Assert.AreEqual(2, headers.Count);

			headers = respInfo.Headers.GetHeaders("Content-length");

			Assert.AreEqual(1, headers.Count);
		}


		//testing that reusing the same client for two requests works
		[TestMethod]
		public void Test_HTTP_ReusingClient()
		{
			string testRequest1 = "GET http://site.com/ HTTP/1.1\r\n\r\n";
			string testResponse1 = "HTTP/1.1 302 Redirect\r\nLocation: http://site.com/a\r\n\r\n";
			string testRequest2 = "GET http://site.com/a HTTP/1.1\r\n\r\n";
			string testResponse2 = "HTTP/1.1 200 OK\r\n\r\n";

			WebRequestClient client = new WebRequestClient();

			TrafficViewerFile mockSite = new TrafficViewerFile();
			mockSite.AddRequestResponse(testRequest1, testResponse1);
			mockSite.AddRequestResponse(testRequest2, testResponse2);
			TrafficViewerFile dataStore = new TrafficViewerFile();
			MockProxy mockProxy = new MockProxy(dataStore, mockSite);
			mockProxy.Start();

			client.SetProxySettings(mockProxy.Host, mockProxy.Port, null);

			CheckResponseStatus(testRequest1, client, 302);
			CheckResponseStatus(testRequest2, client, 200);

			mockProxy.Stop();
		}

        [TestMethod]
        public void Test_HTTP_WebRequestClient_Cookies()
        {
            string[] testRequestList = new string[5];
            string[] testResponseList = new string[5];
            testRequestList[0] = "GET http://site.com/a/1 HTTP/1.1\r\n\r\n";
            testResponseList[0] = "HTTP/1.1 302 Redirect\r\nSet-Cookie:a=1; Path=/a\r\nLocation: http://site.com/a\r\n\r\n";
            testRequestList[1] = "GET http://site.com/a/2 HTTP/1.1\r\n\r\n";
            testResponseList[1] = "HTTP/1.1 302 OK\r\n\r\n";
            testRequestList[2] = "GET http://site.com/b HTTP/1.1\r\nCookie:b=2\r\n\r\n";
            testResponseList[2] = "HTTP/1.1 302 OK\r\n\r\n";
            testRequestList[3] = "GET http://site.com/a/3 HTTP/1.1\r\n\r\n";
            testResponseList[3] = "HTTP/1.1 302 Redirect\r\nSet-Cookie:a=2; Path=/a; Expires=Thu, 01-Jan-1970 00:00:01 GMT;\r\nLocation: http://site.com/a\r\n\r\n";
            testRequestList[4] = "GET http://site.com/a/4 HTTP/1.1\r\n\r\n";
            testResponseList[4] = "HTTP/1.1 200 OK\r\n\r\n";

            WebRequestClient client = new WebRequestClient();
            client.ShouldHandleCookies = true;

            TrafficViewerFile mockSite = new TrafficViewerFile();
            for (int idx = 0; idx < testRequestList.Length; idx++)
            {
                mockSite.AddRequestResponse(testRequestList[idx], testResponseList[idx]);
            }
            
            TrafficViewerFile dataStore = new TrafficViewerFile();
            MockProxy mockProxy = new MockProxy(dataStore, mockSite);
            mockProxy.Start();

            client.SetProxySettings(mockProxy.Host, mockProxy.Port, null);
            for (int idx = 0; idx < testRequestList.Length; idx++)
            {
                client.SendRequest(new HttpRequestInfo(testRequestList[idx]));
            }

            //second request should have the extra cookie
            byte[] receivedRequestBytes = dataStore.LoadRequestData(1);//index starts from 0
            Assert.IsNotNull(receivedRequestBytes,"Missing second request");

            HttpRequestInfo receivedRequest = new HttpRequestInfo(receivedRequestBytes,true);

            Assert.IsNotNull(receivedRequest.Cookies);
            Assert.AreEqual(1, receivedRequest.Cookies.Count);
            Assert.IsTrue(receivedRequest.Cookies.ContainsKey("a"));

            //third request should not have the a cookie it's sent to /b but should have the b cookie
            receivedRequestBytes = dataStore.LoadRequestData(2);
            Assert.IsNotNull(receivedRequestBytes,"Missing third request");
            receivedRequest = new HttpRequestInfo(receivedRequestBytes, true);
            Assert.IsNotNull(receivedRequest.Cookies);
            Assert.AreEqual(1, receivedRequest.Cookies.Count,"Request to /b should have 1 cookie");
            Assert.IsTrue(receivedRequest.Cookies.ContainsKey("b"));

            //last request should have no cookies because the a cookie is expired
            receivedRequestBytes = dataStore.LoadRequestData(4);
            Assert.IsNotNull(receivedRequestBytes, "Missing fifth request");
            receivedRequest = new HttpRequestInfo(receivedRequestBytes, true);
            Assert.IsNotNull(receivedRequest.Cookies);
            Assert.AreEqual(0, receivedRequest.Cookies.Count, "Last request should have no cookies");
            

            mockProxy.Stop();
        }

	}
}
