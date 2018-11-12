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
	public class HttpClientRequestHeaderTests:BaseHttpClientRequestTest
	{

		private void RunHeaderTest(string headerName, string expectedValue, int port = -1) 
		{
			RunHeaderTest(headerName, expectedValue, false, port);
		}

		private void RunHeaderTest(string headerName, string expectedValue, bool expectNull, int port = -1)
		{
			
			HttpRequestInfo expectedRequest = new HttpRequestInfo("POST / HTTP/1.1\r\n\r\na=1\r\n");
			expectedRequest.Headers.Add(headerName, expectedValue);


			HttpRequestInfo receivedReqInfo;
			HttpResponseInfo receivedRespInfo;

            if (port != -1)
            {
                SendTestRequestToMockProxy(expectedRequest, new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n"),
                    out receivedReqInfo,
                    out receivedRespInfo,
                    port);
            }
            else
            {
                SendTestRequestToMockProxy(expectedRequest, new HttpResponseInfo("HTTP/1.1 200 OK\r\n\r\n"),
                    out receivedReqInfo,
                    out receivedRespInfo);
            }

			

			if (expectNull)
			{
				Assert.IsNull(receivedReqInfo.Headers[headerName]);
			}
			else
			{
				Assert.AreEqual(expectedValue, receivedReqInfo.Headers[headerName]);
			}
		}


		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Connection_keep_alive()
		{
			RunHeaderTest("Connection", "Keep-Alive"); //the default for HTTP/1.1 is keep-alive
		}

		[TestMethod] 
		public void Test_HttpClient_RequestHeaders_Connection_close()
		{
			// due to a bug in HttpWebRequest ignoring the keep-alive property the close value is not being added
			// at least validate that keep alive is not being added
			RunHeaderTest("Connection", "Close"); 
		}

		//[TestMethod] This fails due to a bug in HttpWebRequest ignoring the keep-alive property
		public void Test_HttpClient_RequestHeaders_ProxyConnection_close()
		{
			RunHeaderTest("Proxy-Connection", "close");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Accept_textplain()
		{
			RunHeaderTest("Accept", "text/plain");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Accept_Charset_utf8()
		{
			RunHeaderTest("Accept-Charset", "utf-8");
		}

		[TestMethod] 
		public void Test_HttpClient_RequestHeaders_Accept_Encoding_gzip()
		{
			RunHeaderTest("Accept-Encoding", "gzip, deflate");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Accept_Language_enUs()
		{
			RunHeaderTest("Accept-Language", "en-US");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Accept_Datetime_date()
		{
			RunHeaderTest("Accept-Datetime", "Thu, 31 May 2007 20:35:00 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Autorization_basic()
		{
			RunHeaderTest("Authorization", "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_CacheControl_noCache()
		{
			RunHeaderTest("Cache-Control", "no-cache");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Cookie_cookies()
		{
			RunHeaderTest("Cookie", "$Version=1; Skin=new;");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_ContentLength_len()
		{
			RunHeaderTest("Content-Length", "3"); //the test request sends a=1
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_ContentMD5_hash()
		{
			RunHeaderTest("Content-MD5", "Q2hlY2sgSW50ZWdyaXR5IQ=="); //the test request sends a=1
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_ContentType_applicationFormEncoded()
		{
			RunHeaderTest("Content-Type", "application/x-www-form-urlencoded"); 
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Date_date()
		{
			RunHeaderTest("Date", DateTime.Now.ToString("ddd, dd MMM yyyy hh:mm:ss") + " GMT");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Expect_100Continue_isNull() 
		{
			RunHeaderTest("Expect", "100-continue"); 
		}

		
		[TestMethod]
		public void Test_HttpClient_RequestHeaders_From_email()
		{
			RunHeaderTest("From", "user@example.com");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Host_hostandport()
		{
            RunHeaderTest("Host", "127.0.0.1:5999", 5999);
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_IfMatch_id()
		{
			RunHeaderTest("If-Match", "737060cd8c284d8af7ad3082f209582d");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_IfModifiedSince_date()
		{
			RunHeaderTest("If-Modified-Since", "Sat, 29 Oct 1994 19:43:31 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_IfNoneMatch_id()
		{
			RunHeaderTest("If-None-Match", "737060cd8c284d8af7ad3082f209582d");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_IfRange_id()
		{
			RunHeaderTest("If-Range", "737060cd8c284d8af7ad3082f209582d");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_IfUnmodifiedSince_date()
		{
			RunHeaderTest("If-Unmodified-Since", "Sat, 29 Oct 1994 19:43:31 GMT");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_MaxForwards_no()
		{
			RunHeaderTest("Max-Forwards", "10");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Pragma_nocache()
		{
			RunHeaderTest("Pragma", "no-cache");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_ProxyAuthorization_basic()
		{
			RunHeaderTest("Proxy-Authorization", "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Range_specifier_from_to()
		{
			RunHeaderTest("Range", "items=500-999");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Range_specifier_badRange()
		{
			RunHeaderTest("Range", "items=a-b", true);
			RunHeaderTest("Range", "items=30-0", true);
			RunHeaderTest("Range", "items=0-0", true);
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Referer_uri()
		{
			RunHeaderTest("Referer", "http://demo.testfire.net/main.aspx");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_TE_trailersDeflate()
		{
			RunHeaderTest("TE", "trailers, deflate");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Upgrade_expectNull() 
		{
			RunHeaderTest("Upgrade", "HTTP/2.0, SHTTP/1.3, IRC/6.9, RTA/x11");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_UserAgent_uaValue() 
		{
			RunHeaderTest("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:12.0) Gecko/20100101 Firefox/12.0");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_Warning_message()
		{
			RunHeaderTest("Warning", "199 Miscellaneous warning");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_AppScan_xssmessage()
		{
			RunHeaderTest("AppScan", "xyz<script>alert(1)</script>");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_XRequestedWith_XMLHTTPRequest()
		{
			RunHeaderTest("X-Requested-With", "XMLHttpRequest");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_DNT_1()
		{
			RunHeaderTest("DNT", "1");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_XForwardedFor_client()
		{
			RunHeaderTest("X-Forwarded-For", "129.78.138.66, 129.78.64.103");
		}

		[TestMethod]
		public void Test_HttpClient_RequestHeaders_XForwardedProto_proto()
		{
			RunHeaderTest("X-Forwarded-Proto", "https");
		}
	}
}
