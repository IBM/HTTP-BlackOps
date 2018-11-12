using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TVDiff.DiffObjects;

namespace TVDiff
{
	/// <summary>
	/// Implements a lines diff objects collection
	/// </summary>
	public class LinesDiffCollection : TextDiffCollection
	{
		protected const int ESTIMATED_LINE_SIZE = 255;

		/// <summary>
		/// Imports a string into the collection, at the same time overwrites any existing elements or
		/// diff results for this collection
		/// </summary>
		/// <param name="obj"></param>
		public override void Import(string obj)
		{
			_items = new List<IDiffObject>();
			InitDiffs();
			InitCommonElements();

			int i, n = obj.Length;
			StringBuilder currLine = new StringBuilder(ESTIMATED_LINE_SIZE);

			long pos = _basePosition;

			for (i = 0; i < n; i++)
			{
				if (obj[i] == '\r' || obj[i] == '\n')
				{
					string val = currLine.ToString();

					if (!IsWhiteSpace(val))
					{
						_items.Add(new LineDiffObject(pos, currLine.Length, val));
					}

					currLine = new StringBuilder(ESTIMATED_LINE_SIZE);

					pos = _basePosition + i + 1;
				}
				else
				{
					currLine.Append(obj[i]);
				}
			}

			//if the string finished without a \n
			if (currLine.Length > 0)
			{
				_items.Add(new LineDiffObject(n - currLine.Length + _basePosition, currLine.Length, currLine.ToString()));
			}
		}

		public override void InitDiffs()
		{
			_differences = new LinesDiffCollection();
		}

		public override void InitCommonElements()
		{
			_commonElements = new LinesDiffCollection();
		}
	}
}
