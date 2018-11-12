using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using ManualExplorerUnitTest.Properties;
using System.Net;
using System.Net.Sockets;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpBaseProxyUnitTest : BaseHttpClientRequestTest
	{


		[TestMethod]
		public void TestDataStoreHasRequestAndResponse()
		{
			TrafficViewerFile dataStore = new TrafficViewerFile();
			TrafficViewerFile mockSite = new TrafficViewerFile();
			string testRequest = "GET http://site.com/a HTTP/1.1\r\n";
			string expectedResponseLine = "HTTP/1.1 200 OK\r\n\r\n<body>";
			mockSite.AddRequestResponse(testRequest, expectedResponseLine);

			MockProxy proxy = new MockProxy(dataStore, mockSite);

			proxy.Start();

			IHttpClient httpClient = GetHttpClient(proxy.Port);

			HttpRequestInfo testRequestInfo = new HttpRequestInfo(testRequest);

			httpClient.SendRequest(testRequestInfo);

			byte[] testRequestBytes = dataStore.LoadRequestData(0); 
			byte[] testResponseBytes = dataStore.LoadResponseData(0);

			HttpRequestInfo reqInfo = new HttpRequestInfo(testRequestBytes);
			Assert.AreEqual(testRequestInfo.FullUrl, reqInfo.FullUrl);
			HttpResponseInfo respInfo = new HttpResponseInfo(testResponseBytes);
			Assert.AreEqual(200, respInfo.Status);
			Assert.AreEqual("<body>", respInfo.ResponseBody.ToString());

			proxy.Stop();
		}


		private void VerifyErrorResponseMessage(int expectedStatus, string expectedMessage, HttpResponseInfo response)
		{

			Assert.AreEqual(expectedStatus, response.Status);
			Assert.IsTrue(response.ResponseBody.ToString().Contains(expectedMessage));
		
		}

		

		
		[TestMethod]
		public void TestProxyStart()
		{
			MockProxy proxy = new MockProxy(null, null, "", 8888, 18888);
			proxy.Start();
			Assert.AreEqual(8888, proxy.Port);
			Assert.AreEqual(18888, proxy.SecurePort);
			proxy.Stop();
		}

		[TestMethod]
		public void TestGETRequestToProxy()
		{
			TrafficViewerFile dataStore = new TrafficViewerFile();
			TrafficViewerFile mockSite = new TrafficViewerFile();
			string testRequest = "GET http://site.com/a HTTP/1.1\r\n";
			string expectedResponseLine = "HTTP/1.1 200 OK";
			mockSite.AddRequestResponse(testRequest, expectedResponseLine);

			MockProxy proxy = new MockProxy(dataStore, mockSite);
	
			proxy.Start();

			IHttpClient httpClient = GetHttpClient(proxy.Port);

			HttpRequestInfo testRequestInfo = new HttpRequestInfo(testRequest);

			HttpResponseInfo respInfo = httpClient.SendRequest(testRequestInfo);

			Assert.AreEqual(200, respInfo.Status);
			

			proxy.Stop();
		}

		[TestMethod]
		public void TestPOSTRequestToProxy()
		{
			TrafficViewerFile dataStore = new TrafficViewerFile();
			TrafficViewerFile mockSite = new TrafficViewerFile();
		
			string expectedResponseLine = "HTTP/1.1 200 OK";
			mockSite.AddRequestResponse(Resources.POSTRequest, expectedResponseLine);

			MockProxy proxy = new MockProxy(dataStore, mockSite);

			proxy.Start();

			IHttpClient httpClient = GetHttpClient(proxy.Port);

			HttpRequestInfo testRequestInfo = new HttpRequestInfo(Resources.POSTRequest);

			HttpResponseInfo respInfo = httpClient.SendRequest(testRequestInfo);

			Assert.AreEqual(200, respInfo.Status);

			HttpRequestInfo storedRequestInfo = new HttpRequestInfo(mockSite.LoadRequestData(0));

			Assert.AreEqual("uid=jsmith&passwd=Demo1234", storedRequestInfo.ContentDataString);

			proxy.Stop();
		}


		[TestMethod]
		public void TestExclusions()
		{
			TrafficViewerFile dataStore = new TrafficViewerFile();
			dataStore.Profile.SetExclusions(new string[1] { @".*\.gif" });

			TrafficViewerFile mockSite = new TrafficViewerFile();
			string nonExcludedRequest = "GET http://site.com/a HTTP/1.1\r\n\r\n";
			string excludedRequest = "GET http://site.com/image.gif HTTP/1.1\r\n\r\n";
			string testResponse = "HTTP/1.1 200 OK";
			mockSite.AddRequestResponse(nonExcludedRequest, testResponse);
			mockSite.AddRequestResponse(excludedRequest, testResponse);

			MockProxy proxy = new MockProxy(dataStore, mockSite);

			proxy.Start();

			IHttpClient httpClient = GetHttpClient(proxy.Port);

			HttpRequestInfo testRequestInfo = new HttpRequestInfo(excludedRequest);

			HttpResponseInfo respInfo = httpClient.SendRequest(testRequestInfo);

			Assert.AreEqual(200, respInfo.Status);
			//verify that nothing was added to the file
			Assert.AreEqual(0, dataStore.RequestCount);

			//verify that when sending a request that is not excluded the request is being added

			testRequestInfo = new HttpRequestInfo(nonExcludedRequest);
			respInfo = httpClient.SendRequest(testRequestInfo);
			
			Assert.AreEqual(200, respInfo.Status);
			//verify that the request was added to the file
			Assert.AreEqual(1, dataStore.RequestCount);
			
			HttpRequestInfo savedReqInfo = new HttpRequestInfo(dataStore.LoadRequestData(0));
			Assert.AreEqual(testRequestInfo.FullUrl, savedReqInfo.FullUrl);

			proxy.Stop();
		}


		[TestMethod]
		public void TestRemovingCachedHeaders()
		{
			//setup a mock web server

			TrafficViewerFile serverdataStore = new TrafficViewerFile();
			serverdataStore.Profile.SetExclusions(new string[0] {  });
			TrafficViewerFile mockSiteData = new TrafficViewerFile();
			string testRequest = "GET /a HTTP/1.1\r\nIf-Modified-Since: 10-10-2012\r\nIf-None-Match: 123\r\nProxy-Connection: keep-alive\r\nAccept-Encoding: gzip\r\n\r\n";
			string testResponse = "HTTP/1.1 200 OK\r\nConnection: close\r\n\r\n";

			mockSiteData.AddRequestResponse(testRequest, testResponse);
			MockProxy mockServer = new MockProxy(serverdataStore, mockSiteData);
			
			mockServer.Start();

			//setup a mock proxy

			TrafficViewerFile proxyDataStore = new TrafficViewerFile();
			proxyDataStore.Profile.SetExclusions(new string[1] { @".*\.gif" });
			ManualExploreProxy meProxy = new ManualExploreProxy("127.0.0.1", 17777, proxyDataStore);
			meProxy.Start();

			IHttpClient httpClient = GetHttpClient(ClientType.TrafficViewerHttpClient, meProxy.Port); //need to use the traffic viewer client here
			//the webrequestclient does not allow requests to localhost through a proxy on localhost
			HttpRequestInfo testRequestInfo = new HttpRequestInfo(testRequest);
			testRequestInfo.Host = mockServer.Host;
			testRequestInfo.Port = mockServer.Port;


			httpClient.SendRequest(testRequestInfo);

			
			HttpRequestInfo savedReqInfo = new HttpRequestInfo(serverdataStore.LoadRequestData(0));
			Assert.IsNull(savedReqInfo.Headers["If-Modified-Since"]);
			Assert.IsNull(savedReqInfo.Headers["If-None-Match"]);
			Assert.IsNull(savedReqInfo.Headers["Accept-Encoding"]);
			Assert.IsNull(savedReqInfo.Headers["Proxy-Connection"]);

			meProxy.Stop();
			mockServer.Stop();
		}
		
		private void SendManyConnectionsToProxy(bool keepAlive)
		{

			TrafficViewerFile dataStore = new TrafficViewerFile();
			TrafficViewerFile mockSite = new TrafficViewerFile();
			string testRequest;
			if (keepAlive)
			{
				testRequest = "GET http://site.com/a HTTP/1.1\r\nConnection: keep-alive\r\n\r\n";
			}
			else
			{
				testRequest = "GET http://site.com/a HTTP/1.1\r\nConnection: close\r\n\r\n";
			}
			string expectedResponseLine = "HTTP/1.1 200 OK";
			mockSite.AddRequestResponse(testRequest, expectedResponseLine);

			MockProxy proxy = new MockProxy(dataStore, mockSite);

			//set a lower connection limit
			proxy.ConnectionLimit = 2;

			proxy.Start();
			int connCount = 500;
			//open 500 connections and expect a good response
			for (int i = 0; i < connCount; i++)
			{
				IHttpClient httpClient = GetHttpClient(proxy.Port);

				HttpRequestInfo testRequestInfo = new HttpRequestInfo(testRequest);

				HttpResponseInfo respInfo = httpClient.SendRequest(testRequestInfo);

				Assert.AreEqual(200, respInfo.Status);
			}

			proxy.Stop();

			Assert.AreEqual(connCount, dataStore.RequestCount);
		}


		[TestMethod]
		public void Test_Proxy_OpenManyNonPersistentConnections()
		{
			SendManyConnectionsToProxy(false);
		}

		

		[TestMethod]
		public void Test_Proxy_OpenManyPersistentConnections()
		{
			SendManyConnectionsToProxy(true);
		}

		[TestMethod]
		public void Test_Proxy_ConnectionBusyProperty()
		{
			string testRequest = "GET http://site.com/ HTTP/1.1\r\nConnection: keep-alive\r\n\r\n";
			MockProxy proxy = new MockProxy(testRequest, "HTTP/1.1 200 OK\r\n\r\n");
			proxy.Start();

			IHttpClient client = GetHttpClient(proxy.Port);

			HttpRequestInfo testReqInfo = new HttpRequestInfo(testRequest);

			client.SendRequest(testReqInfo);

			Assert.IsFalse(proxy.CurrentConnection.IsBusy);

			proxy.Stop();
		}


		[TestMethod]
		public void Test_Proxy_ConnectionKeepAliveHeader()
		{
			string testRequest = "GET http://site.com/ HTTP/1.1\r\nConnection: keep-alive\r\n\r\n";
			MockProxy proxy = new MockProxy(testRequest, "HTTP/1.1 200 OK\r\n\r\n");
			proxy.Start();

			IHttpClient client = GetHttpClient(proxy.Port);

			HttpRequestInfo testReqInfo = new HttpRequestInfo(testRequest);

			client.SendRequest(testReqInfo);

			Assert.IsFalse(proxy.CurrentConnection.Closed);

			proxy.Stop();

			Assert.IsTrue(proxy.CurrentConnection.Closed);
		}

		[TestMethod]
		public void Test_Proxy_ConnectionCloseRequestHeader()
		{
			string testRequest = "GET http://site.com/ HTTP/1.1\r\nConnection: close\r\n\r\n";
			MockProxy proxy = new MockProxy(testRequest, "HTTP/1.1 200 OK\r\n\r\n");
			proxy.Start();

			IHttpClient client = GetHttpClient(ClientType.TrafficViewerHttpClient, proxy.Port);

			HttpRequestInfo testReqInfo = new HttpRequestInfo(testRequest);

			client.SendRequest(testReqInfo);

			Assert.IsTrue(proxy.CurrentConnection.Closed);

			proxy.Stop();
		}

		[TestMethod]
		public void Test_Proxy_ConnectionCloseResponseHeader()
		{
			string testRequest = "GET http://site.com/ HTTP/1.1\r\n\r\n";
			MockProxy proxy = new MockProxy(testRequest, "HTTP/1.1 200 OK\r\nConnection: close\r\n");
			proxy.Start();

			IHttpClient client = GetHttpClient(proxy.Port);

			HttpRequestInfo testReqInfo = new HttpRequestInfo(testRequest);

			client.SendRequest(testReqInfo);

			Assert.IsTrue(proxy.CurrentConnection.Closed);

			proxy.Stop();
		}
		
	}
}
