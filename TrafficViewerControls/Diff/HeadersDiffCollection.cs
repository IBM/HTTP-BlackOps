using System;
using System.Collections.Generic;
using System.Text;
using TVDiff;

namespace TrafficViewerControls.Diff
{
	internal class HeadersDiffCollection : LinesDiffCollection
	{

		/// <summary>
		/// Imports a string containing Http Headers to a collection of HttpDiffObjects
		/// </summary>
		/// <param name="text">Request or response text</param>
		/// <exception cref="DiffException"></exception>
		public override void Import(string text)
		{
			_items = new List<IDiffObject>();
			_differences = new HeadersDiffCollection();
			_commonElements = new HeadersDiffCollection();


			//the headers start after the first new line (following the request line)
			int nlIndex = text.IndexOf('\n') + 1;

			_basePosition += nlIndex;

			int headersEnd = text.IndexOf("\r\n\r\n");
			if (headersEnd == -1)
			{
				headersEnd = text.IndexOf("\n\n");
				if (headersEnd == -1)
				{
					headersEnd = text.Length;
				}
			}


			string headersText = String.Empty;

			if (headersEnd > nlIndex)
			{
				headersText = text.Substring(nlIndex, headersEnd - nlIndex);
			}

			int i, n = headersText.Length;
			StringBuilder currLine = new StringBuilder(ESTIMATED_LINE_SIZE);

			long pos = _basePosition;

			for (i = 0; i < n; i++)
			{
				if (headersText[i] == '\r' || headersText[i] == '\n')
				{
					string val = currLine.ToString();

					if (val != String.Empty)
					{
						AddNewItem(headersText, i, pos, val);
						currLine = new StringBuilder(ESTIMATED_LINE_SIZE);
					}
					pos = _basePosition + i + 1;

				}
				else
				{
					currLine.Append(headersText[i]);
				}
			}

			//if the string finished without a \n
			if (currLine.Length > 0)
			{
				AddNewItem(headersText, n - 2, n - currLine.Length + _basePosition, currLine.ToString());
			}
		}

		/// <summary>
		/// Adds a new header to the current collection
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <param name="pos"></param>
		/// <param name="val"></param>
		private void AddNewItem(string obj, int i, long pos, string val)
		{
			if (!IsWhiteSpace(val))
			{
				HeaderDiffObject newHeader = new HeaderDiffObject((int)pos, val);
				//check if the header appears more than once, this should change the similarity
				//algorithm, if the header only appears once then headers with the same name
				//will be considered matches

				string hName = newHeader.Name + ":";

				if (//search ahead
					obj.IndexOf("\n" + hName, i + 1, StringComparison.CurrentCultureIgnoreCase) == -1 &&
					//search behind
					obj.IndexOf(hName, 0, i - val.Length, StringComparison.CurrentCultureIgnoreCase) == -1
					)
				{
					newHeader.ValuesMinSimilarity = 0.0; //values don't matter 
				}


				_items.Add(newHeader);
			}
		}
	}
}
