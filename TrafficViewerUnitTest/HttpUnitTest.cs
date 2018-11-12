using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficServer;
using TrafficViewerSDK.Http;
using System.Threading;
using System.Xml;
using TrafficViewerUnitTest.Properties;
using System.IO;
using TrafficViewerControls;
using TrafficViewerInstance;

namespace TrafficViewerUnitTest
{

	/// <summary>
	/// Summary description for HttpUnitTest
	/// </summary>
	[TestClass]
	public class HttpUnitTest
	{

		private static Random rand = new Random();

		public HttpUnitTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion


        [TestMethod]
        public void TestBasicAuth()
        {
            TrafficViewerFile tvf = new TrafficViewerFile();
            tvf.AddRequestResponse("GET / HTTP/1.1", Resources.basicauthresponse);

            TrafficStoreProxy proxy = new TrafficStoreProxy(tvf);
            proxy.Start();

            TrafficViewerHttpClient client = new TrafficViewerHttpClient();
            client.SetProxySettings(proxy.Host, proxy.Port, null);
            
        }

		//[TestMethod]
		public void TestSendingMultiPartRequest()
		{
			TrafficViewer.Instance.NewTvf();

			TrafficViewer.Instance.TrafficViewerFile.AddRequestResponse(Resources.MultipartRequest, "HTTP/1.1 200 OK\r\nConnection: close\r\n\r\n");

			TrafficStoreProxy httpProxy = new TrafficStoreProxy(TrafficViewer.Instance.TrafficViewerFile);

			httpProxy.Start();

            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.MultipartRequest);
			TrafficViewerHttpClient client = new TrafficViewerHttpClient();
			client.SetProxySettings("127.0.0.1", httpProxy.Port, null);


			HttpResponseInfo respInfo = client.SendRequest(reqInfo);

			Assert.AreEqual(200, respInfo.Status);

			TrafficViewer.Instance.CloseTvf(false);
		}

		[TestMethod]
		public void TestTrafficLogProxy()
		{
            TrafficViewer.Instance.HttpClientFactory = new TrafficViewerHttpClientFactory();

			TrafficViewerFile tvf = UnitTestUtils.GenerateTestTvf();
			TrafficStoreProxy proxy = new TrafficStoreProxy(tvf);
			proxy.Start();

			HttpClientRequest request = new HttpClientRequest();

			HttpRequestInfo reqInfo = new HttpRequestInfo(Properties.Resources.AltoroLoginPageRequest);

			//change the host and port to the proxy

			reqInfo.Host = proxy.Host;
			reqInfo.Port = proxy.Port;

			//test http
			request.SendRequest(reqInfo, false);

			request.RequestCompleteEvent.WaitOne();

			ValidateResponse(request);

			//test https
			proxy.Stop();
			proxy.Start();

			request.SendRequest(reqInfo, true);

			request.RequestCompleteEvent.WaitOne(2*1000);

			ValidateResponse(request);

			proxy.Stop();
		}

		private static void ValidateResponse(HttpClientRequest request)
		{
			Assert.IsNotNull(request.Response);

			//parse the response and validate it is correct
			HttpResponseInfo respInfo = new HttpResponseInfo();
			respInfo.ProcessResponse(request.Response);

			Assert.AreEqual(200, respInfo.Status);

			//parse the response body
			XmlDocument dom;
			HtmlParserHelper.Parse(respInfo, out dom);

			XmlNode title = dom.SelectSingleNode("//title");

			Assert.IsTrue(title.InnerText.Contains("Altoro Mutual: Online Banking Login"));
		}


		[TestMethod]
		public void TestBasicChunkedEncoding()
		{
			HttpResponseInfo resp = new HttpResponseInfo();

			resp.ProcessResponse(Resources.ChunkedResponse);

			string body = resp.ResponseBody.ToString();

			Assert.AreEqual(Resources.DecodedChunkedBody, body);

			resp.ProcessResponse(Resources.IncorrectChunkedResponse);

			body = resp.ResponseBody.ToString();

			Assert.AreEqual(Resources.DecodedChunkedBody, body);
		}

	

	}
}
