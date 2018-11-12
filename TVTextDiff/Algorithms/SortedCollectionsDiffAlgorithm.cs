using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff.Algorithms
{
	/// <summary>
	/// Performs a diff optimized for sorted collections. Will not provide reliable results on unsorted collections
	/// </summary>
	public class SortedCollectionsDiffAlgorithm : IDiffAlgorithm
	{
		private bool _ignoreCase = false;
		/// <summary>
		/// Gets/sets if the comparison algorithm should ignore the case
		/// </summary>
		public bool IgnoreCase
		{
			get { return _ignoreCase; }
			set { _ignoreCase = value; }
		}

		#region IDiffAlgorithm Members

		public double Diff(ref IDiffObjectsCollection first, ref IDiffObjectsCollection second)
		{
			int i = 0, j = 0, m = first.Count, n = second.Count;

			double diffCount1 = 0;
			double diffCount2 = 0;

			IDiffObjectsCollection granularDiffs1 = null;
			IDiffObjectsCollection granularDiffs2 = null;
			IDiffObjectsCollection granularCommon = null;

			IDiffObjectsCollection currCommonElements = first.CommonElements;
			second.CommonElements = currCommonElements;

			while (i < m || j < n)
			{
				if (j < n && i < m && first[i].ValueMatches(second[j], out granularDiffs1, out granularDiffs2, out granularCommon))
				{
					first.Differences.AddRange(granularDiffs1);
					second.Differences.AddRange(granularDiffs2);
					if (granularDiffs1 == null && granularDiffs2 == null)
					{
						currCommonElements.Add(first[i]);
					}
					else
					{
						currCommonElements.AddRange(granularCommon);
					}
					i++;
					j++;
				}
				else if (i < m && (j >= n || first[i].CompareValues(second[j], _ignoreCase) == CompareResult.Lower))
				{
					first.Differences.Add(first[i]);
					diffCount1++;
					i++;
				}
				else if (j < n && (i >= m || first[i].CompareValues(second[j], _ignoreCase) == CompareResult.Greater))
				{
					second.Differences.Add(second[j]);
					diffCount2++;
					j++;
				}
			}

			//calculate diff ratio, or similarity factor
			double ratio1 = 0;

			if (first.Count > 0)
			{
				ratio1 = 1 - diffCount1 / first.Count;
			}

			double ratio2 = 0;

			if (second.Count > 0)
			{
				ratio2 = 1 - diffCount2 / second.Count;
			}

			return Math.Min(ratio1, ratio2);
		}

		#endregion
	}
}
