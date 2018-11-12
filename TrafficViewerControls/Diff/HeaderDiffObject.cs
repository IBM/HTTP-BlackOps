using System;
using System.Collections.Generic;
using System.Text;
using TVDiff;
using TrafficViewerSDK;
using TVDiff.Implementations;

namespace TrafficViewerControls.Diff
{
	internal class HeaderDiffObject : IDiffObject
	{

		private int _position = 0;
		private int _length = 0;
		private string _headerLine = String.Empty;

		private double _valuesMinSimilarity = 0.6;
		/// <summary>
		/// Gets/sets the similarity factor used when matching headers values.
		/// Specifies how similar the values of a header have to be for the header
		/// to be considered the same with the header is being matched against
		/// </summary>
		public double ValuesMinSimilarity
		{
			get { return _valuesMinSimilarity; }
			set { _valuesMinSimilarity = value; }
		}

		private int _headerLineHash;
		/// <summary>
		/// Returns a unique hash for the header line
		/// </summary>
		public int HeaderLineHash
		{
			get { return _headerLineHash; }
		}

		private string _name = String.Empty;
		/// <summary>
		/// The header name
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		WordsDiffCollection _values = new WordsDiffCollection();
		//gets the header values
		public WordsDiffCollection Values
		{
			get { return _values; }
		}

		public override string ToString()
		{
			return _headerLine;
		}

		/// <summary>
		/// Empty ctor
		/// </summary>
		public HeaderDiffObject() { }

		/// <summary>
		/// Transforms a string into a header diff object
		/// </summary>
		/// <param name="position">The position of the header line in the raw request</param>
		/// <param name="headerLine">The header line</param>
		public HeaderDiffObject(int position, string headerLine)
		{
			_position = position;
			_length = headerLine.Length;
			_headerLine = headerLine;
			_headerLineHash = _headerLine.GetHashCode();

			//extract the name, find the first :
			int firstColonPos = headerLine.IndexOf(':');
			if (firstColonPos > -1)
			{
				_name = headerLine.Substring(0, firstColonPos);
				_values.BasePosition = _position + firstColonPos + 1;
				_values.Import(headerLine.Substring(firstColonPos + 1));
			}
			else
			{
				throw new DiffException("Malformed header");
			}
		}

		#region IDiffObject Members

		/// <summary>
		/// Gets the position of the header in the request or response
		/// </summary>
		public long Position
		{
			get { return _position; }
		}

		/// <summary>
		/// Gets the length of the header
		/// </summary>
		public int Length
		{
			get { return _length; }
		}

		/// <summary>
		/// Compares the values 
		/// </summary>
		/// <param name="diffObject"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public CompareResult CompareValues(IDiffObject diffObject, bool ignoreCase)
		{
			HeaderDiffObject other = diffObject as HeaderDiffObject;
			CompareResult res = CompareResult.Greater;
			if (other != null)
			{
				string otherString = other.ToString();
				res = (CompareResult)String.Compare(_headerLine, otherString);
			}
			return res;
		}

		/// <summary>
		/// Verifies if the headers are exactly equal
		/// </summary>
		/// <param name="diffObject"></param>
		/// <returns></returns>
		public bool ValueEquals(IDiffObject diffObject)
		{
			bool res = false;
			HeaderDiffObject otherObject = diffObject as HeaderDiffObject;
			if (otherObject != null)
			{
				res = _headerLineHash == otherObject.HeaderLineHash;
			}
			return res;
		}

		/// <summary>
		/// Checks if the values match, are similar
		/// </summary>
		/// <param name="diffObject"></param>
		/// <returns></returns>
		public bool ValueMatches(IDiffObject diffObject)
		{
			IDiffObjectsCollection diffsForSelf;
			IDiffObjectsCollection diffsForArg;
			IDiffObjectsCollection common;
			return ValueMatches(diffObject, out diffsForSelf, out diffsForArg, out common);
		}

		/// <summary>
		/// Check if the values match and outputs a set of granular differences
		/// </summary>
		/// <param name="diffObject"></param>
		/// <param name="diffsForSelf"></param>
		/// <param name="diffsForArgument"></param>
		/// <returns></returns>
		public bool ValueMatches(IDiffObject diffObject, 
			out IDiffObjectsCollection diffsForSelf, 
			out IDiffObjectsCollection diffsForArgument,
			out IDiffObjectsCollection commonElements)
		{
			HeaderDiffObject otherObject = diffObject as HeaderDiffObject;
			diffsForSelf = null;
			diffsForArgument = null;
			commonElements = null;

			bool matches = false;
			if (otherObject != null)
			{
				matches = _headerLineHash == otherObject.HeaderLineHash;
				//if the values are different but the headers are the same
				if (!matches && String.Compare(_name, otherObject.Name, true) == 0)
				{
					WordsDiffer valuesDiffer = new WordsDiffer();
					valuesDiffer.AddTask(_values);
					valuesDiffer.AddTask(otherObject.Values);
					valuesDiffer.Properties.Sorted = true;
					valuesDiffer.Properties.CaseInSensitiveSort = true;
					double diffRatio = valuesDiffer.DoDiff(0, 1);

					diffsForSelf = valuesDiffer.GetResultingDifferences(0);
					diffsForArgument = valuesDiffer.GetResultingDifferences(1);
					commonElements = valuesDiffer.GetResultingCommonElements(0);
					matches = diffRatio >= _valuesMinSimilarity;
				}
			}
			return matches;
		}

		/// <summary>
		/// Merge of headers is not supported this will always return false
		/// </summary>
		/// <param name="objectToMergeWith"></param>
		/// <returns></returns>
		public bool Merge(IDiffObject objectToMergeWith)
		{
			return false;
		}

		/// <summary>
		/// Returns a deep clone of the current element
		/// </summary>
		/// <returns></returns>
		public IDiffObject Clone()
		{
			HeaderDiffObject clone = new HeaderDiffObject(_position, _headerLine);
			return clone;
		}

		#endregion
	}
}
