using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVHtmlParser
{
	/// <summary>
	/// Implements a simple html parser that simply converts relevant html tags into a XML document.
	/// This was specifically created for DOM Uniqueness calculation but can be used in other purposes
	/// </summary>
	public class HtmlParser : BaseHtmlElementParser
	{
		/// <summary>
		/// Parses Html into XmlDocument
		/// </summary>
		/// <param name="html"></param>
		/// <param name="doc">The resulting xml</param>
		public void Parse(string html, out XmlDocument doc)
		{
			//initialize variables
			doc = new XmlDocument();
            doc.XmlResolver = null;
			XmlNode parentNode = doc.CreateElement("root");
			XmlNode currNode = doc.CreateElement("DummyNode");
			doc.AppendChild(parentNode);
			int index = 0;
			//start the parsing operation
			Parse(doc, html, ref index,ref parentNode, ref currNode);

		}

		/// <summary>
		/// Parsing logic
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="rawHtml"></param>
		/// <param name="index"></param>
		/// <param name="parentNode"></param>
		public override void Parse(XmlDocument doc, string rawHtml, ref int index,ref XmlNode parentNode,ref XmlNode currNode)
		{
			SkipWhiteSpace(rawHtml,ref index);

			//keep parsing nodes until the end of string

			int n = rawHtml.Length;

			while (index < n)
			{
				IHtmlElementParser currentParser;
				if (rawHtml[index] == OPEN_TAG)
				{
					currentParser = new HtmlNodeParser();
				}
				else
				{
					currentParser = new HtmlInnerTextParser();
				}

				currentParser.Parse(doc, rawHtml, ref index,ref parentNode, ref currNode);
			}
		}
	}
}
