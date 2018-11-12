using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Implementations;

namespace TVDiff.DiffObjects
{
	/// <summary>
	/// A text diff object made of one or more letters
	/// </summary>
	public class LetterDiffObject : BaseTextDiffObject
	{

		protected override double SimilarityFactor
		{
			get { return 1; }
		}

		public override IDiffObject Clone()
		{
			LetterDiffObject clone = new LetterDiffObject(_position, _length, _value);
			return clone;
		}

		/// <summary>
		/// Differ used for granular comparisons in ValueEquals, not necessary in LetterDiffObject, usually returns null
		/// </summary>
		protected override LettersDiffer InitGranularDiffer()
		{
			return null;
		}

		public LetterDiffObject(long start, int length, string value) : base(start, length, value) { }
	}
}
