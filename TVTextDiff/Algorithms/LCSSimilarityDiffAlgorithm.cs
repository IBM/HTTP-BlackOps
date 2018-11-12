using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.DiffObjects;

namespace TVDiff.Algorithms
{
	/// <summary>
	/// Encapsulates the diff algorithm
	/// </summary>
	public class LCSSimilarityDiffAlgorithm : IDiffAlgorithm
	{
		private const int MAX_MATRIX_LENGTH = 10000;

		/// <summary>
		/// Diffs the two collections using the ValueEquals function of their elements and stores differences in the
		/// arguments collection
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		public double Diff(ref IDiffObjectsCollection first, ref IDiffObjectsCollection second)
		{
			double similarity = 0.0;
			if ((first == null || first.Count == 0) && second != null)
			{
				second.Differences = second;
			}
			else if ((second == null || second.Count == 0) && first != null)
			{
				first.Differences = first;
			}
			else
			{
				//use the LCS algorith to compute the longest common subsequence and populate the Differences in first and second
				similarity = LCS(ref first, ref second);
			}
			return similarity;
		}

		#region Debugging Utils

		private string PrintMatrix(ushort[, ] C, int m, int n)
		{
			int i, j;
			StringBuilder result = new StringBuilder();

			for (i = 0; i < m; i++)
			{
				for (j = 0; j < n; j++)
				{
					result.AppendFormat("{0,5}", C[i, j]);
				}
				result.Append(Environment.NewLine);
			}

			return result.ToString();
		}

		private string PrintTextDifferences(IDiffObjectsCollection diff)
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < diff.Count; i++)
			{
				result.Append((diff[i] as BaseTextDiffObject).Value);
				result.Append(" ");
			}

			return result.ToString();
		}

		#endregion

		/// <summary>
		/// Computes the longest common subsequence matrix between the two collections
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		private double LCS(ref IDiffObjectsCollection first, ref IDiffObjectsCollection second)
		{
			int start = 0;
			int firstEnd = first.Count;
			int secondEnd = second.Count;
			int i, j, n, m;

			IDiffObjectsCollection granularDiffsForFirst = null;
			IDiffObjectsCollection granularDiffsForSecond = null;
			IDiffObjectsCollection granularCommonElements = null; 

			IDiffObjectsCollection currCommonElements = first.CommonElements;
			second.CommonElements = currCommonElements;

			//trim the matching items at the beggining
			while (start < firstEnd && start < secondEnd && first[start].ValueMatches(second[start], out granularDiffsForFirst, out granularDiffsForSecond, out granularCommonElements))
			{
				
				//if the collections were slightly different add the small differences
				first.Differences.AddRange(granularDiffsForFirst);
				second.Differences.AddRange(granularDiffsForSecond);
				if (granularDiffsForFirst == null && granularDiffsForSecond == null)
				{
					currCommonElements.Add(first[start]);
				}
				else
				{
					currCommonElements.AddRange(granularCommonElements);
				}

				start++;
			}

			//trim the matching items at the end
			while (firstEnd > start && secondEnd > start && first[firstEnd - 1].ValueMatches(second[secondEnd - 1], out granularDiffsForFirst, out granularDiffsForSecond, out granularCommonElements))
			{
				firstEnd--;
				secondEnd--;

				if (granularDiffsForFirst == null && granularDiffsForSecond == null)
				{
					currCommonElements.Add(first[firstEnd]);
				}
				else
				{
					currCommonElements.AddRange(granularCommonElements);
				}
				//if the collections were slightly different insert the small differences
				first.Differences.PushRange(granularDiffsForFirst);
				second.Differences.PushRange(granularDiffsForSecond);
								
			}

			m = firstEnd - start + 1;
			n = secondEnd - start + 1;

			//truncate the matrix to prevent OOM
			m = Math.Min(m, MAX_MATRIX_LENGTH);
			n = Math.Min(n, MAX_MATRIX_LENGTH);
			//create the matrix for LCS

			ushort[,] C = new ushort[m, n];


			for (i = 1; i < m; i++)
			{
				for (j = 1; j < n; j++)
				{
					if (first[start + i - 1].ValueMatches(second[start + j - 1]))
					{
						C[i, j] = (ushort)(C[i - 1, j - 1] + 1);
					}
					else
					{
						C[i, j] = Math.Max(C[i, j - 1], C[i - 1, j]);
					}
				}
			}

			//populate the differences
			return ExtractDiffs(C, first, second, start, m, n);
		}

		private double ExtractDiffs(
			ushort[, ] C, IDiffObjectsCollection first, IDiffObjectsCollection second, int start, int m, int n)
		{

			int i = m - 1, j = n - 1;

			double diffCount1 = 0;
			double diffCount2 = 0;

			IDiffObjectsCollection currFirstDiffs = first.Differences;
			IDiffObjectsCollection currSecondDiffs = second.Differences;
			IDiffObjectsCollection currCommonElements = first.CommonElements;
			second.CommonElements = currCommonElements;

			do
			{
				int iIndex = start + i - 1;
				int jIndex = start + j - 1;
				IDiffObject currFirst = null;
				IDiffObject currSecond = null;


				IDiffObjectsCollection granularDiffsForFirst = null;
				IDiffObjectsCollection granularDiffsForSecond = null;
				IDiffObjectsCollection granularCommonElements = null;


				if (iIndex >= 0 && iIndex < first.Count)
				{
					currFirst = first[iIndex];
				}

				if (jIndex >= 0 && jIndex < second.Count)
				{
					currSecond = second[jIndex];
				}


				if (currFirst != null && 
					currFirst.ValueMatches(currSecond, out granularDiffsForFirst, out granularDiffsForSecond, out granularCommonElements))
				{
					//ValuesMatch can return true if the objects are very similar
					//In that case we need to capture granular differences
					currFirstDiffs.PushRange(granularDiffsForFirst);
					currSecondDiffs.PushRange(granularDiffsForSecond);


					if (granularDiffsForFirst == null && granularDiffsForSecond == null)
					{
						currCommonElements.Add(currFirst);
					}
					else
					{
						currCommonElements.AddRange(granularCommonElements);
					}

					i--; j--;
				}
				else if (j > 0 && (i <= 0 || C[i, j - 1] >= C[i - 1, j]))
				{
					currSecondDiffs.Push(currSecond);
					diffCount2++;
					j--;
				}
				else if (i > 0 && (j <= 0 || C[i, j - 1] < C[i - 1, j]))
				{
					currFirstDiffs.Push(currFirst);
					diffCount1++;
					i--;
				}

			}
			while (i > 0 || j > 0);

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

	}
}
