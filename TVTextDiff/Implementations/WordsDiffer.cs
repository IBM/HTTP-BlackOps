using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff.Implementations
{
	/// <summary>
	/// Text diffs word elements then finds the different characters
	/// </summary>
	public class WordsDiffer:LettersDiffer
	{

		/// <summary>
		/// Adds a data to be compared
		/// </summary>
		/// <param name="task"></param>
		/// <param name="baseDiffPositon">The base position that will be added to the start position of all Diff Objects in this collection. This
		/// is usefull when you have individual differences inside a line and you want to report on the position inside the entire file</param>
		/// <returns>Index of the added task or negative if the task was not added</returns>
		public override int AddTask(string task, long basePosition)
		{
			WordsDiffCollection newElement = new WordsDiffCollection();
			newElement.BasePosition = basePosition;
			newElement.IgnoreWhiteSpace = _properties.IgnoreWhiteSpace;
			newElement.Import(task);
			_collections.Add(newElement);
			return _collections.Count - 1;
		}

	}
}
