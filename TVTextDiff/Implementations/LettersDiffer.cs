using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Algorithms;

namespace TVDiff.Implementations
{
	/// <summary>
	/// Implements your basic char differ and produces a diff collection 
	/// </summary>
	public class LettersDiffer : GenericDiffer
	{

		/// <summary>
		/// Adds a data to be compared
		/// </summary>
		/// <param name="task"></param>
		/// <param name="baseDiffPositon">The base position that will be added to the start position of all Diff Objects in this collection. This
		/// is usefull when you have individual differences inside a line and you want to report on the position inside the entire file</param>
		/// <returns>Index of the added task or negative if the task was not added</returns>
		public virtual int AddTask(string task, long basePosition)
		{
			TextDiffCollection newElement = new TextDiffCollection();
			newElement.BasePosition = basePosition;
			newElement.IgnoreWhiteSpace = _properties.IgnoreWhiteSpace;
			newElement.Import(task);
			_collections.Add(newElement);
			return _collections.Count - 1;
		}


		/// <summary>
		/// Imports first and second to and diffs them.
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns>The results of the diff operation as a set of IDiffObjectCollections</returns>
		public DiffResult DoDiff(string first, string second)
		{
			int index1 = this.AddTask(first, 0);
			int index2 = this.AddTask(second, 0);

			this.DoDiff(index1, index2);

			DiffResult result = new DiffResult();
			result.DifferencesForFirst = _collections[index1].Differences;
			result.DifferencesForSecond = _collections[index2].Differences;

			return result;
		}
	}
}
