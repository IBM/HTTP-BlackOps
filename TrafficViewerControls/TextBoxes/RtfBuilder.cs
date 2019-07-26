using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TVDiff;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using System.ComponentModel;
using TrafficViewerInstance;

namespace TrafficViewerControls.TextBoxes
{
	//This file contains a set of utils using in RTF generation

	/// <summary>
	/// Used to generate RTF directly from the byte[] request/responses for 
	/// high speed display of large responses
	/// </summary>
	public class RtfBuilder
	{
		/// <summary>
		/// This is a rtf header for a courier new rich textbox
		/// </summary>
		private const string RTF_HEADER1 = @"{\rtf1\ansi\deff0";
		private const string RTF_FONT_TABLE = @"{\fonttbl{\f0\fnil\fcharset0 Courier New;}}";
		private const string RTF_HEADER2 = @"\viewkind4\uc1\pard\lang1033\f0\fs17 ";

		private const string RTF_FOOTER = "\\par\r\n}";
		private const char NL = '\n';
		private const char CR = '\r';
		private const char TB = '\t';

		private bool _cancelRequested = false;

		public bool CancelRequested
		{
			get { return _cancelRequested; }
			set { _cancelRequested = value; }
		}


		/// <summary>
		/// Converts a byte array into an RTF for the raw traffic view
		/// </summary>
		/// <param name="text"></param>
		/// <param name="highlights">List of highlights to be applied to this Rtf</param>
		/// <returns>Rtf formatted string</returns>
		public string Convert(string text, params RtfHighlight[] highlights)
		{
			_cancelRequested = false;

			int i, n = text.Length;
			int lastBreakIndex = 0; //maintains the index following the last break in the text
			Dictionary<Color, int> colorMap = new Dictionary<Color, int>(); //holds color map for the rtf

			StringBuilder sb = new StringBuilder(n);

			sb.Append(RTF_HEADER1);
			sb.Append(RTF_FONT_TABLE);
			//if there is highlighting define a color table
			highlights = SortHighlights(highlights);

			int hIndex = 0; //maintains the index of the last processed highlight
			int hLen = highlights.Length;

			//generate the color table
			sb.Append("\r\n{\\colortbl ");
			//get the default font color
			Color defaultFontColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorTextboxText);
			sb.AppendFormat(@"\red{0}\green{1}\blue{2};",
				defaultFontColor.R,
				defaultFontColor.G, 
				defaultFontColor.B);
			
			Color defaultBkgColor = TVColorConverter.GetColorFromString(TrafficViewerOptions.Instance.ColorTextboxBackground);
			sb.AppendFormat(@"\red{0}\green{1}\blue{2};",
				defaultBkgColor.R,
				defaultBkgColor.G,
				defaultBkgColor.B);

			const int DEFAULT_FONT_COLOR_INDEX = 0;
			const int DEFAULT_BKG_COLOR_INDEX = 1;
			const int HIGHLIGHTS_OFFSET = 2;

			if (hLen > 0)
			{
				
				int colorIndex = HIGHLIGHTS_OFFSET;
				foreach (RtfHighlight h in highlights)
				{
					if (!colorMap.ContainsKey(h.Color))
					{
						colorMap.Add(h.Color, colorIndex);
						colorIndex++;
						sb.AppendFormat(@"\red{0}\green{1}\blue{2};", h.Color.R, h.Color.G, h.Color.B);
					}
				}
				
			}

			sb.Append("}");

			sb.Append(Environment.NewLine);
			sb.Append(RTF_HEADER2);

			//add /PAR for each newline add /TAB for each tab
			//add highlighting
			//start by adding a default text color
			sb.Append("\\cf0 ");

			int len; //used in measuring a chunk to be written to the string builder

			RtfHighlight currHlt = null;
			int nextBreak = 0;
			string formatText = String.Empty;

