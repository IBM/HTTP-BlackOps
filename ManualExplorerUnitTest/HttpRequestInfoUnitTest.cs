/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using ManualExplorerUnitTest.Properties;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpRequestInfoUnitTest
	{

		/// <summary>
		/// Test ipv6 request
		/// </summary>
		[TestMethod]
		public void Test_HttpRequestInfo_IPv6_NoPort()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.IPV6Request);

			Assert.AreEqual("http://[2002:917:2df7::917:2df7]/ase", reqInfo.FullUrl);
			Assert.AreEqual("/ase", reqInfo.Path);
			Assert.AreEqual("[2002:917:2df7::917:2df7]", reqInfo.Host);
			Assert.AreEqual("[2002:917:2df7::917:2df7]", reqInfo.Headers["Host"]);

		}

		/// <summary>
		/// Test ipv6 request
		/// </summary>
		[TestMethod]
		public void Test_HttpRequestInfo_IPv6_Port()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.IPv6RequestWithPort);

			Assert.AreEqual("https://[2002:917:2df7::917:2df7]:9443/jts", reqInfo.FullUrl);
			Assert.AreEqual("[2002:917:2df7::917:2df7]", reqInfo.Host);
			Assert.AreEqual("/jts", reqInfo.Path);
			Assert.AreEqual("[2002:917:2df7::917:2df7]", reqInfo.Host);
			Assert.AreEqual(9443, reqInfo.Port); 
			Assert.AreEqual("[2002:917:2df7::917:2df7]:9443", reqInfo.Headers["Host"]);
		}


		/// <summary>
		/// Tests a request that has a url in its query parameters
		/// </summary>
		[TestMethod]
		public void Test_HttpRequestInfo_UrlInQueryVariables()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.NyTimesRequest);
			Assert.AreEqual("/glogin", reqInfo.Path, "Path is not parsed properly");
			Assert.AreEqual(3, reqInfo.QueryVariables.Count, "Number of query variables is not determined properly");
			Assert.IsTrue(reqInfo.QueryVariables.ContainsKey("URI"), "URI is missing from query variables");
			string expectedUrlQueryString = @"http://www.nytimes.com/2013/01/15/world/asia/china-allows-media-to-report-alarming-air-pollution-crisis.html";
			Assert.AreEqual(expectedUrlQueryString, reqInfo.QueryVariables["URI"], "Value of 'URI' query variable is parsed improperly");
		}

		/// <summary>
		/// Tests a request that has a url in its path
		/// </summary>
		[TestMethod]
		public void Test_HttpRequestInfo_UrlInPath()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithHttpInPath);
			string expectedPath = "/bt/api/res/1.2/RMHM2QL7.RMHwFwGPO0kug--/YXBwaWQ9eW5ld3M7Y2g9MjQ4MTtjcj0xO2N3PTM3MjE7ZHg9MDtkeT0wO2ZpPXVsY3JvcDtoPTQyMTtxPTg1O3c9NjMw/http://media.zenfs.com/en_us/News/ap_webfeeds/57cdb3e9907f9605290f6a706700d101.jpg";
			Assert.AreEqual(expectedPath, reqInfo.Path, "Path is not parsed properly");
			Assert.AreEqual("l.yimg.com", reqInfo.Host, "Host is not correct");
		}

		//Firefox will send only one byte at a time
		[TestMethod]
		public void Test_HttpRequestInfo_IncompleteMethod()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo("G");

			Assert.IsFalse(reqInfo.IsFullRequest);

			reqInfo = new HttpRequestInfo("GET / ");

			Assert.IsFalse(reqInfo.IsFullRequest);
		}


		[TestMethod]
		public void Test_HttpRequestInfo_RequestMethod()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo("GET / HTTP/1.1");
			Assert.AreEqual("GET", reqInfo.Method);
			Assert.AreEqual(false, reqInfo.IsConnect);
			Assert.AreEqual(false, reqInfo.IsPost);
			Assert.AreEqual(false, reqInfo.IsPut);
			reqInfo = new HttpRequestInfo("POST / HTTP/1.1");
			Assert.AreEqual("POST", reqInfo.Method);
			Assert.AreEqual(false, reqInfo.IsConnect);
			Assert.AreEqual(true, reqInfo.IsPost);
			Assert.AreEqual(false, reqInfo.IsPut);
			reqInfo = new HttpRequestInfo("PUT / HTTP/1.1");
			Assert.AreEqual("PUT", reqInfo.Method);
			Assert.AreEqual(false, reqInfo.IsConnect);
			Assert.AreEqual(false, reqInfo.IsPost);
			Assert.AreEqual(true, reqInfo.IsPut);
			reqInfo = new HttpRequestInfo("CONNECT / HTTP/1.1");
			Assert.AreEqual("CONNECT", reqInfo.Method);
			Assert.AreEqual(true, reqInfo.IsConnect);
			Assert.AreEqual(false, reqInfo.IsPost);
			Assert.AreEqual(false, reqInfo.IsPut);
		}


		[TestMethod]
		public void Test_HttpRequestInfo_BinaryContent()
		{
			byte[] expectedPostData = new byte[4] { 1, 0, 0, 5 };
			string request = "POST / HTTP/1.1\r\nContent-Length: 4\r\n\r\n";
			ByteArrayBuilder arrayBuilder = new ByteArrayBuilder();
			byte[] requestBytes = Encoding.UTF8.GetBytes(request);
			arrayBuilder.AddChunkReference(requestBytes, requestBytes.Length);
			arrayBuilder.AddChunkReference(expectedPostData, expectedPostData.Length);
			HttpRequestInfo reqInfo = new HttpRequestInfo(arrayBuilder.ToArray());

			Assert.AreEqual(4, reqInfo.ContentData.Length);
			Assert.AreEqual(reqInfo.ContentData[0], 1);
			Assert.AreEqual(reqInfo.ContentData[1], 0);
			Assert.AreEqual(reqInfo.ContentData[2], 0);
			Assert.AreEqual(reqInfo.ContentData[3], 5);

		}

		[TestMethod]
		public void Test_HttpRequestInfo_RequestWithContentType()
		{
			
			string request = "POST / HTTP/1.1\r\nContent-Length: 4\r\nContent-Type: text/xml; charset=utf-8\r\n\r\n1234";
			HttpRequestInfo reqInfo = new HttpRequestInfo(request);
			Assert.AreEqual("1234", reqInfo.ContentDataString);
			request = "POST / HTTP/1.1\r\nContent-Length: 4\r\nContent-Type: text/xml; charset=appscan\r\n\r\n1234";
			reqInfo = new HttpRequestInfo(request);
			Assert.AreEqual("1234", reqInfo.ContentDataString);
			request = "POST / HTTP/1.1\r\nContent-Length: 4\r\nContent-Type: text/xml\r\n\r\n1234";
			reqInfo = new HttpRequestInfo(request);
			Assert.AreEqual("1234", reqInfo.ContentDataString);
		}

		[TestMethod]
		public void Test_HttpRequestInfo_IsFullRequestPOST()
		{
			string request = "POST / HTTP/1.1\r\nContent-Length: 4\r\n\r\n123";

			HttpRequestInfo reqInfo = new HttpRequestInfo(request);

			Assert.AreEqual(false, reqInfo.IsFullRequest);
			Assert.AreEqual(3, reqInfo.ContentLength);

			request = "POST / HTTP/1.1\r\nContent-Length: 4\r\n\r\n1234";
			reqInfo = new HttpRequestInfo(request);

			Assert.AreEqual(true, reqInfo.IsFullRequest);
			Assert.AreEqual(4, reqInfo.ContentLength);

			request = "POST / HTTP/1.1\r\nContent-Length: 4\r\n\r\n12345";
			reqInfo = new HttpRequestInfo(request);

			Assert.AreEqual(true, reqInfo.IsFullRequest);
			Assert.AreEqual(5, reqInfo.ContentLength);
		}

		[TestMethod]
		public void Test_HttpRequestInfo_IsFullRequestGET()
		{
			string request = "GET / HTTP/1.1\r\nHost: site.com\r\n\r\n";
			HttpRequestInfo reqInfo = new HttpRequestInfo(request);

			Assert.AreEqual(true, reqInfo.IsFullRequest);

			request = "GET / HTTP/1.1\r\nHost: site.com\r\n";
			reqInfo = new HttpRequestInfo(request);

			Assert.AreEqual(false, reqInfo.IsFullRequest);
		}


		[TestMethod]
		public void Test_HttpRequestInfo_GetRequestLine_From_Binary_CRNL()
		{
			string request = "GET / HTTP/1.1\r\nAccept:*.*\r\n\r\n";
			byte [] requestBytes = Encoding.UTF8.GetBytes(request);

			Assert.AreEqual("GET / HTTP/1.1", HttpRequestInfo.GetRequestLine(requestBytes));
		}

		[TestMethod]
		public void Test_HttpRequestInfo_GetRequestLine_From_Binary_NL()
		{
			string request = "GET / HTTP/1.1\nAccept:*.*\n\n";
			byte[] requestBytes = Encoding.UTF8.GetBytes(request);

			Assert.AreEqual("GET / HTTP/1.1", HttpRequestInfo.GetRequestLine(requestBytes));
		}

		[TestMethod]
		public void Test_HttpRequestInfo_ConstructPathAndQuery()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithEmptyQUery);
			Assert.AreEqual("/bank/login.aspx", reqInfo.PathAndQuery);
			reqInfo.QueryVariables.Add("z", "3");
			Assert.AreEqual("/bank/login.aspx?z=3", reqInfo.PathAndQuery);
			reqInfo.Path = "/x.jsp";
			Assert.AreEqual("/x.jsp?z=3", reqInfo.PathAndQuery);
			reqInfo.QueryVariables.Clear();
			Assert.AreEqual("/x.jsp", reqInfo.PathAndQuery);
			
		}

        [TestMethod]
        public void Test_HttpRequestInfo_AddQueryVariables()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithEmptyQUery);
            Assert.AreEqual("/bank/login.aspx", reqInfo.PathAndQuery);
            reqInfo.QueryVariables.Add("a", "1");
            Assert.AreEqual("/bank/login.aspx?a=1", reqInfo.PathAndQuery);
            reqInfo = new HttpRequestInfo(reqInfo.ToString());
            reqInfo.QueryVariables.Add("b", "2");
            Assert.AreEqual("/bank/login.aspx?a=1&b=2", reqInfo.PathAndQuery);
        }

		[TestMethod]
		public void Test_HttpRequestInfo_ConstructPostData()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo("POST / HTTP/1.1");
            
			reqInfo.BodyVariables.Add("x", "1");
			Assert.AreEqual("x=1", reqInfo.ContentDataString);

		}

		[TestMethod]
		public void Test_HttpRequestInfo_CopyHeadersAndCheckHost()
		{
			HttpRequestInfo reqInfo1 = new HttpRequestInfo("POST /1 HTTP/1.1");
			reqInfo1.Headers.Add("Host", "demo.testfire.net:81");
			HttpRequestInfo reqInfo2 = new HttpRequestInfo("POST /2 HTTP/1.1");
			reqInfo2.Headers = reqInfo1.Headers;

			Assert.AreEqual("demo.testfire.net", reqInfo2.Host);
			Assert.AreEqual(81, reqInfo2.Port);

		}

        [TestMethod]
        public void Test_HttpRequestInfo_PseudoHeaders()
        {
            HttpRequestInfo reqInfo1 = new HttpRequestInfo("POST /1 HTTP/2.0");
            reqInfo1.Headers.Add(":authority", "google.com");
            Assert.AreEqual("google.com", reqInfo1.Headers[":authority"]);
            HttpRequestInfo reqInfo2 = new HttpRequestInfo(reqInfo1.ToString());
            Assert.AreEqual("google.com", reqInfo2.Headers[":authority"]);
            Assert.AreEqual("google.com", reqInfo2.Host);

        }

        [TestMethod]
		public void Test_HttpRequestInfo_SearchRegex()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.MultipartRequest);
			Assert.AreEqual("POST (http://[^/]+)?/rafservices/EFS/put[\\s|\\?]", reqInfo.SearchRegex);
			reqInfo = new HttpRequestInfo(Resources.POSTRequest);
			Assert.AreEqual(@"POST (http://[^/]+)?/bank/login\.aspx[\s|\?]", reqInfo.SearchRegex);
		}

		[TestMethod]
		public void Test_HttpRequestInfo_MultiPart()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest);
			Assert.IsFalse(reqInfo.IsMultipart);
			reqInfo = new HttpRequestInfo(Resources.MultipartRequest);
			Assert.IsTrue(reqInfo.IsMultipart);
			Assert.AreEqual(1095, reqInfo.ContentLength);
			Assert.AreEqual(1095, reqInfo.ContentData.Length);
		}


		[TestMethod]
		public void Test_HttpRequestInfo_RequestLineProperty()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo("GET /a1 HTTP1/1");
			Assert.AreEqual("GET /a1 HTTP1/1", reqInfo.RequestLine);
			reqInfo = new HttpRequestInfo("GET HTTP://site.com/a1 HTTP1/1");
			Assert.AreEqual("GET http://site.com/a1 HTTP1/1", reqInfo.RequestLine);
			reqInfo = new HttpRequestInfo("GET HTTPs://site.com/a1 HTTP1/1");
			Assert.AreEqual("GET /a1 HTTP1/1", reqInfo.RequestLine);
		}

		[TestMethod]
		public void Test_HttpRequestInfo_FullUrl()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithQuery);
			reqInfo.Port = 8080;
			Assert.AreEqual("http://www.altoromutual.com:8080/bank/login.aspx?x=1&y=2&true", reqInfo.FullUrl);
			reqInfo.Port = 80;
			Assert.AreEqual("http://www.altoromutual.com/bank/login.aspx?x=1&y=2&true", reqInfo.FullUrl);
			reqInfo.Port = 443;
			reqInfo.IsSecure = true;
			Assert.AreEqual("https://www.altoromutual.com/bank/login.aspx?x=1&y=2&true", reqInfo.FullUrl);

		}

		[TestMethod]
        public void Test_HttpRequestInfo_QueryParsing()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithQuery);
			string expectedQueryString = "x=1&y=2&true";
			string expectedPathAndQuery = "/bank/login.aspx?x=1&y=2&true";

			int expectedQueryParamsCount = 3;

			Assert.AreEqual(expectedQueryString, reqInfo.QueryString);
			Assert.AreEqual(expectedPathAndQuery, reqInfo.PathAndQuery);
			Assert.AreEqual(expectedQueryParamsCount, reqInfo.QueryVariables.Count);

			Assert.AreEqual("1", reqInfo.QueryVariables["x"]);
			Assert.AreEqual("2", reqInfo.QueryVariables["y"]);
			Assert.AreEqual("", reqInfo.QueryVariables["true"]);
			
			//update the variables
			reqInfo.QueryVariables["x"] = "111";
			reqInfo.QueryVariables["Y"] = "";
			expectedQueryString = "x=111&y=&true";
			Assert.AreEqual(expectedQueryString, reqInfo.QueryString);

		}

        [TestMethod]
        public void Test_HttpRequestInfo_QueryParameters_QRadarBug()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo("GET /console/do/core/genericsearchlist?appName=assets&pageId=VaScannerSchedulesList&countDown=true&columnSorting=true&orderBy=priority&sorting=asc&pageNumber=1 HTTP/1.1\r\n",true);
            string newRawReqInfo = reqInfo.ToString();
            reqInfo = new HttpRequestInfo(newRawReqInfo,true);


            Assert.AreEqual( "true",reqInfo.QueryVariables["columnSorting"]);

        }


		[TestMethod]
		public void Test_HttpRequestInfo_QueryParsingNoQuery()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithEmptyQUery);
			string expectedQueryString = "";
			string expectedPathAndQuery = "/bank/login.aspx";

			int expectedQueryParamsCount = 0;

			Assert.AreEqual(expectedQueryString, reqInfo.QueryString);
			Assert.AreEqual(expectedPathAndQuery, reqInfo.PathAndQuery);
			Assert.AreEqual(expectedQueryParamsCount, reqInfo.QueryVariables.Count);


		}


		[TestMethod]
		public void Test_HttpRequestInfo_PostParsing()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest);
			string expectedPostData = "uid=jsmith&passwd=Demo1234";
			

			int expectedParamsCount = 2;

			Assert.AreEqual(expectedPostData, reqInfo.ContentDataString);
			
			Assert.AreEqual(expectedParamsCount, reqInfo.BodyVariables.Count);


		}



        [TestMethod]
        public void Test_HttpRequestInfo_VariableInfoBodyParameters()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest);
            List<HttpVariableInfo> varInfoCollection = reqInfo.BodyVariables.GetVariableInfoCollection();

            int expectedParamsCount = 2;

            Assert.AreEqual(expectedParamsCount, varInfoCollection.Count);

            int ord = 0;

            Assert.AreEqual("uid", varInfoCollection[ord].Name);
            Assert.AreEqual("jsmith", varInfoCollection[ord].Value);
            Assert.AreEqual("Regular", varInfoCollection[ord].Type);
            Assert.AreEqual(RequestLocation.Body, varInfoCollection[ord].Location);
            Assert.AreEqual(false, varInfoCollection[ord].IsTracked);

            ord = 1;

            Assert.AreEqual("passwd", varInfoCollection[ord].Name);
            Assert.AreEqual("Demo1234", varInfoCollection[ord].Value);
            Assert.AreEqual("Regular", varInfoCollection[ord].Type);
            Assert.AreEqual(RequestLocation.Body, varInfoCollection[ord].Location);
            Assert.AreEqual(false, varInfoCollection[ord].IsTracked);


            


        }


        [TestMethod]
        public void Test_HttpRequestInfo_VariableInfoCookies()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest);
            List<HttpVariableInfo> varInfoCollection = reqInfo.Cookies.GetVariableInfoCollection();
            int expectedParamsCount = 2;

            Assert.AreEqual(expectedParamsCount, varInfoCollection.Count);

            

            Assert.AreEqual("ASP.NET_SessionId", varInfoCollection[0].Name);
            Assert.AreEqual("gyixpjmizsnswazv5zunxial", varInfoCollection[0].Value);
            Assert.AreEqual("Regular", varInfoCollection[0].Type);
            Assert.AreEqual(RequestLocation.Cookies, varInfoCollection[0].Location);
            Assert.AreEqual(true, varInfoCollection[0].IsTracked);

            Assert.AreEqual("amSessionId", varInfoCollection[1].Name);
            Assert.AreEqual("1975473225", varInfoCollection[1].Value);
            Assert.AreEqual("Regular", varInfoCollection[1].Type);
            Assert.AreEqual(RequestLocation.Cookies, varInfoCollection[1].Location);
            Assert.AreEqual(true, varInfoCollection[1].IsTracked);


        }


        [TestMethod]
        public void Test_HttpRequestInfo_VariableInfoQueryParameters()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithQuery);
            List<HttpVariableInfo> varInfoCollection = reqInfo.QueryVariables.GetVariableInfoCollection();

            int expectedParamsCount = 3;

            Assert.AreEqual(expectedParamsCount, varInfoCollection.Count);

           


            Assert.AreEqual("x", varInfoCollection[0].Name);
            Assert.AreEqual("1", varInfoCollection[0].Value);
            Assert.AreEqual("Regular", varInfoCollection[0].Type);
            Assert.AreEqual(RequestLocation.Query, varInfoCollection[0].Location);
            Assert.AreEqual(false, varInfoCollection[0].IsTracked);

            Assert.AreEqual("y", varInfoCollection[1].Name);
            Assert.AreEqual("2", varInfoCollection[1].Value);
            Assert.AreEqual("Regular", varInfoCollection[1].Type);
            Assert.AreEqual(RequestLocation.Query, varInfoCollection[1].Location);
            Assert.AreEqual(false, varInfoCollection[1].IsTracked);


            Assert.AreEqual("true", varInfoCollection[2].Name);
            Assert.AreEqual("", varInfoCollection[2].Value);
            Assert.AreEqual("Regular", varInfoCollection[2].Type);
            Assert.AreEqual(RequestLocation.Query, varInfoCollection[2].Location);
            Assert.AreEqual(false, varInfoCollection[2].IsTracked);


        }


		[TestMethod]
		public void Test_HttpRequestInfo_JSONPostParsing()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.JSONPostRequest);
			string expectedPostData = "{\"uid\":\"jsmith\", \"passwd\":\"Demo1234\"}"; 


			int expectedParamsCount = 2;

			Assert.AreEqual(expectedPostData, reqInfo.ContentDataString);


			Assert.AreEqual(expectedParamsCount, reqInfo.BodyVariables.Count);


		}

        [TestMethod]
        public void Test_HttpRequestInfo_EncJSONPostParsing()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.encJSONPostRequest);
            string expectedPostData = "{method%3A%22getAlertMessages%22%2Cparams%3Anull%2CCSRF%3A%2297199c9c-5df4-46cd-b0f1-f4fbc8d2381c%22%2Cid%3A%2285%22}";


            int expectedParamsCount = 4;

            Assert.AreEqual(expectedPostData, reqInfo.ContentDataString.Trim());

            Assert.AreEqual(expectedParamsCount, reqInfo.BodyVariables.Count);


        }


		[TestMethod]
		public void Test_HttpRequestInfo_SOAPPostParsing()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.SOAPRequest);
			string expectedPostData = Resources.SOAPPost;


			int expectedParamsCount = 2;

			Assert.AreEqual(expectedPostData, reqInfo.ContentDataString);

			Assert.AreEqual(expectedParamsCount, reqInfo.BodyVariables.Count);


		}


		[TestMethod]
		public void Test_HttpRequestInfo_CookieParsing()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest);

            string expectedCookieHeader = "ASP.NET_SessionId=1234; amSessionId=1975473225";

			reqInfo.SetCookie("ASP.NET_SessionId", "1234");

			int expectedCookiesCount = 2;

			Assert.AreEqual(expectedCookieHeader, reqInfo.Headers["Cookie"]);

			Assert.AreEqual(expectedCookiesCount, reqInfo.Cookies.Count);

            expectedCookieHeader = "ASP.NET_SessionId=; amSessionId=1975473225; MyCookie=111";
			reqInfo.SetCookie("ASP.NET_SessionId", "");
			reqInfo.SetCookie("MyCookie", "111");

			Assert.AreEqual(expectedCookieHeader, reqInfo.Headers["Cookie"]);
		}

		[TestMethod]
		public void Test_HttpRequestInfo_SetCookie()
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.RequestWithQuery);

			Assert.AreEqual(0, reqInfo.Cookies.Count);

			reqInfo.SetCookie("a", "1");

			Assert.AreEqual("a=1", reqInfo.Headers["Cookie"]);

			reqInfo.SetCookie("b", "2");

			Assert.AreEqual("a=1; b=2", reqInfo.Headers["Cookie"]);

			reqInfo.SetCookie("a", "");

			Assert.AreEqual("a=; b=2", reqInfo.Headers["Cookie"]);
		}


        [TestMethod]
        public void Test_HttpRequestInfo_ParsingWithoutParameters()
        {
            HttpRequestInfo reqInfo = new HttpRequestInfo(Resources.POSTRequest,false);

            Assert.AreEqual(Resources.POSTRequest, reqInfo.ToString());
        }

 
        [TestMethod]
        public void Test_HttpRequestInfo_Hard2Scan()
        {
            SdkSettings.Instance.VariableDefinitions.Add(new HttpVariableDefinition(
                "Hard2Scan",RequestLocation.Body,"([^][,]+)[^][]?\\[([^][]+)"));
            HttpRequestInfo reqInfo = new HttpRequestInfo("POST /survey_questions.php HTTP/1.1\r\n\r\nimpressed[1], [Next], [Cancel], target_page[4]", true);

            Assert.AreEqual(2, reqInfo.BodyVariables.Count, "Incorrect number of echo variables");
            Assert.AreEqual("1", reqInfo.BodyVariables["impressed"], "Incorrect value for impressed variable");
            Assert.AreEqual("4", reqInfo.BodyVariables["target_page"], "Incorrect value for target_page variable");

        }
	}
}
