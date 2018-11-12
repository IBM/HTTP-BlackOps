using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using ManualExplorerUnitTest.Properties;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpClientResponseBodyTest:BaseHttpClientRequestTest
	{
	

		private void RunResponseBodyTest(string contentType, string expectedBody, string encoding = "UTF-8")
		{
			Encoding expectedEncoding = Encoding.GetEncoding(encoding);
			byte[] expectedBodyBytes = expectedEncoding.GetBytes(expectedBody);

			RunResponseBodyTest(contentType, expectedBodyBytes, expectedEncoding);
		}

		private void RunResponseBodyTest(string contentType, byte[] expectedBody, Encoding expectedEncoding)
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();


			byte[] responseHead = Constants.DefaultEncoding.GetBytes(String.Format("HTTP/1.1 200 OK\r\nContent-Type: {0}\r\nContent-Length: {1}\r\n\r\n", contentType, expectedBody.Length));

			builder.AddChunkReference(responseHead, responseHead.Length);
			builder.AddChunkReference(expectedBody, expectedBody.Length);
			byte[] expectedResponseBytes = builder.ToArray();
			HttpRequestInfo expectedRequest = new HttpRequestInfo("GET / HTTP/1.1\r\n\r\n");
			HttpResponseInfo expectedResponse = new HttpResponseInfo(expectedResponseBytes);

            

			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedRespInfo;

			SendTestRequestToMockProxy(expectedRequest, expectedResponse,
				out receivedReqInfo,
				out receivedRespInfo);


			string receivedResponseBody = receivedRespInfo.ResponseBody.ToString(receivedRespInfo.Headers["Content-Type"]);
			Assert.AreEqual(expectedEncoding.GetString(expectedBody), receivedResponseBody);
		}


		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_NoCharset()
		{
			RunResponseBodyTest("Content-type: text/html", "<body>Test</body>");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Charset_UTF8()
		{
			RunResponseBodyTest("Content-type: text/html;charset=UTF-8", "<body>Test</body>", "UTF-8");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_UTF8Charset_SHIFTJIS_Fail()
		{
			bool failed = false;
			try
			{
				RunResponseBodyTest("Content-type: text/html;charset=SHIFT-JIS", "<body>はん用的試験項目【はんようてきしけんこうもく】</body>", "UTF-8");
			}
			catch (AssertFailedException)
			{
				failed = true;
			}

			Assert.IsTrue(failed);
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Japanese_UTF8()
		{
			RunResponseBodyTest("Content-type: text/html;charset=UTF-8", "<body>はん用的試験項目【はんようてきしけんこうもく】</body>", "UTF-8");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Japanese_SHIFTJIS()
		{
			RunResponseBodyTest("Content-type: text/html;charset=SHIFT-JIS", "<body>はん用的試験項目【はんようてきしけんこうもく】</body>", "SHIFT-JIS");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Korean_EUCKR()
		{
			RunResponseBodyTest("Content-type: text/html;charset=EUC-KR", "<body>로그인 하시면 회원님을 위한 현대카드의 다양한 서비스와 혜택을 누리실 수 있습니다</body>", "EUC-KR");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Chinese_Simplified()
		{
			RunResponseBodyTest("Content-type: text/html;charset=GB2312", "<body>找不到相关页面</body>", "GB2312");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Text_Html_Chinese_UTF8()
		{
			RunResponseBodyTest("Content-type: text/html;charset=utf-8", "<body>找不到相关页面</body>", "UTF-8");
		}

		

		[TestMethod]
		public void Test_HttpClient_ResponseBody_Binary()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			byte[] responseHead = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\nContent-Length:4\r\n\r\n");
			builder.AddChunkReference(responseHead, responseHead.Length);
			builder.AddChunkReference(new byte[4] { 0, 1, 0, 1 }, 4);
			HttpRequestInfo reqInfo = new HttpRequestInfo("GET /binary HTTP/1.1");
			HttpResponseInfo respInfo = new HttpResponseInfo(builder.ToArray());

			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedResponseInfo;

			SendTestRequestToMockProxy(reqInfo, respInfo, out receivedReqInfo, out receivedResponseInfo);

			
			byte[] respBody = receivedResponseInfo.ResponseBody.ToArray();

			Assert.AreEqual(4, respBody.Length);
			Assert.AreEqual(0, respBody[0]);
			Assert.AreEqual(1, respBody[1]);
			Assert.AreEqual(0, respBody[2]);
			Assert.AreEqual(1, respBody[3]);
		}

		[TestMethod]
		public void Test_HttpClient_ResponseBody_ChunkedResponse()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			
			HttpRequestInfo reqInfo = new HttpRequestInfo("GET /chunked HTTP/1.1");
			HttpResponseInfo respInfo = new HttpResponseInfo(Resources.ChunkedResponse);

			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedResponseInfo;

			SendTestRequestToMockProxy(reqInfo, respInfo, out receivedReqInfo, out receivedResponseInfo);

			Assert.IsNotNull(receivedResponseInfo);

			Assert.IsNull(receivedResponseInfo.Headers["Content-length"]);
			byte[] respBody = receivedResponseInfo.ResponseBody.ToArray();

			Assert.IsNotNull(respBody);

			ValidateChunk("This is the data in the first chunk\r\n", receivedResponseInfo.ResponseBody.ReadChunk());
			ValidateChunk("and this is the second one\r\n", receivedResponseInfo.ResponseBody.ReadChunk());
			ValidateChunk("con", receivedResponseInfo.ResponseBody.ReadChunk());
			ValidateChunk("sequence", receivedResponseInfo.ResponseBody.ReadChunk());

		}

		private void ValidateChunk(string expected, byte[] chunk)
		{
			Assert.IsNotNull(chunk);
			Assert.AreEqual(expected, Encoding.UTF8.GetString(chunk));
		}
	}
}
