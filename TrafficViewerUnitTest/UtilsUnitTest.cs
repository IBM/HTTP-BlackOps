/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using System.IO;
using TrafficViewerUnitTest.Properties;
using System.Net;
using System.Diagnostics;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class UtilsUnitTest
	{
        


		[TestMethod]
		public void TrafficViewerEncoder_GetString()
		{ 
			TrafficViewerEncoding enc = new TrafficViewerEncoding();
			string result = enc.GetString(new byte[3] { 49, 0, 50 });
			Assert.AreEqual(@"1\x00002", result);



		}

        
		
		[TestMethod]
		public void TrafficViewerEncoder_GetBytes()
		{
			TrafficViewerEncoding enc = new TrafficViewerEncoding();
			byte [] expected = new byte[3] { 49, 0, 50 };
			byte [] result = enc.GetBytes(@"1\x00002");
			Assert.AreEqual(expected.Length, result.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], result[i]);
			}

			string expectedS = "HTTP 200 OK\r\n<r>1</r>";
			result = enc.GetBytes(expectedS);
			Assert.AreEqual(enc.GetString(result), expectedS);

		}

		[TestMethod]
		public void TrafficViewerEncoder_GetBytesWrongSlash()
		{
			TrafficViewerEncoding enc = new TrafficViewerEncoding();
			byte[] expected = new byte[3] { 49, 92, 50 };
			byte[] result = enc.GetBytes(@"1\2");
			Assert.AreEqual(expected.Length, result.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], result[i]);
			}

			string resultS = enc.GetString(result);
			Assert.AreEqual(@"1\2", resultS);
		}

		[TestMethod]
		public void CompressDecompressBase64Test()
		{
			string expected = Constants.DefaultEncoding.GetString(Resources.Regular_Scan);
			string compressed = Utils.CompressToBase64String(expected);
			Assert.IsNotNull(compressed);
			string result = Utils.DecompressFromBase64String(compressed);
			Assert.AreEqual(expected, result);
		}



		[TestMethod]
		public void TestReadLineEmptyLine()
		{
			byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
			MemoryStream stream = new MemoryStream(bytes);
			byte[] line = Utils.ReadLine(stream, 1024, LineEnding.Any);
			Assert.IsNotNull(line);
			Assert.AreEqual(0, line.Length);
		}

		[TestMethod]
		public void TestReadLineEmptyLineFollowedByText()
		{
			byte[] bytes = Encoding.UTF8.GetBytes("\r\nline2");
			MemoryStream stream = new MemoryStream(bytes);
			byte[] line = Utils.ReadLine(stream, 1024);
			Assert.IsNotNull(line);
			Assert.AreEqual(0, line.Length);
		}


		[TestMethod]
		public void TestReadLineLastLine()
		{
			byte[] bytes = Encoding.UTF8.GetBytes("\r\nline2");
			MemoryStream stream = new MemoryStream(bytes);
			Utils.ReadLine(stream, 1024);
			byte[] line = Utils.ReadLine(stream, 1024);
			Assert.IsNotNull(line);
			Assert.AreEqual("line2", Encoding.UTF8.GetString(line));
		}

		[TestMethod]
		public void TestAddSlashes()
		{
			string escaped = Utils.AddSlashes("alert(\"\")");
			Assert.AreEqual("alert(\\\"\\\")", escaped);
			escaped = Utils.AddSlashes("alert('')");
			Assert.AreEqual("alert(\\\'\\\')", escaped);
			escaped = Utils.AddSlashes("/Product.do?serch_sec=>\"'><script>alert(83893)</script>&");
			Assert.AreEqual("/Product.do?serch_sec=>\\\"\\\'><script>alert(83893)</script>&", escaped);
			escaped = Utils.AddSlashes("/Product.do?serch_sec=>\\\"'><script>\r\nalert(83893)</script>&");
			Assert.AreEqual("/Product.do?serch_sec=>\\\\\\\"\\\'><script>\\r\\nalert(83893)</script>&", escaped);
		}

		[TestMethod]
		public void TestSelectedUrlEncode()
		{
			string escaped = Utils.UrlEncodeTags("myPage?param=alert<script></script>");
			Assert.AreEqual("myPage?param=alert%3cscript%3e%3c/script%3e", escaped);
		}

        [TestMethod]
        public void Base64DecodeSimple()
        {
            string control = "The quick brown fox jumps over the lazy rabbit";
            Assert.AreEqual(control, Utils.Base64Decode("VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IHJhYmJpdA=="));
        }

        [TestMethod]
        public void Base64DecodeNoEquals()
        {
            string control = "The quick brown fox jumps over the lazy rabbit";
            Assert.AreEqual(control, Utils.Base64Decode("VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IHJhYmJpdA"));
        }

        [TestMethod]
        public void Base64DecodeTrim()
        {
            string control = "The quick brown fox jumps over the lazy rabbit";
            Assert.AreEqual(control, Utils.Base64Decode(" VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IHJhYmJpdA "));
        }

        [TestMethod]
        public void Base64DecodeDot()
        {
            string control = "The quick brown fox jumps over the lazy rabbit";
            Assert.AreEqual(control, Utils.Base64Decode("VGhlIHF1aWNrIGJyb3duIGZveA.anVtcHMgb3ZlciB0aGUgbGF6eSByYWJiaXQ"));
        }

        [TestMethod]
        public void EncryptorTest()
        {
            string control = "The quick brown fox jumps over the lazy rabbit";
            string enc = Encryptor.EncryptToString(control);
            string dec = Encryptor.DecryptToString(enc);
            Assert.AreEqual(control, dec);
        }


        [TestMethod]
		public void TestExtractPathAndQuery()
		{
			string originalPathAndQuery = "/index.jsp?q=1&x=2";
			string path;
			string query;
			Utils.ExtractPathAndQuery(originalPathAndQuery, out path, out query);

			Assert.AreEqual("/index.jsp", path);
			Assert.AreEqual("q=1&x=2", query);

			originalPathAndQuery = "/index.jsp";

			Utils.ExtractPathAndQuery(originalPathAndQuery, out path, out query);

			Assert.AreEqual("/index.jsp", path);
			Assert.AreEqual("", query);

			originalPathAndQuery = "/index.jsp?";

			Utils.ExtractPathAndQuery(originalPathAndQuery, out path, out query);

			Assert.AreEqual("/index.jsp", path);
			Assert.AreEqual("", query);

			originalPathAndQuery = "";

			Utils.ExtractPathAndQuery(originalPathAndQuery, out path, out query);

			Assert.AreEqual("", path);
			Assert.AreEqual("", query);
		}

	}
}
