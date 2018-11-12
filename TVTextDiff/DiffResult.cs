using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Contains all relevant information resulting from a diff operation
	/// </summary>
	public class DiffResult
	{
		IDiffObjectsCollection _differencesForFirst = null;
		/// <summary>
		/// Returns the differences that the first element had compared to the second
		/// </summary>
		public IDiffObjectsCollection DifferencesForFirst
		{
			get { return _differencesForFirst; }
			set { _differencesForFirst = value; }
		}

		IDiffObjectsCollection _differencesForSecond = null;
		/// <summary>
		/// Gets/sets differences between second and first
		/// </summary>
		public IDiffObjectsCollection DifferencesForSecond
		{
			get { return _differencesForSecond; }
			set { _differencesForSecond = value; }
		}

	}
}