			for (i = 0; i < n && !_cancelRequested; i++)
			{
				int colorIndex = 0;
				//start a highlight section if necessary
				if (hIndex < hLen && highlights[hIndex].Start == i)
				{
					currHlt = highlights[hIndex];

					if (colorMap.TryGetValue(currHlt.Color, out colorIndex))
					{
						//append the text collected since the last highlight or special symbol
						len = i - lastBreakIndex;
						sb.Append(text, lastBreakIndex, len);
						lastBreakIndex = i;

						if (currHlt.Type == RtfHighlightType.Background)
						{
							formatText = "highlight";
						}
						else
						{
							formatText = "cf";
						}

						sb.AppendFormat("\\{0}{1} ", formatText, colorIndex);

						//calculate the index where the highlight stops
						nextBreak = i + currHlt.Length;

						//the highlight tag was appended continue as usual
					}
					else
					{
						currHlt = null;
					}
				}

				//check for special symbols
				short charCode = (short)text[i];
				
				if (text[i] == NL)
				{
					len = i - lastBreakIndex;
					bool cr = false;
					if (i > 0 && len > 0 && text[i - 1] == CR)
					{
						cr = true;
						//append the cr after the \par
						len--;
					}

					sb.Append(text, lastBreakIndex, len);
					if (currHlt == null) sb.Append(@"\par");
					if (cr) sb.Append("\r");
					sb.Append("\n");
					

					lastBreakIndex = i + 1;
				}
				else if (text[i] == TB)
				{
					len = i - lastBreakIndex;
					sb.Append(text, lastBreakIndex, len);
					while (i < n && text[i] == TB)
					{
						sb.Append("\\tab");
						i++;
					}
					sb.Append(' ');
					lastBreakIndex = i;
					i--;
				}
				else if (text[i] == '{' || text[i] == '}' || text[i] == '\\')
				{
					len = i - lastBreakIndex;
					sb.Append(text, lastBreakIndex, len);
					sb.Append("\\" + text[i]);
					lastBreakIndex = i + 1;
				}
				else if (charCode > 127 || charCode <= 0)
				{ 
					//any characters above the regular ASCII set should be converted to unicode
					len = i - lastBreakIndex;
					sb.Append(text, lastBreakIndex, len);
					sb.AppendFormat("\\u{0}?",charCode);
					lastBreakIndex = i + 1;
				}


				//close the highlight
				if (currHlt != null && i == nextBreak)
				{
					//append the text in between as usual
					len = i - lastBreakIndex;
					if (len > 0)
					{
						sb.Append(text, lastBreakIndex, len);
					}

					//depending on the type of highlight set the default color back
					int defColorIndex = currHlt.Type == RtfHighlightType.Foreground ? 
									DEFAULT_FONT_COLOR_INDEX : DEFAULT_BKG_COLOR_INDEX;

					sb.AppendFormat("\\{0}{1} ", formatText, defColorIndex);

					lastBreakIndex = i;
					i--;
					currHlt = null;
					hIndex++;
				}
			}

			len = i - lastBreakIndex;
			if (len > 0 && i <= n)
			{
				sb.Append(text, lastBreakIndex, len);
				if (currHlt != null)
				{
					//depending on the type of highlight set the default color back
					int defColorIndex = currHlt.Type == RtfHighlightType.Foreground ?
									DEFAULT_FONT_COLOR_INDEX : DEFAULT_BKG_COLOR_INDEX;
					//close any highlights that were eventually opened before the end
					sb.AppendFormat("\\{0}{1} ", formatText, defColorIndex);
				}
			}

			sb.Append(RTF_FOOTER);

			return sb.ToString();
		}

		/// <summary>
		/// Sorts the specified highlights array by start index
		/// </summary>
		/// <param name="highlights"></param>
		/// <returns></returns>
		private static RtfHighlight[] SortHighlights(RtfHighlight[] highlights)
		{
			if (highlights == null)
			{
				return new RtfHighlight[0];
			}
			int i, j, m = highlights.Length, n = m - 1;
			RtfHighlight temp;
			for (i = 0; i < n; i++)
			{
				for (j = i + 1; j < m; j++)
				{
					if (highlights[i].Start > highlights[j].Start)
					{
						temp = highlights[i];
						highlights[i] = highlights[j];
						highlights[j] = temp;
					}
				}
			}
			return highlights;
		}

	}

	public enum RtfHighlightType
	{
		Background,
		Foreground
	}

	/// <summary>
	/// Stores information about a highlighted portion of text
	/// </summary>
	public class RtfHighlight
	{
		private Color DEFAULT_DIFF_COL = Color.DarkMagenta;

		private int _start = -1;
		/// <summary>
		/// The index of the first highlighted character
		/// </summary>
		public int Start
		{
			get { return _start; }
			set { _start = value; }
		}

		private int _length = 0;
		/// <summary>
		/// The length of the highlight
		/// </summary>
		public int Length
		{
			get { return _length; }
			set { _length = value; }
		}

		private Color _color;
		/// <summary>
		/// The color of the highlight
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		private RtfHighlightType _type;
		/// <summary>
		/// How is the highlight being applied, font color or background
		/// </summary>
		public RtfHighlightType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public void ImportDiff(IDiffObject diffElement)
		{
			if (diffElement != null)
			{
				_start = (int)diffElement.Position;
				_length = diffElement.Length;
				_color = DEFAULT_DIFF_COL;
				_type = RtfHighlightType.Foreground;
			}
		}

		/// <summary>
		/// Creates an object storing information about a highlighted portion of text
		/// </summary>
		/// <param name="start">The index of the first highlighted character</param>
		/// <param name="length">The length of the highlight</param>
		/// <param name="color">The color of the highlight</param>
		/// <param name="type">The type of the highlight</param>
		public RtfHighlight(int start, int length, Color color, RtfHighlightType type)
		{
			_start = start;
			_length = length;
			_color = color;
			_type = type;
		}

		/// <summary>
		/// Creates an object storing information about a highlighted portion of text from
		/// a diff elements
		/// </summary>
		/// <param name="diffElement"></param>
		public RtfHighlight(IDiffObject diffElement)
		{
			ImportDiff(diffElement);
		}
	}

}
