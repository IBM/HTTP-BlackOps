using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using System.Net;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class BaseHttpClientRequestTest
	{

		protected enum ClientType
		{
			WebRequestClient,
			TrafficViewerHttpClient
		}

		protected virtual ClientType DefaultHttpClientType
		{
			get
			{
				return ClientType.WebRequestClient;
			}
		}


		protected IHttpClient GetHttpClient(int proxyPort = -1)
		{
			return GetHttpClient(DefaultHttpClientType, proxyPort);
		}

		protected IHttpClient GetHttpClient(ClientType clientType, int proxyPort = -1)
		{
			IHttpClient client;

			if (clientType == ClientType.WebRequestClient)
			{
				client = new WebRequestClient();
			}
			else
			{
				client = new TrafficViewerHttpClient();
			}

			if (proxyPort != -1)
			{
				INetworkSettings netSettings = new DefaultNetworkSettings();
				netSettings.WebProxy = new WebProxy("127.0.0.1", proxyPort);
				client.SetNetworkSettings(netSettings);
                
			}

			return client;

		}


		protected void SendTestRequestThroughMockProxy(string testRequest, 
			string testResponse, out HttpRequestInfo receivedRequest, out HttpResponseInfo receivedResponse)
		{
			SendTestRequestThroughMockProxy(new HttpRequestInfo(testRequest), new HttpResponseInfo(testResponse), out receivedRequest, out receivedResponse);
		}

		protected void SendTestRequestThroughMockProxy(HttpRequestInfo testRequest, 
			HttpResponseInfo testResponse, out HttpRequestInfo receivedRequest, out HttpResponseInfo receivedResponse, 
			ClientType clientType = ClientType.WebRequestClient, int proxyPort = 0)
		{
			
			TrafficViewerFile mockSite = new TrafficViewerFile();
			mockSite.AddRequestResponse(testRequest.ToArray(true), testResponse.ToArray());
			TrafficViewerFile dataStore = new TrafficViewerFile();
			MockProxy mockProxy = new MockProxy(dataStore, mockSite, "127.0.0.1", proxyPort, 0);
			mockProxy.Start();

			IHttpClient client = GetHttpClient(mockProxy.Port);

			receivedResponse = client.SendRequest(testRequest);
			// check what was received in the proxy

			byte [] receivedRequestBytes = dataStore.LoadRequestData(0);
			if(receivedRequestBytes == null)
			{
				receivedRequest = null;
			}
			else
			{
				receivedRequest = new HttpRequestInfo(receivedRequestBytes);
			}
			mockProxy.Stop();
		}


		protected void SendTestRequestToMockProxy(HttpRequestInfo testRequest, 
			HttpResponseInfo testResponse, out HttpRequestInfo receivedRequest, out HttpResponseInfo receivedResponse,  int proxyPort = 0)
		{
			TrafficViewerFile mockSite = new TrafficViewerFile();
			TrafficViewerFile dataStore = new TrafficViewerFile();

			MockProxy mockProxy = new MockProxy(dataStore, mockSite, "127.0.0.1", proxyPort, 0);
			mockProxy.Start();

			//change the requests host and port to be the ones of the mock proxy
			testRequest.Host = mockProxy.Host;
			testRequest.Port = mockProxy.Port;

			mockSite.AddRequestResponse(testRequest.ToArray(false), testResponse.ToArray());
			
			IHttpClient client = GetHttpClient();

			receivedResponse = client.SendRequest(testRequest);
			// check what was received in the proxy
			byte[] receivedRequestBytes = dataStore.LoadRequestData(0);
			if (receivedRequestBytes == null)
			{
				receivedRequest = null;
			}
			else
			{
				receivedRequest = new HttpRequestInfo(receivedRequestBytes);
			}
			mockProxy.Stop();
		}


		protected HttpResponseInfo GetResponseFromMockProxy(string testRequest, string testResponse)
		{
			HttpRequestInfo receivedRequest;
			HttpResponseInfo receivedResponse;
			SendTestRequestThroughMockProxy(testRequest, testResponse, out receivedRequest, out receivedResponse);
			return receivedResponse;
		}

		protected HttpRequestInfo GetRequestSentToMockProxy(string testRequest, string testResponse)
		{
			HttpRequestInfo receivedRequest;
			HttpResponseInfo receivedResponse;
			SendTestRequestThroughMockProxy(testRequest, testResponse, out receivedRequest, out receivedResponse);
			return receivedRequest;
		}


		protected void CheckResponseStatus(string testRequest, WebRequestClient client, int expectedStatus)
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(testRequest);
			HttpResponseInfo receivedResponse = client.SendRequest(reqInfo);

			Assert.AreEqual(expectedStatus, receivedResponse.Status);

		}
	}
}
