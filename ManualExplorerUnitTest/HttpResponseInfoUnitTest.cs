using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;
using ManualExplorerUnitTest.Properties;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpResponseInfoUnitTest
	{
		[TestMethod]
		public void TestGetResponseStatus()
		{
			Assert.AreEqual("200", HttpResponseInfo.GetResponseStatus("HTTP/1.1 200 OK"));
		}

		[TestMethod]
		public void TestGetResponseStatusFromBytes()
		{
			Assert.AreEqual("200", HttpResponseInfo.GetResponseStatus(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK")));
		}

		[TestMethod]
		public void TestGetResponseStatusFromEmptyBytes()
		{
			Assert.AreEqual(String.Empty, HttpResponseInfo.GetResponseStatus(Encoding.UTF8.GetBytes(String.Empty)));
		}

        [TestMethod]
        public void TestGetAltoroNonChunkedResponseWithChunkedHeader()
        {
            HttpResponseInfo respInfo = new HttpResponseInfo(Resources.AltoroDifChunkedResponse);
            Assert.AreNotEqual(0, respInfo.ResponseBody.Length);
        }
	}
}
