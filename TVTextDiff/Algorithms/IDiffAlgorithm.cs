using System;
namespace TVDiff.Algorithms
{
	/// <summary>
	/// Defines a diff algorithm to be used by a diff implementation
	/// </summary>
	public interface IDiffAlgorithm
	{
		/// <summary>
		/// Finds all differences between first and second and returns their similarity factor
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns>Similarity ratio</returns>
		double Diff(ref IDiffObjectsCollection first, ref IDiffObjectsCollection second);
	}
}
