using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Interface for a collection of diff objects
	/// </summary>
	public interface IDiffObjectsCollection
	{
		/// <summary>
		/// Gets an object in the collection
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IDiffObject this[int index]
		{
			get;
		}

		/// <summary>
		/// Gets the list of diff objects in the collection
		/// </summary>
		List<IDiffObject> Items
		{
			get;
			set;
		}

		/// <summary>
		/// Wether to allow adjacent elements to be merged
		/// </summary>
		bool AllowMerges
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the total number of elements in the collection
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Holds the differences resulted from a diff with another collection
		/// </summary>
		IDiffObjectsCollection Differences
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a collection of the elements that are not differences
		/// </summary>
		/// <returns></returns>
		IDiffObjectsCollection CommonElements
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes the diff collection with an empty list
		/// </summary>
		void InitDiffs();

		/// <summary>
		/// Initializes the common elements collection with an empty list
		/// </summary>
		void InitCommonElements();

		/// <summary>
		/// Adds an diff object to the collection
		/// </summary>
		/// <param name="obj"></param>
		void Add(IDiffObject obj);

		/// <summary>
		/// Adds a range of IDiffObjects
		/// </summary>
		/// <param name="collection"></param>
		void AddRange(IDiffObjectsCollection collection);

		/// <summary>
		/// Inserts a diff object in front of the collection
		/// </summary>
		/// <param name="obj"></param>
		void Push(IDiffObject obj);

		/// <summary>
		/// Inserts a diff collections in front
		/// </summary>
		/// <param name="collection"></param>
		void PushRange(IDiffObjectsCollection collection);

		/// <summary>
		/// Sorts the objects of the collection by position
		/// <param name="ignoreCase"></param>
		/// </summary>
		void SortByPosition();

		/// <summary>
		/// Sorts the objects of the collection by values
		/// <param name="ignoreCase"></param>
		/// </summary>
		void SortByValues(bool ignoreCase);

		/// <summary>
		/// Sorts by position and attempts to merge all the objects in the collection
		/// </summary>
		void MergeAll();


	}

}
