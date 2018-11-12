using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using ManualExplorerUnitTest.Properties;
using TrafficViewerSDK;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpClientRequestBodyTest : BaseHttpClientRequestTest
	{

		private void RunRequestBodyTest(string method, string contentType, string expectedBody, string encoding = "UTF-8")
		{
			Encoding expectedEncoding = Encoding.GetEncoding(encoding);
			byte[] expectedBodyBytes = expectedEncoding.GetBytes(expectedBody);
			
			RunRequestBodyTest(method, contentType, expectedBodyBytes, expectedEncoding);
		}

		private void RunRequestBodyTest(string method, string contentType, byte [] expectedBody, Encoding expectedEncoding)
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			

			byte [] requestHead = Encoding.UTF8.GetBytes(String.Format("{0} / HTTP/1.1\r\nContent-Type: {1}\r\nContent-Length: {2}\r\n\r\n", method, contentType, expectedBody.Length));

			builder.AddChunkReference(requestHead, requestHead.Length);
			builder.AddChunkReference(expectedBody, expectedBody.Length);

			HttpRequestInfo expectedRequest = new HttpRequestInfo(builder.ToArray(), false);


			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedRespInfo;

			SendTestRequestToMockProxy(expectedRequest, new HttpResponseInfo("HTTP/1.1 200 OK"),
				out receivedReqInfo,
				out receivedRespInfo);


			Assert.AreEqual(expectedEncoding.GetString(expectedBody), receivedReqInfo.ContentDataString);
		}


		[TestMethod]
		public void Test_HttpClient_RequestBody_GET_emptyrequestbody()
		{
			RunRequestBodyTest("GET", "application/x-www-form-urlencoded", "");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_form_regular_notEncoded()
		{
			RunRequestBodyTest("POST", "application/x-www-form-urlencoded", "a=1&b=2");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_form_regular_encoded()
		{
			RunRequestBodyTest("POST", "application/x-www-form-urlencoded", "a=1%2f&b=2");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_json()
		{
			RunRequestBodyTest("POST", "application/json", "{\"uid\":\"jsmith\", \"passwd\":\"Demo1234\"}");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_PUT_json()
		{
			RunRequestBodyTest("PUT", "application/json", "{\"uid\":\"jsmith\", \"passwd\":\"Demo1234\"}");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_SOAP()
		{
			RunRequestBodyTest("POST", "text/xml;charset=UTF-8", Resources.SOAPPost);
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_Multipart()
		{
			RunRequestBodyTest("POST", 
				"Content-Type: multipart/related; type=\"application/xop+xml\"; boundary=\"uuid:9cd759e9-e0e1-43e1-abc4-c9f3db8601ad\"; start=\"<1>\"; start-info=\"text/xml\"", 
				Resources.MultiPartData);
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_JSON_Japanese_SHIFTJIS()
		{
			RunRequestBodyTest("POST", "application/json;charset=SHIFT-JIS", "{\"text\":\"はん用的試験項目【はんようてきしけんこうもく】\"}", "SHIFT-JIS");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_JSON_Korean_EUCKR()
		{
			RunRequestBodyTest("POST", "application/json;charset=EUC-KR", "{\"text\":\"로그인 하시면 회원님을 위한 현대카드의 다양한 서비스와 혜택을 누리실 수 있습니다\"}", "EUC-KR");
		}

		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_JSON_Chinese_Simplified()
		{
			RunRequestBodyTest("POST", "application/json;charset=GB2312", "{\"text\":\"找不到相关页面\"}", "GB2312");
		}

		//if an unknown charset is specified default to utf8
		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_JSON_UTF8()
		{
			RunRequestBodyTest("POST", "application/json;charset=utf-8", "{\"text\":\"找不到相关页面\"}", "UTF-8");
		}

		//if no charset is specified default to utf8
		//[TestMethod]
		public void Test_HttpClient_RequestBody_POST_JSON_NoCharset()
		{
			RunRequestBodyTest("POST", "application/json", "{\"text\":\"找不到相关页面\"}", "UTF-8");
		}

		//if no charset is specified default to utf8
		[TestMethod]
		public void Test_HttpClient_RequestBody_POST_Binary()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			byte [] requestHead = Encoding.UTF8.GetBytes("POST / HTTP/1.1\r\nContent-Type: application/octet-stream\r\nContent-Length:4\r\n\r\n");
			builder.AddChunkReference(requestHead, requestHead.Length);
			builder.AddChunkReference(new byte[4] { 0, 1, 0, 1 }, 4);
			HttpRequestInfo reqInfo = new HttpRequestInfo(builder.ToArray());
			
			HttpRequestInfo receivedRequestInfo;
			HttpResponseInfo receivedResponseInfo;

			SendTestRequestToMockProxy(reqInfo, new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n"), out receivedRequestInfo, out receivedResponseInfo);

			
			byte [] respBody = receivedRequestInfo.ContentData.ToArray();

			Assert.AreEqual(4, respBody.Length);
			Assert.AreEqual(0, respBody[0]);
			Assert.AreEqual(1, respBody[1]);
			Assert.AreEqual(0, respBody[2]);
			Assert.AreEqual(1, respBody[3]);
		}
	}
}
