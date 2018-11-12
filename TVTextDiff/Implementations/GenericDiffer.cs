using System;
using System.Collections.Generic;
using System.Text;
using TVDiff.Algorithms;

namespace TVDiff.Implementations
{
	/// <summary>
	/// This is an abstract class for diff implementations
	/// </summary>
	/// <typeparam name="T">Type of the elements that need to be diffed.</typeparam>
	public class GenericDiffer
	{
		protected double DoDiff(IDiffObjectsCollection first, IDiffObjectsCollection second)
		{
			first.InitDiffs();
			second.InitDiffs();
			first.InitCommonElements();
			second.InitCommonElements();

			if (_properties.Sorted)
			{
				first.SortByValues(_properties.CaseInSensitiveSort);
				second.SortByValues(_properties.CaseInSensitiveSort);
				//replace the default LCS algorithm with a simple algorithm for sorted collections
				_algorithm = new SortedCollectionsDiffAlgorithm();
			}

			first.AllowMerges = _properties.AllowMerges;
			second.AllowMerges = _properties.AllowMerges;

			return _algorithm.Diff(ref first, ref second);
		}
		
		protected IDiffAlgorithm _algorithm = new LCSSimilarityDiffAlgorithm();
		/// <summary>
		/// Contains the Diff logic being used. Any implementation can override this logic
		/// </summary>
		public IDiffAlgorithm Algorithm
		{
			get { return _algorithm; }
			set { _algorithm = value; }
		}


		/// <summary>
		/// Collections of elements to be compared
		/// </summary>
		protected List<IDiffObjectsCollection> _collections = new List<IDiffObjectsCollection>();

		protected DiffProperties _properties = new DiffProperties();
		/// <summary>
		/// Gets, sets the properties for the diff operation
		/// </summary>
		public DiffProperties Properties
		{
			get { return _properties; }
			set { _properties = value; }
		}

		/// <summary>
		/// Adds a task to be compared
		/// </summary>
		/// <param name="task"></param>
		/// <returns>Index of the added task</returns>
		public int AddTask(IDiffObjectsCollection task)
		{
			_collections.Add(task);
			return _collections.Count - 1;
		}

		/// <summary>
		/// Removes a task 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveTask(int index)
		{
			_collections.RemoveAt(index);
		}

		/// <summary>
		/// Clears all existing diff tasks
		/// </summary>
		public void ClearTasks()
		{
			_collections.Clear();
		}

		/// <summary>
		/// Executes the diff operation on the first and second objects that were loaded
		/// </summary>
		/// <exception cref="DiffException">Insufficient collections loaded</exception>
		public double DoDiff()
		{
			return DoDiff(0, 1);
		}

		/// <summary>
		/// Executes the diff operation between the specified indexes of the collections list
		/// </summary>
		/// <param name="indexOfFirst">The index of the first element to diff</param>
		/// <param name="indexOfSecond">The index of the second element to diff</param>
		public double DoDiff(int indexOfFirst, int indexOfSecond)
		{
			IDiffObjectsCollection first = _collections[indexOfFirst];
			IDiffObjectsCollection second = _collections[indexOfSecond];

			return DoDiff(first, second);
		}



		/// <summary>
		/// Finds all the common elements in all the loaded collections and returns a list. Might want to override if the
		/// diff produces diff elements of a different type
		/// </summary>
		/// <returns>The collection of common elements</returns>
		public virtual IDiffObjectsCollection FindCommonElements()
		{
			int n = _collections.Count;

			IDiffObjectsCollection commonElements = _collections[0];

			for (int i = 1; i < n; i++)
			{
				//disable merges for this diff

				bool allowMerges = _properties.AllowMerges;
				_properties.AllowMerges = false;

				DoDiff(commonElements,_collections[i]);

				_properties.AllowMerges = allowMerges;

				//replace the current collection with the common elements collection
				//at the next iteration the next element in the collection will be compared against the common elements
				commonElements = _collections[i].CommonElements;

				if (commonElements.Count == 0)
				{
					return commonElements; //return an empty collection, there are no common elements
				}

				//sort the elements by position
				commonElements.SortByPosition();

			}

			return commonElements; //the last element will hold all the common elements 
		}

		/// <summary>
		/// Gets the differences between the specified index element and the element it was diffed against
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Collection of diff objects, which contain that start and length for highlighting purposes</returns>
		public IDiffObjectsCollection GetResultingDifferences(int index)
		{
			if (index >= 0 && index < _collections.Count)
			{
				return _collections[index].Differences;
			}
			return null;
		}

		/// <summary>
		/// Gets the common elements between the specified index element and the element it was diffed against
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Collection of diff objects</returns>
		public IDiffObjectsCollection GetResultingCommonElements(int index)
		{
			if (index > 0 && index < _collections.Count)
			{
				return _collections[index].CommonElements;
			}
			return null;
		}

	}
}
