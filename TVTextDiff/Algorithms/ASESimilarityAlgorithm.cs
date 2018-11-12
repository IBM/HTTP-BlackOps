using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff.Algorithms
{
	/// <summary>
	/// Calculates the similarity factor between two strings
	/// </summary>
	public static class ASESimilarityAlgorithm
	{
		/// <summary>
		/// Calculates the similarity factor between the two strings
		/// </summary>
		/// <param name="text1"></param>
		/// <param name="text2"></param>
		/// <returns></returns>
		public static double CalculateSimilarity(string text1, string text2)
		{

			if (text1.Length < 2 && text2.Length < 2)
			{
				if (text1 == text2)
				{
					return 1.0;
				}
				else
				{
					return 0.0;
				}
			}

			// This algorithm is the winner of the "Body Similar Contest".
			// We need to keep track of how many times each character appears.
			const int NUM_CHARS = 256;
			const int LAST_CHAR_INDEX = NUM_CHARS - 1;

			int[] counts1 = new int[NUM_CHARS];
			int[] counts2 = new int[NUM_CHARS];

			for (int i = 0; i < NUM_CHARS; i++)
			{
				counts1[i] = 0;
				counts2[i] = 0;
			}

			int length1 = text1.Length;
			int length2 = text2.Length;

			// count occurrences of each character in each string
			for (int i = 0; i < length1; i++)
			{
				counts1[text1[i] & LAST_CHAR_INDEX]++;
			}
			for (int i = 0; i < length2; i++)
			{
				counts2[text2[i] & LAST_CHAR_INDEX]++;
			}

			int numZeros = 0;
			double min = 0.0;
			double max = 0.0;
			double total = 0.0;
			for (int i = 0; i < NUM_CHARS; i++)
			{
				min = (double)Math.Min(counts1[i], counts2[i]);
				max = (double)Math.Max(counts1[i], counts2[i]);

				if (max == 0.0) //if max is zero then min is zero too
				{
					numZeros++;
				}
				else
				{
					total += (min / max);
				}
			}

			double similarity = 1.0;

			if (numZeros < NUM_CHARS)
			{
				similarity = (total / (NUM_CHARS - numZeros));
			}

			return similarity;

		}

	}
}
