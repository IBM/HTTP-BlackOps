using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace TVHtmlParser
{
	/// <summary>
	/// Responsible for parsing a node
	/// </summary>
	internal class HtmlNodeParser : BaseHtmlElementParser
	{


		public override void Parse(XmlDocument doc, string rawHtml, ref int index, ref XmlNode parentNode, ref XmlNode currNode)
		{
			try
			{
				int n = rawHtml.Length;
				StringBuilder tagText = new StringBuilder(ESTIMATED_TEXT_LEN);
				XmlNode tempCurrNode = null;//variable used to store the current node
				bool isClosed = false;//flag indicating if the node is closed
				bool isCloseForExistingNode = false;//flag indicating if this is a close node for a node that we already read

				SkipWhiteSpace(rawHtml, ref index);

				while (index < n)
				{
					char c = rawHtml[index];
					if (c == '!' && tagText.Length == 0)
					{
						SkipNode(rawHtml, ref index); //skip nodes that start with <!
						break;
					}

					if (IsNameCharacter(c))
					{
						if (tempCurrNode != null)
						{
							//we read the node name and now we are reading a attribute 
							IHtmlElementParser attributeParser = new HtmlAttributeParser();
							attributeParser.Parse(doc, rawHtml, ref index, ref parentNode, ref tempCurrNode);
						}
						else
						{
							tagText.Append(c);
						}
					}
					else if (c == FW_SLASH) //this is a closed node, either closing a previous node or closing itself
					{
						isClosed = true;
						if (tagText.Length == 0)
						{
							isCloseForExistingNode = true;
						}
					}
					else if (Char.IsSeparator(c) || c == CLOSE_TAG)
					{
						if (tempCurrNode == null && tagText != null && tagText.Length > 0) //create the current node if it doesn't exist already using tagText
						{
							tempCurrNode = doc.CreateElement(tagText.ToString());
						}

						if (c == CLOSE_TAG)
						{
							break;
						}
					}

					index++;
				}


				//finished reading a node

				if (tempCurrNode != null)
				{
					if (isClosed)
					{
						if (!isCloseForExistingNode) //this node is <nodeName...attributes... />
						{
							parentNode.AppendChild(tempCurrNode);
						}
						else //this node is </someParentNode>
						{
							XmlNode navNode = parentNode;
							//find the parent
							while (navNode.ParentNode != null && String.Compare(navNode.Name, tempCurrNode.Name, true) != 0)
							{
								navNode = navNode.ParentNode;
							}

							if (navNode.ParentNode != null)
							{
								//set the parent node to be the parent of the recently closed node otherwise the tag is invalid and should be skipped
								parentNode = navNode.ParentNode;
							}
						}
					}
					else //this is <node ...attributes>..innerContent
					{						
						parentNode.AppendChild(tempCurrNode);
						//this node now becomes the parent node
						if (TAGS_WITHOUT_INNER_HTML.IsMatch(tempCurrNode.Name) == false)
						{
							parentNode = tempCurrNode;
						}
						currNode = tempCurrNode;
					}
				}

				index++;
			}
			catch { }
		}
	}
}
