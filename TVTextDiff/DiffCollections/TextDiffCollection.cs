using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.DiffObjects;

namespace TVDiff
{
	/// <summary>
	/// Implements a chars diff objects collection
	/// </summary>
	public class TextDiffCollection : BaseDiffObjectsCollection<string>
	{

		protected bool _ignoreWhiteSpace = false;
		/// <summary>
		/// If elements with whitespace values should be ignored
		/// </summary>
		public bool IgnoreWhiteSpace
		{
			get { return _ignoreWhiteSpace; }
			set { _ignoreWhiteSpace = value; }
		}

		/// <summary>
		/// Checks if the specified value is a white space
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		protected bool IsWhiteSpace(string val)
		{
			foreach (char c in val)
			{
				if (!Char.IsWhiteSpace(c))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets a string representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (BaseTextDiffObject obj in _items)
			{
				sb.Append(obj.Value);
				sb.Append(" ");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Calculates the length of all the values in the collection. Used to calculate similarity
		/// </summary>
		/// <returns></returns>
		public int TotalLength()
		{
			int total = 0;
			foreach (BaseTextDiffObject item in _items)
			{
				total += item.Value.Length;
			}
			return total;
		}

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

			for (i = 0; i < n; i++)
			{
				if (!_ignoreWhiteSpace || !Char.IsWhiteSpace(obj[i]))
				{
					_items.Add(new LetterDiffObject(_basePosition + i, 1, obj[i].ToString()));
				}
			}

		}

		public override void InitDiffs()
		{
			_differences = new TextDiffCollection();
		}

		public override void InitCommonElements()
		{
			_commonElements = new TextDiffCollection();
		}
	}


}
