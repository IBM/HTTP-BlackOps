using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using TrafficServer;
using TrafficViewerUnitTest.Properties;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class TrafficLogHttpClientUnitTest
	{
		public void FindSequence(
			IEnumerable<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availSequence,
			IEnumerable<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence,
			TrafficServerMode proxyMode,
			bool ignoreAuth)
		{
			TrafficViewerFile mockSite = new TrafficViewerFile();
			//add a few requests
			mockSite.AddRequestResponse("GET / HTTP/1.1\r\nHost:demo\r\n\r\n", "HTTP/1.1 200 OK\r\n\r\n");
			mockSite.AddRequestResponse("POST /login HTTP/1.1\r\nHost:demo\r\n\r\n", "HTTP/1.1 302 Redirect\r\n\r\n");
			//add the sequence
			foreach (KeyValuePair<HttpRequestInfo, HttpResponseInfo> reqRespInfo in availSequence)
			{ 
				mockSite.AddRequestResponse(reqRespInfo.Key.ToString(), reqRespInfo.Value.ToString());
			}
			//add some more requests
			mockSite.AddRequestResponse("GET /main.aspx HTTP/1.1\r\nHost:demo\r\n\r\n", "HTTP/1.1 200 OK\r\n\r\n");

			//now send the expected sequence
			TrafficStoreHttpClient client = new TrafficStoreHttpClient(mockSite, proxyMode, ignoreAuth);

			foreach (KeyValuePair<HttpRequestInfo, HttpResponseInfo> sentReqResp in expectedSequence)
			{
				HttpResponseInfo receivedResp = client.SendRequest(sentReqResp.Key);
				if (receivedResp.Status != 404)
				{
					Assert.IsNotNull(receivedResp.Headers["Traffic-Store-Req-Id"]);//verify that the request id is being added
					receivedResp.Headers.Remove("Traffic-Store-Req-Id"); //remove the request id from the comparison
				}
				
				Assert.AreEqual(sentReqResp.Value.ToString(), receivedResp.ToString());
			}
		}

		public void FindRequest(string availableRequest, string availableResponse, string sentRequest, string expectedResponse, TrafficServerMode proxyMode, bool ignoreAuth)
		{
			HttpResponseInfo availRespInfo = new HttpResponseInfo(availableResponse);
			HttpResponseInfo expectedRespInfo = new HttpResponseInfo(expectedResponse);
			HttpRequestInfo availableReqInfo = new HttpRequestInfo(availableRequest);
			HttpRequestInfo sentReqInfo = new HttpRequestInfo(sentRequest);

			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			availableSequence.Add(new KeyValuePair<HttpRequestInfo,HttpResponseInfo>(availableReqInfo, availRespInfo));
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(new KeyValuePair<HttpRequestInfo,HttpResponseInfo>(sentReqInfo, expectedRespInfo));
			FindSequence(availableSequence, expectedSequence, proxyMode, ignoreAuth);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_Request_Not_Found()
		{
			FindRequest(
				"GET /x HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\n",
				"GET /y HTTP/1.1\r\n\r\n",
				Resources.ExpectedTrafficLogHttpClient404,
				TrafficServerMode.BrowserFriendly,
				false);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_Variables_Not_Matching_Values_IgnoreCookiesMode()
		{
			FindRequest(
				"GET /x0?x=1 HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				"GET /x0?x=2 HTTP/1.1\r\n\r\n",
				Resources.ExpectedTrafficLogHttpClient404,
				TrafficServerMode.IgnoreCookies,
				false);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_Variables_Not_Matching_Values_BrowserFriendlyMode()
		{
			FindRequest(
				"GET /x1?x=1 HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				"GET /x1?x=2 HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				TrafficServerMode.BrowserFriendly,
				false);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_POST_Variables_Not_Matching_Values_IgnoreCookiesMode()
		{
			FindRequest(
				"POST /x2 HTTP/1.1\r\n\r\nx=1",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				"POST /x2 HTTP/1.1\r\n\r\nx=2",
				Resources.ExpectedTrafficLogHttpClient404,
				TrafficServerMode.IgnoreCookies,
				false);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_POST_Variables_Not_Matching_Values_BrowserFriendlyMode()
		{
			FindRequest(
				"POST /x3 HTTP/1.1\r\n\r\nx=1",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				"POST /x3 HTTP/1.1\r\n\r\nx=2",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				TrafficServerMode.BrowserFriendly,
				false);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_POST_Variables_Not_Matching_Values_BrowserFriendlyMode_ExtraVar()
		{
			FindRequest(
				"POST /x6 HTTP/1.1\r\n\r\nx=1",
				"HTTP/1.1 200 OK\r\n\r\nx=1response",
				"POST /x6 HTTP/1.1\r\n\r\nx=2&y=3",
				Resources.ExpectedTrafficLogHttpClient404,
				TrafficServerMode.BrowserFriendly,
				false);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_Cookies_Not_Matching_Values_IgnoreCookiesMode()
		{
			FindRequest(
				"GET /x4?x=1 HTTP/1.1\r\nCookie:JSESSIONID=1\r\n",
				"HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1response",
				"GET /x4?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n",
				"HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1response",
				TrafficServerMode.IgnoreCookies,
				false);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_Cookies_Not_Matching_Values_StrictMode()
		{
			FindRequest(
				"GET /x5?x=1 HTTP/1.1\r\nCookie:JSESSIONID=1\r\n",
				"HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1response",
				"GET /x5?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n",
				Resources.ExpectedTrafficLogHttpClient404,
				TrafficServerMode.Strict,
				false);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_XSS_Alert()
		{
			FindRequest(
				"GET /x12?x=<script>alert(1)</script> HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\n<script>alert(1)</script>",
				"GET /x12?x=<script>alert(2)</script> HTTP/1.1\r\n\r\n",
				"HTTP/1.1 200 OK\r\n\r\n<script>alert(2)</script>",
				TrafficServerMode.IgnoreCookies,
				false);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_ExactMatch_BrowserFriendlyMode()
		{
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo,HttpResponseInfo>>();
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo,HttpResponseInfo>(
					new HttpRequestInfo("GET /x7?x=1 HTTP/1.1\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nx=1")));
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x7?x=2 HTTP/1.1\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nx=2")));

			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x7?x=2 HTTP/1.1\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nx=2")));

			//both requests will match however the second is the exact one and is the one that should be returned
			FindSequence(availableSequence, expectedSequence, TrafficServerMode.BrowserFriendly, true);
		}

		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_ExactMatch_IgnoreCookiesMode()
		{
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x8?x=1 HTTP/1.1\r\nCookie:JSESSIONID=1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1")));
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x8?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=2")));

			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x8?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=2")));

			//both requests will match however the second is the exact one and is the one that should be returned
			FindSequence(availableSequence, expectedSequence, TrafficServerMode.IgnoreCookies, true);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_ExactMatch_Twice_Different_Responses_StrictMode()
		{
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x9?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1")));
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x9?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=2")));

			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x9?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=1")));
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x9?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\nJSESSIONID=2")));

			FindSequence(availableSequence, expectedSequence, TrafficServerMode.IgnoreCookies, true);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_GET_Match_Twice_Different_Responses_IgnoreCookiesMode()
		{
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x10?x=1 HTTP/1.1\r\nCookie:JSESSIONID=1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n1")));
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x10?x=1 HTTP/1.1\r\nCookie:JSESSIONID=1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n2")));
			
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x10?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n1")));
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x10?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n2")));
			
			FindSequence(availableSequence, expectedSequence, TrafficServerMode.IgnoreCookies, true);
		}


		[TestMethod]
		public void Test_TrafficLogHttpClient_SimulateAuthentication()
		{
			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> availableSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x11?x=1 HTTP/1.1\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 401 Authenticate\r\nWWW-Authenticate:basic\r\n\r\n")));
			availableSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x11?x=1 HTTP/1.1\r\nAuthorization:basic vsfdrfegww==\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n")));

			List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>> expectedSequence = new List<KeyValuePair<HttpRequestInfo, HttpResponseInfo>>();
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x11?x=1 HTTP/1.1\r\nCookie:JSESSIONID=2\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 401 Authenticate\r\nWWW-Authenticate:basic\r\n\r\n")));
			expectedSequence.Add(
				new KeyValuePair<HttpRequestInfo, HttpResponseInfo>(
					new HttpRequestInfo("GET /x11?x=1 HTTP/1.1\r\nAuthorization:basic vsfdrfegww==\r\n\r\n"),
					new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n")));

			FindSequence(availableSequence, expectedSequence, TrafficServerMode.IgnoreCookies, false);
		}


		
	}
}
