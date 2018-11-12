using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Algorithms;
using TVDiff.Implementations;

namespace TVDiff
{
	/// <summary>
	/// Implements a diff object for text
	/// </summary>
	public abstract class BaseTextDiffObject : IDiffObject
	{

		/// <summary>
		/// Value used to decide if two objects of this type are similar
		/// </summary>
		protected abstract double SimilarityFactor
		{
			get;
		}

		protected abstract LettersDiffer InitGranularDiffer();

		protected string _value = String.Empty;
		/// <summary>
		/// Gets the value of the text
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_length = _value.Length;
				_valueHash = _value.GetHashCode();
			}
		}

		protected int _valueHash;
		/// <summary>
		/// Gets an unique int for the containing text, mainly used in comparisons
		/// </summary>
		public int ValueHash
		{
			get { return _valueHash; }
		}

		/// <summary>
		/// Checks if the element is exactly equal. This is different than the compare function
		/// which checks if the value of the element is equal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return this.GetHashCode() == obj.GetHashCode();
		}

		/// <summary>
		/// Gets the hash for this object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (int)_position ^ _length ^ _value.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0,6}", _position);
			sb.AppendFormat("{0,6}", _length);
			sb.Append("     ");
			sb.Append(_value);
			return sb.ToString();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="value"></param>
		public BaseTextDiffObject(long start, int length, string value)
		{
			_position = start;
			_length = length;
			_value = value;
			if (value != null)
			{
				_valueHash = _value.GetHashCode();
			}
		}

		#region IDiffObject Members

		protected long _position = -1;
		/// <summary>
		/// The start position of this element in the text to be diffed
		/// </summary>
		public long Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		protected int _length = -1;
		/// <summary>
		/// The length of the text
		/// </summary>
		public int Length
		{
			get
			{
				return _length;
			}
		}

		/// <summary>
		/// Compares the current element against the given argument
		/// </summary>
		/// <param name="diffObject"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public CompareResult CompareValues(IDiffObject diffObject, bool ignoreCase)
		{
			BaseTextDiffObject otherObject = diffObject as BaseTextDiffObject;
			CompareResult result;
			if (diffObject != null)
			{
				result = (CompareResult)String.Compare(_value, otherObject.Value, ignoreCase);
			}
			else
			{
				throw new ArgumentException("Invalid comparison argument. Has to be of type TextDiffObject or not null");
			}

			return result;
		}


		public bool Merge(IDiffObject objectToMergeWith)
		{
			bool success = false;

			BaseTextDiffObject otherObject = objectToMergeWith as BaseTextDiffObject;

			if (otherObject != null)
			{
				if (_position + _length == objectToMergeWith.Position)
				{
					_value += otherObject.Value;
					_length = _value.Length;
					success = true;
				}
			}

			return success;
		}

		/// <summary>
		/// Checks is the value is equal. If not where applicable a collection of granular differences is outputed
		/// </summary>
		/// <param name="diffObject"></param>
		/// <param name="diffsForSelf"></param>
		/// <param name="diffsForArgument"></param>
		/// <param name="commonElements"></param>
		/// <returns></returns>
		public bool ValueMatches(IDiffObject diffObject, out IDiffObjectsCollection diffsForSelf, out IDiffObjectsCollection diffsForArgument, out IDiffObjectsCollection commonElements)
		{
			BaseTextDiffObject otherObject = diffObject as BaseTextDiffObject;
			diffsForSelf = null;
			diffsForArgument = null;
			commonElements = null;
			bool matches = false;

			if (otherObject != null)
			{
				matches = this.ValueHash == otherObject.ValueHash;

				if (!matches)
				{
					double simFactor = ASESimilarityAlgorithm.CalculateSimilarity(this.Value, otherObject.Value);

					LettersDiffer granularDiffer = InitGranularDiffer();

					if (simFactor > 0 && granularDiffer != null)
					{
						//calculate granular differences
						granularDiffer.AddTask(Value, Position); //imports the current word into a letters collection starting from the position of the current element
						granularDiffer.AddTask(otherObject.Value, otherObject.Position);

						double diffRatio = granularDiffer.DoDiff(0, 1);

						diffsForSelf = granularDiffer.GetResultingDifferences(0);
						diffsForArgument = granularDiffer.GetResultingDifferences(1);
						commonElements = granularDiffer.GetResultingCommonElements(0);

						matches = diffRatio >= this.SimilarityFactor;
					}
				}
			}

			return matches;
		}

		public bool ValueMatches(IDiffObject diffObject)
		{
			BaseTextDiffObject otherObject = diffObject as BaseTextDiffObject;
			bool equals = false;
			if (otherObject != null)
			{
				equals = this.ValueHash == otherObject.ValueHash;

				if (!equals && SimilarityFactor < 1)
				{
					equals = ASESimilarityAlgorithm.CalculateSimilarity(this.Value, otherObject.Value) >= this.SimilarityFactor;
				}
			}
			return equals;
		}

		public abstract IDiffObject Clone();

		public bool ValueEquals(IDiffObject diffObject)
		{
			bool res = false;
			BaseTextDiffObject other = diffObject as BaseTextDiffObject;
			if (other != null)
			{
				res = _valueHash == other.ValueHash;
			}
			return res;
		}

		#endregion
	}
}
