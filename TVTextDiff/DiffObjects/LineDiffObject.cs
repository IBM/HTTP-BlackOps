using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Implementations;

namespace TVDiff.DiffObjects
{
	public class LineDiffObject : BaseTextDiffObject
	{
		/// <summary>
		/// Used with similarity diff algorithms
		/// </summary>
		protected override double SimilarityFactor
		{
			get { return 0.6; }
		} 


		public override IDiffObject Clone()
		{
			LineDiffObject clone = new LineDiffObject(_position,_length,_value);
			return clone;
		}

		/// <summary>
		/// Creates differ used for granular comparisons in ValueEquals(used with similarity algorithms)
		/// </summary>
		protected override LettersDiffer InitGranularDiffer()
		{
			return new WordsDiffer();
		}

		public LineDiffObject(long start, int length, string value) : base (start, length, value) { }
	}
}
