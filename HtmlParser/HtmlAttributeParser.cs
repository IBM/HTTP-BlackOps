using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVHtmlParser
{
	internal class HtmlAttributeParser : BaseHtmlElementParser
	{
		private void ParseName(string rawHtml, ref int index, out string name)
		{

			int len = rawHtml.Length;
			char c;
			StringBuilder attrName = new StringBuilder(ESTIMATED_TEXT_LEN);

			while (index < len)
			{
				c = rawHtml[index];
				if (IsNameCharacter(c))
				{
					attrName.Append(c);
					index++;
				}
				else
				{
					break;
				}
			}

			name = attrName.ToString();
		}


		private void ParseValue(string rawHtml, char delimiter, ref int index, out string value)
		{
			StringBuilder attrValue = new StringBuilder(ESTIMATED_TEXT_LEN);

			int len = rawHtml.Length;
			char c;

			while (index < len)
			{
				c = rawHtml[index];
				if (delimiter == NULL_CHAR)
				{

					if (c == CLOSE_TAG)
					{
						index--; //go back one char to allow the CLOSE tag to be processed by the node parser
						break;
					}
					else if (Char.IsSeparator(c))
					{
						break;
					}
					
				}
				else if(c == delimiter)
				{
					break;
				}
				
				attrValue.Append(c);
				index++;
			}

			value = attrValue.ToString();
		}


		public override void Parse(XmlDocument doc, string rawHtml, ref int index, ref XmlNode parentNode, ref XmlNode currNode)
		{
			try
			{

				XmlAttribute currAttr = null;

				int len = rawHtml.Length;
				char c;
				string name;

				ParseName(rawHtml, ref index, out name);

				if (!String.IsNullOrEmpty(name))
				{
					currAttr = doc.CreateAttribute(name);
					currNode.Attributes.Append(currAttr);
					SkipWhiteSpace(rawHtml, ref index);
					c = rawHtml[index];
					if (c != EQUAL)
					{
						//attribute with no value, decrement the index and return
						index--;
					}
					else
					{
						index++;//go to the next char after EQUAL
						//search for the quotes
						SkipWhiteSpace(rawHtml, ref index);

						c = rawHtml[index];
						if (c != S_QUOTE && c != D_QUOTE)
						{
							//this is an attribute without quotes
							c = NULL_CHAR;
						}
						else
						{
							index++;
						}
						string value;
						//parse the value with c as separator (if c is not single quote or double quote c is null)
						ParseValue(rawHtml, c, ref index, out value);
						currAttr.Value = value;
					}
				}

			}
			catch { }
		}
	}
}
