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
	public class HttpClientResponseHeaderTest:BaseHttpClientRequestTest
	{
		private void RunResponseHeaderTest(string headerName, string expectedValue)
		{
			HTTPHeaders headers = new HTTPHeaders();
			headers.Add(headerName, expectedValue);
			RunResponseHeaderTest(headers);
		}

		private void RunResponseHeaderTest(HTTPHeaders expectedHeaders)
		{

			HttpRequestInfo expectedRequest = new HttpRequestInfo("GET / HTTP/1.1\r\n\r\n");
			HttpResponseInfo expectedResponse = new HttpResponseInfo("HTTP/1.1 200 OK\r\n"+expectedHeaders.ToString()+"\r\nbody");


			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedResponseInfo;

			SendTestRequestToMockProxy(expectedRequest, expectedResponse, out receivedReqInfo, out receivedResponseInfo);

			foreach (HTTPHeader expectedHeader in expectedHeaders)
			{
				List<HTTPHeader> headers = receivedResponseInfo.Headers.GetHeaders(expectedHeader.Name);
				bool isMatch = false;
				foreach (HTTPHeader header in headers)
				{
					if (String.Compare(header.Value, expectedHeader.Value, false) == 0)
					{
						isMatch = true;
					}
				}
				Assert.IsTrue(isMatch, "Header {0} mismatch", expectedHeader.Name);
			}
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_AccessControlAllowOrigin_any() 
		{
			RunResponseHeaderTest("Access-Control-Allow-Origin", "*");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_AcceptRanges_bytes()
		{
			RunResponseHeaderTest("Accept-Ranges", "bytes");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Age_seconds()
		{
			RunResponseHeaderTest("Age", "12");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Allow_method()
		{
			RunResponseHeaderTest("Allow", "GET HEAD");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_CacheControl_age()
		{
			RunResponseHeaderTest("Cache-Control", "max-age=3600");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Connection_close()
		{
			RunResponseHeaderTest("Connection", "close");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentEncoding_gzip()
		{
			RunResponseHeaderTest("Content-Encoding", "gzip");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentLanguage_en()
		{
			RunResponseHeaderTest("Content-Language", "en");
		}

		
		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentLength_correct() //content length is added anyways automatically
		{
			RunResponseHeaderTest("Content-length", "4");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentLocation_address() 
		{
			RunResponseHeaderTest("Content-location", "/index.htm");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentMD5_hash()
		{
			RunResponseHeaderTest("Content-MD5", "Q2hlY2sgSW50ZWdyaXR5IQ==");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentDisposition_attachmentFile()
		{
			RunResponseHeaderTest("Content-Disposition", "attachment; filename=\"fname.ext\"");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentRange_range()
		{
			RunResponseHeaderTest("Content-Range", "bytes 21010-47021/47022");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ContentType_mimeAndEncoding()
		{
			RunResponseHeaderTest("Content-Type", "text/html; charset=utf-8");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Date_date()
		{
			RunResponseHeaderTest("Date", "Tue, 15 Nov 1994 08:12:31 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Etag_hash()
		{
			RunResponseHeaderTest("ETag", "737060cd8c284d8af7ad3082f209582d");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Expires_date()
		{
			RunResponseHeaderTest("Expires", "Thu, 01 Dec 1994 16:00:00 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_LastModified_date()
		{
			RunResponseHeaderTest("Last-Modified", "Thu, 01 Dec 1994 16:00:00 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Link_rel()
		{
			RunResponseHeaderTest("Link", "</feed>; rel=\"alternate\"");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Location_link()
		{
			RunResponseHeaderTest("Location", "http://demo.testfire.net");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_P3P_val()
		{
			RunResponseHeaderTest("P3P", "CP=\"This is not a P3P policy! See http://www.google.com/support/accounts/bin/answer.py?hl=en&answer=151657 for more info.\"");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Pragma_noCache()
		{
			RunResponseHeaderTest("Pragma", "no-cache");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ProxyAuthenticate_basic()
		{
			RunResponseHeaderTest("Proxy-Authenticate", "basic");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_ProxyAuthenticate_multiple()
		{
			HTTPHeaders headers = new HTTPHeaders();
			headers.Add("Proxy-Authenticate", "Negotiate");
			headers.Add("Proxy-Authenticate", "NTLM");
			headers.Add("Proxy-Authenticate", "digest");
			headers.Add("Proxy-Authenticate", "basic");
			RunResponseHeaderTest(headers);
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Refresh_secUrl()
		{
			RunResponseHeaderTest("Refresh", "5; url=http://www.w3.org/pub/WWW/People.html");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_RetryAfter_sec()
		{
			RunResponseHeaderTest("Retry-After", "120");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Server_name()
		{
			RunResponseHeaderTest("Server", "Apache");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_SetCookie_single()
		{
			RunResponseHeaderTest("Set-Cookie", "ASP.Net_SessionId=cvJV936295632bkcsaksc");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_SetCookie_singleHeaderMultipleCookies()
		{
			RunResponseHeaderTest("Set-Cookie", "UserID=JohnDoe; Max-Age=3600; Version=1");
		}


        [TestMethod]
		public void Test_HttpClient_ResponseHeaders_SetCookie_multiple()
		{
			HTTPHeaders headers = new HTTPHeaders();
 			headers.Add("Set-Cookie", "ORASSO_AUTH_HINT=v1.0~20130715222353; path=/; ;");
            headers.Add("Set-Cookie", "ORA_UCM_INFO=3~291A1AA7BA90676EE0401490BEAB1D1; path=/; ; expires=Sat, 11-Jan-2014 14:23:53 GMT");
            headers.Add("Set-Cookie", "CookieName=cookie_v==alue; expires=Sat, 11-Jan-2014 14:23:53 GMT; path=/; ;");
			RunResponseHeaderTest(headers);
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_StrictTransportSecurity_value()
		{
			RunResponseHeaderTest("Strict-Transport-Security", "max-age=16070400; includeSubDomains");
		}


		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Trailer_value()
		{
			RunResponseHeaderTest("Trailer", "Max-Forwards");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_TransferEncoding_chunked()
		{
			RunResponseHeaderTest("Transfer-Encoding", "chunked");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Vary_any()
		{
			RunResponseHeaderTest("Vary", "*");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Via_proxies()
		{
			RunResponseHeaderTest("Via", "TrafficViewer");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_Warning_message()
		{
			RunResponseHeaderTest("Warning", "199 Miscellaneous warning");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_WWWAuthenticate_single()
		{
			RunResponseHeaderTest("WWW-Authenticate", "Negotiate");
		}

		[TestMethod]
		public void Test_HttpClient_ResponseHeaders_WWWAuthenticate_multiple()
		{
			HTTPHeaders headers = new HTTPHeaders();
			headers.Add("WWW-Authenticate", "Negotiate");
			headers.Add("WWW-Authenticate", "NTLM");
			headers.Add("WWW-Authenticate", "digest");
			headers.Add("WWW-Authenticate", "basic");
			RunResponseHeaderTest(headers);
		}


	}
}
