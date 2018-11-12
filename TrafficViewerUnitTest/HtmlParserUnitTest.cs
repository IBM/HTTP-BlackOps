using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Xml;
using TVHtmlParser;
using System.IO;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class HtmlParserUnitTest
	{
		private static XmlDocument Parse(string html)
		{
			HtmlParser parser = new HtmlParser();
			XmlDocument doc;

			parser.Parse(html, out doc);

			return doc;
		}



		[TestMethod]
		public void TestSingleOpenEndedTag()
		{
			string html = "<html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html /></root>", doc.InnerXml);

		}


		[TestMethod]
		public void TestOpenAndEndSingleTag()
		{
			string html = "<html></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html /></root>", doc.InnerXml);

		}

		[TestMethod]
		public void TestOpenAndEndSingleTagWithSomeInnerText()
		{
			string html = "<html>Some Inner Text</html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html>Some Inner Text</html></root>", doc.InnerXml);

		}

		[TestMethod]
		public void TestTwoNestedTags()
		{
			string html = "<html><head></head></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><head /></html></root>", doc.InnerXml);

		}

		[TestMethod]
		public void TestTwoTagsNestedInOneTag()
		{
			string html = "<html><head></head><body></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><head /><body /></html></root>", doc.InnerXml);

		}

		[TestMethod]
		public void TestNestedTagWithAttributes()
		{
			string html = "<html><head><script type=\"javascript\" src=\"functions.js\"></script></head><body></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><head><script type=\"javascript\" src=\"functions.js\" /></head><body /></html></root>", doc.InnerXml);

		}

		[TestMethod]
		public void TestNestedTagWithAttributesAndInnerText()
		{
			string html = "<html><head><script type=\"javascript\" src=\"functions.js\"></script></head><body><script>alert(1)</script></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><head><script type=\"javascript\" src=\"functions.js\" /></head><body><script>alert(1)</script></body></html></root>", doc.InnerXml);

		}


		[TestMethod]
		public void TestNestedTagWithAttributesAndInnerTextAndFormatting()
		{
			string html = "\r\n\r\n<html>         \r\n\t<head>\r\n\t\t<script type=\"javascript\" src=\"functions.js\"></script>\r\n\t</head>\r\n\t<body>\r\n\t\t<script>alert(1)</script>\r\n\t</body>\r\n</html>";
			XmlDocument doc = Parse(html);

			XmlNode body = doc.SelectSingleNode("//body");

			Assert.IsNotNull(body);

			XmlNode script = body.SelectSingleNode("script");

			Assert.IsNotNull(script);
			Assert.AreEqual("alert(1)", script.InnerText);

		}

		[TestMethod]
		public void TestIncorrectTagsOneInputWithNoClosingTags()
		{
			string html = "<html><body><input name=\"user\"></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user\" /></body></html></root>", doc.InnerXml);
		}

		[TestMethod]
		public void TestIncorrectTagsTwoInputsWithNoClosingTags()
		{
			string html = "<html><body><input name=\"user\"><input name=\"pass\"></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user\" /><input name=\"pass\" /></body></html></root>", doc.InnerXml);
		}

		[TestMethod]
		public void TestIncorrectAttributesOneInputWithNoClosingTagsNoQuotes()
		{
			string html = "<html><body><input name=user></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user\" /></body></html></root>", doc.InnerXml);
		}

		[TestMethod]
		public void TestIncorrectAttributesSingleQuoteWithDoubleQuotesValue()
		{
			string html = "<html><body><input name='user\"jsmith\"'></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user&quot;jsmith&quot;\" /></body></html></root>", doc.InnerXml);
		}


		[TestMethod]
		public void TestAttributeWithNoValue()
		{
			string html = "<html><body><input selected/></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input selected=\"\" /></body></html></root>", doc.InnerXml);
		}

		[TestMethod]
		public void TestCorrectAttributes()
		{
			string html = "<html><body><input name=\"user\" id =\"user\" value=\"jsmith\"></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user\" id=\"user\" value=\"jsmith\" /></body></html></root>", doc.InnerXml);
		}

		[TestMethod]
		public void TestIncorrectAttributesOneCorrectTwoIncorrect()
		{
			string html = "<html><body><input name=user id = \"user\"  value = jsmith></body></html>";
			XmlDocument doc = Parse(html);

			Assert.AreEqual("<root><html><body><input name=\"user\" id=\"user\" value=\"jsmith\" /></body></html></root>", doc.InnerXml);
		}

	}
}
