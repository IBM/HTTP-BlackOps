using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Interface for objects that the diff algorithm processes
	/// </summary>
	public interface IDiffObject
	{
		/// <summary>
		/// The start position inside the comparison context
		/// </summary>
		long Position
		{
			get;
		}

		/// <summary>
		/// The length of the compare element
		/// </summary>
		int Length
		{
			get;
		}

		/// <summary>
		/// Compares the value of the current element with the value of the argument
		/// </summary>
		/// <param name="diffObject">Element to compare with</param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		CompareResult CompareValues(IDiffObject diffObject, bool ignoreCase);

		/// <summary>
		/// Checks if the value of the current element is exactly equal to the value of the
		/// argument
		/// </summary>
		/// <param name="diffObject"></param>
		/// <returns></returns>
		bool ValueEquals(IDiffObject diffObject);

		/// <summary>
		/// Checks if the value of the current element is similar to the value of the argument
		/// </summary>
		/// <param name="diffObject">Element to compare with</param>
		/// <returns>True if the values are equal</returns>
		bool ValueMatches(IDiffObject diffObject);

		/// <summary>
		/// Checks if the value of the current element is equal to the value of the argument
		/// </summary>
		/// <param name="diffObject">Element to compare with</param>
		/// <param name="diffsForSelf">The differences between the current object and the argument</param>
		/// <param name="diffsForSecond">The differences between the argument and this object</param>
		/// <param name="diffsForArgument"></param>
		/// <param name="commonElements"></param>
		/// <returns>True if the values are equal</returns>
		bool ValueMatches(IDiffObject diffObject, out IDiffObjectsCollection diffsForSelf, out IDiffObjectsCollection diffsForArgument, out IDiffObjectsCollection commonElements);

		/// <summary>
		/// Merges the two objects 
		/// </summary>
		/// <param name="objectToMergeWith"></param>
		/// <returns>True if a merge is possible</returns>
		bool Merge(IDiffObject objectToMergeWith);

		/// <summary>
		/// Makes a deep clone of the current object
		/// </summary>
		/// <returns></returns>
		IDiffObject Clone();

	}
}
