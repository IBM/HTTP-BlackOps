using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class UriHttpRequestInfoUnitTest
	{

		private void TestUri(string uri)
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(String.Format("GET {0} HTTP/1.1\r\n", uri));
			Uri netUri = new Uri(uri);
			Uri compareUri = new Uri(reqInfo.FullUrl);
			
			//verify the parsing of HttpRequestInfo and the parsing of .Net Uri matches for most uris
			Assert.AreEqual(0, Uri.Compare(netUri, compareUri, UriComponents.Host, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase), "Hosts don't match");
			Assert.AreEqual(netUri.Port, reqInfo.Port);
			Assert.AreEqual(netUri.Scheme == "https", reqInfo.IsSecure);
			Assert.AreEqual(netUri.PathAndQuery, reqInfo.PathAndQuery);
			Assert.AreEqual(0, Uri.Compare(netUri, compareUri, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase), "Full Urls don't match");
		}

		
		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_domain_root()
		{
			TestUri("http://demo.testfire.net/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_root()
		{
			TestUri("https://demo.testfire.net/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_unc_root()
		{
			TestUri("http://demo/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_domain_port_root()
		{
			TestUri("http://demo.testfire.net:8080/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_port_root()
		{
			TestUri("https://demo.testfire.net:9443/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_http_port_root()
		{
			TestUri("https://demo.testfire.net:80/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_ipv4_root()
		{
			TestUri("http://127.0.0.1/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_ipv4_port_root()
		{
			TestUri("http://127.0.0.1:8080/");
		}



		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_ipv6_root()
		{
			TestUri("http://[2002:917:33da::917:33da]/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_ipv6_port_root()
		{
			TestUri("http://[2002:917:33da::917:33da]:8080/");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTP_ipv6_port_root_query_with_column()
		{
			TestUri("http://[2002:917:33da::917:33da]:8080/?a:1");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_http_path_query()
		{
			TestUri("http://demo.testfire.net/a?b=1");
		}


		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_http_pathjsessionid()
		{
			TestUri("http://demo.testfire.net/a;jsessionid=1234");
		}

		[TestMethod]
		public void Test_Uri_HttpRequestInfo_HTTPS_domain_http_pathcookieless()
		{
			TestUri("http://demo.testfire.net:80/(S(asdgasghfsdhsdffhs))/a");
		}
	}
}
