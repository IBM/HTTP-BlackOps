using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Compares two diff objects by values
	/// </summary>
	public class DiffObjectValuesComparer : IComparer<IDiffObject>
	{
		/// <summary>
		/// If the compare should be case sensitive
		/// </summary>
		private bool _ignoreCase = false;

		public DiffObjectValuesComparer(bool ignoreCase)
		{
			_ignoreCase = ignoreCase;
		}


		#region IComparer<IDiffObject> Members

		public int Compare(IDiffObject x, IDiffObject y)
		{
			return (int)x.CompareValues(y, _ignoreCase);
		}

		#endregion
	}

	/// <summary>
	/// Compares two diff objects by position
	/// </summary>
	public class DiffObjectPositionComparer : IComparer<IDiffObject>
	{

		#region IComparer<IDiffObject> Members

		public int Compare(IDiffObject x, IDiffObject y)
		{
			int result = 0;

			if(x.Position < y.Position)
			{
				result = -1;
			}
			else if (x.Position > y.Position)
			{
				result = 1;
			}

			return result;
		}

		#endregion
	}
}
