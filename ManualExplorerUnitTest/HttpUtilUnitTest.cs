using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Http;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpUtilUnitTest
	{
		[TestMethod]
		public void Test_HttpUtil_ParseRange_bytes_from_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("bytes=256-1024", out specifier, out from, out to);

			Assert.AreEqual("bytes", specifier);
			Assert.AreEqual(256, from);
			Assert.AreEqual(1024, to);
		
		}

		[TestMethod]
		public void Test_HttpUtil_ParseRange_bytes_empty_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("bytes=-1024", out specifier, out from, out to);

			Assert.AreEqual("bytes", specifier);
			Assert.AreEqual(0, from);
			Assert.AreEqual(1024, to);
		}


		[TestMethod]
		public void Test_HttpUtil_ParseRange_bytes_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("bytes = 1024", out specifier, out from, out to);

			Assert.AreEqual("bytes", specifier);
			Assert.AreEqual(0, from);
			Assert.AreEqual(1024, to);
		}

		[TestMethod]
		public void Test_HttpUtil_ParseRange_items_from_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("items=0-50", out specifier, out from, out to);

			Assert.AreEqual("items", specifier);
			Assert.AreEqual(0, from);
			Assert.AreEqual(50, to);
		}

		[TestMethod]
		public void Test_HttpUtil_ParseRange_nospecifier_from_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("0-50", out specifier, out from, out to);

			Assert.AreEqual("bytes", specifier);
			Assert.AreEqual(0, from);
			Assert.AreEqual(50, to);
		}

		[TestMethod]
		public void Test_HttpUtil_ParseRange_nospecifier_to()
		{
			string specifier;
			int from;
			int to;

			HttpUtil.ParseRange("50", out specifier, out from, out to);

			Assert.AreEqual("bytes", specifier);
			Assert.AreEqual(0, from);
			Assert.AreEqual(50, to);
		}
	
	}
}
