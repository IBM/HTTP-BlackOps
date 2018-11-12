using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Implementations;

namespace TVDiff.DiffObjects
{
	/// <summary>
	/// A text diff object made of one or more words
	/// </summary>
	public class WordDiffObject : BaseTextDiffObject
	{
		/// <summary>
		/// Used with similarity algorithms.
		/// </summary>
		protected override double SimilarityFactor
		{
			get { return 0.8; }
		}

		public override IDiffObject Clone()
		{
			WordDiffObject clone = new WordDiffObject(_position,_length,_value);
			return clone;
		}

		/// <summary>
		/// Creates differ used for granular comparisons in ValueEquals
		/// </summary>
		protected override LettersDiffer InitGranularDiffer()
		{
			return new LettersDiffer();
		}

		public WordDiffObject(long start, int length, string value) : base(start, length, value) { }
	}
}
