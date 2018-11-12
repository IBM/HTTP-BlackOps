using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace TVHtmlParser
{
	/// <summary>
	/// Interface used by all html parser elements
	/// </summary>
	public interface IHtmlElementParser
	{
		/// <summary>
		/// Parses raw content into xml
		/// </summary>
		/// <param name="rawHtml"></param>
		/// <param name="index">The current index in the string</param>
		/// <param name="parentNode">The current parent node</param>
		/// <param name="currNode">The current node</param>
		void Parse(XmlDocument doc, string rawHtml, ref int index,ref XmlNode parentNode, ref XmlNode currNode);
	}

	/// <summary>
	/// Base class for all htmlparser elements
	/// </summary>
	public abstract class BaseHtmlElementParser:IHtmlElementParser
	{

		protected Regex TAGS_WITHOUT_INNER_HTML = new Regex(@"\b(input|br|hr|meta|link)\b",RegexOptions.IgnoreCase);


		#region Constants

		protected const char OPEN_TAG = '<';
		protected const char CLOSE_TAG = '>';
		protected const char FW_SLASH = '/';
		protected const char BK_SLASH = '\\';
		protected const char D_QUOTE = '"';
		protected const char S_QUOTE = '\'';
		protected const char EQUAL = '=';
		protected const char SPACE = ' ';
		protected const char TAB = '\t';
		protected const char NL = '\n';
		protected const char CR = '\r';
		protected const char UNDERSCORE = '_';
		protected const char NULL_CHAR = '\0';

		protected const int ESTIMATED_INNER_TEXT_LEN = 255;
		protected const int ESTIMATED_TEXT_LEN = 25;

		#endregion

		#region Utils for children classes

		protected bool IsNameCharacter(char c)
		{
			return Char.IsLetter(c) || Char.IsDigit(c) || c == UNDERSCORE;
		}

		/// <summary>
		/// Skips white characters
		/// </summary>
		/// <param name="rawHtml"></param>
		/// <param name="index"></param>
		protected void SkipWhiteSpace(string rawHtml, ref int index)
		{ 
			int n=rawHtml.Length;
			while (index < n && Char.IsSeparator(rawHtml[index]))
			{
				index++;
			}
		}

		/// <summary>
		/// Skips a node advancing the position to the character that follows
		/// </summary>
		/// <param name="rawHtml"></param>
		/// <param name="index"></param>
		protected void SkipNode(string rawHtml, ref int index)
		{
			int n = rawHtml.Length;
			int tagCounter = 1;//counts the open/end pairs, we start with an open pair
			while (index < n && tagCounter > 0)
			{ 
				char c = rawHtml[index];
				if (c == OPEN_TAG)
				{
					tagCounter++;
				}
				else if (c == CLOSE_TAG)
				{
					tagCounter--;
				}
				index++;
			}
		}
		#endregion

		#region IElementParser Members

		/// <summary>
		/// Parses raw content into xml
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="rawHtml"></param>
		/// <param name="index"></param>
		/// <param name="parentNode"></param>
		/// <param name="currNode"></param>
		public abstract void Parse(XmlDocument doc, string rawHtml, ref int index, ref XmlNode parentNode, ref XmlNode currNode);
		
		#endregion
	}

	

}
