using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVHtmlParser
{
	/// <summary>
	/// Responsible for parsing 
	/// </summary>
	internal class HtmlInnerTextParser:BaseHtmlElementParser
	{

		public override void Parse(XmlDocument doc, string rawHtml, ref int index, ref XmlNode parentNode, ref XmlNode currNode)
		{
			try
			{
				StringBuilder innerText = new StringBuilder(ESTIMATED_INNER_TEXT_LEN);
				int n = rawHtml.Length;
				while (index < n && rawHtml[index] != OPEN_TAG)
				{
					char c = rawHtml[index];
					innerText.Append(c);
					index++;
				}

				if (innerText.Length > 0 && currNode.InnerText.Length == 0)
				{
					currNode.InnerText = innerText.ToString();
				}

			}
			catch { } //the html parser is very permissive
		}
	}
}
