using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Encapsulate properties used for the diff operation
	/// </summary>
	public class DiffProperties
	{
		private bool _sorted = false;
		/// <summary>
		/// Wether before the diff operation the diff collection will be sorted
		/// </summary>
		public bool Sorted
		{
			get { return _sorted; }
			set { _sorted = value; }
		}

		private bool _caseInSensitiveSort = false;
		/// <summary>
		/// If sorted is true and collections are sorted before the diff this flag specifies if they should be 
		/// sorted case insensitive
		/// </summary>
		public bool CaseInSensitiveSort
		{
			get { return _caseInSensitiveSort; }
			set { _caseInSensitiveSort = value; }
		}

		private bool _ignoreWhiteSpace = true;
		/// <summary>
		/// If elements with whitespace values should be ignored
		/// </summary>
		public bool IgnoreWhiteSpace
		{
			get { return _ignoreWhiteSpace; }
			set { _ignoreWhiteSpace = value; }
		}

		private bool _allowMerges = true;
		/// <summary>
		/// Whether to allow elements that are adjacent to be merged
		/// </summary>
		public bool AllowMerges
		{
			get { return _allowMerges; }
			set { _allowMerges = value; }
		}
	}
}
