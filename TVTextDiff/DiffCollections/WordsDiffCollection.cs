using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TVDiff.DiffObjects;

namespace TVDiff
{
	/// <summary>
	/// Implements a words diff objects collection
	/// </summary>
	public class WordsDiffCollection : TextDiffCollection
	{
		private const int ESTIMATED_WORD_SIZE = 50;

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
			StringBuilder currWord = new StringBuilder(ESTIMATED_WORD_SIZE);
			long start = _basePosition;
			for (i = 0; i < n; i++)
			{
				if (Char.IsPunctuation(obj[i]) ||
					Char.IsSeparator(obj[i]) ||
					Char.IsSymbol(obj[i]))
				{
					if (currWord.Length > 0)
					{
						currWord.Append(obj[i]);
						_items.Add(new WordDiffObject(start, currWord.Length, currWord.ToString()));
						currWord = new StringBuilder(ESTIMATED_WORD_SIZE);
					}
					start = _basePosition + i + 1;
				}
				else
				{
					currWord.Append(obj[i]);
				}
			}
			
			if (currWord.Length > 0)
			{
				_items.Add(new WordDiffObject(n - currWord.Length + _basePosition, currWord.Length, currWord.ToString()));
			}
		}

		public override void InitDiffs()
		{
			_differences = new WordsDiffCollection();
		}

		public override void InitCommonElements()
		{
			_commonElements = new WordsDiffCollection();
		}
	}
}
