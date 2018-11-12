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
	public class HttpClientRequestLineTest
	{
		private void RunRequestLineTest(string expectedValue)
		{
			WebRequestClient wrClient = new WebRequestClient();
			TrafficViewerFile dataStore = new TrafficViewerFile();
			TrafficViewerFile mockSite = new TrafficViewerFile();
			MockProxy mockProxy = new MockProxy(dataStore, mockSite);
			mockProxy.Start();

			HttpRequestInfo expectedRequest = new HttpRequestInfo(expectedValue);
			expectedRequest.Host = mockProxy.Host;
			expectedRequest.Port = mockProxy.Port;

			//set the webrequest to use a proxy

			HttpResponseInfo respInfo = wrClient.SendRequest(expectedRequest);

			mockProxy.Stop();
			if (!expectedRequest.IsConnect)
			{
				Assert.AreEqual(1, dataStore.RequestCount);

				byte[] receivedReqBytes = dataStore.LoadRequestData(0);

				HttpRequestInfo receivedRequest = new HttpRequestInfo(receivedReqBytes);

				Assert.AreEqual(expectedValue, receivedRequest.RequestLine);
			}
			else
			{
				Assert.AreEqual("HTTP/1.1 200 Connection established", respInfo.StatusLine);
			}
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_root_HTTP11()
		{
			RunRequestLineTest("GET / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_root_HTTP10()
		{
			RunRequestLineTest("GET / HTTP/1.0");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_HEAD_root_HTTP11()
		{
			RunRequestLineTest("HEAD / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_PUT_root_HTTP11()
		{
			RunRequestLineTest("PUT / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_POST_root_HTTP11()
		{
			RunRequestLineTest("POST / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_DELETE_root_HTTP11()
		{
			RunRequestLineTest("DELETE / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_TRACE_root_HTTP11()
		{
			RunRequestLineTest("TRACE / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_CONNECT_root_HTTP11()
		{
			RunRequestLineTest("CONNECT / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_OPTIONS_root_HTTP11()
		{
			RunRequestLineTest("OPTIONS / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_PATCH_root_HTTP11()
		{
			RunRequestLineTest("PATCH / HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_path_HTTP11()
		{
			RunRequestLineTest("GET /testPath/iNdEx.html HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_PUT_path_HTTP11()
		{
			RunRequestLineTest("PUT /testPath/iNdEx.html HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_POST_path_HTTP11()
		{
			RunRequestLineTest("POST /testPath/iNdEx.html HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_pathAndQuery_HTTP11()
		{
			RunRequestLineTest("GET /testPath/iNdEx.html?a=1&b=2 HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_PUT_pathAndQuery_HTTP11()
		{
			RunRequestLineTest("PUT /testPath/iNdEx.html?a=1&b=2 HTTP/1.1");
		}


		[TestMethod]
		public void Test_HttpClient_RequestLine_POST_pathAndQuery_HTTP11()
		{
			RunRequestLineTest("POST /testPath/iNdEx.html?a=1&b=2 HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_pathTrailingDot_HTTP11()
		{
			RunRequestLineTest("GET /path./ HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_queryTrailingDot_HTTP11()
		{
			RunRequestLineTest("GET /path?a=. HTTP/1.1");
		}



		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_pathEncodedForwardSlash_HTTP11()
		{
			RunRequestLineTest("GET /%2f/ HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_queryEncodedForwardSlash_HTTP11()
		{
			RunRequestLineTest("GET /?a=%2f HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_pathBackSlash_HTTP11()
		{
			RunRequestLineTest("GET /\\/ HTTP/1.1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_queryBackSlashEncoded_HTTP11()
		{
			RunRequestLineTest("GET /?a=%5C/ HTTP/1.1");
		}


		[TestMethod]
		public void Test_HttpClient_RequestLine_GET_pathSpecialChar_HTTP11() 
		{
			RunRequestLineTest("GET /xjs/_/!@$&*()_+;'\\/= HTTP/1.1");
			//the following chars have special behavior
			//# truncates the uri
			//%<>" are automatically url encoded
		}

		
	}
}
