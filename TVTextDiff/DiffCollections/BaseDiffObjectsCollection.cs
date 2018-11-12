using System;
using System.Collections.Generic;
using System.Text;

namespace TVDiff
{
	/// <summary>
	/// Collection of diff objects
	/// </summary>
	public abstract class BaseDiffObjectsCollection<T> : IDiffObjectsCollection
	{

		protected long _basePosition = 0;
		/// <summary>
		/// The base position that will be added to the start position of all Diff Objects in this collection. This
		/// is usefull when you have individual differences inside a line and you want to report on the position inside the entire file
		/// </summary>
		public long BasePosition
		{
			get { return _basePosition; }
			set { _basePosition = value; }
		}

		protected bool _allowMerges = false;
		/// <summary>
		/// Whether to allow elements that are adjacent to be merged
		/// </summary>
		public bool AllowMerges
		{
			get { return _allowMerges; }
			set { _allowMerges = value; }
		}

		#region IDiffObjectsCollection Members

		public IDiffObject this[int index]
		{
			get
			{
				return Items[index];
			}
		}

		public int Count
		{
			get
			{
				return Items.Count;
			}
		}

		protected List<IDiffObject> _items = new List<IDiffObject>();
		/// <summary>
		/// Returns a collection of the items
		/// </summary>
		public List<IDiffObject> Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
			}
		}

		protected IDiffObjectsCollection _differences;
		/// <summary>
		/// Gets a collection of differences
		/// </summary>
		public IDiffObjectsCollection Differences
		{
			get
			{
				return _differences;
			}
			set
			{
				_differences = value;
			}
		}


		protected IDiffObjectsCollection _commonElements;
		/// <summary>
		/// Returns the collection of common elements resulted from a diff with a different collection
		/// </summary>
		public IDiffObjectsCollection CommonElements
		{
			get
			{
				return _commonElements;
			}
			set
			{
				_commonElements = value;
			}
		}


		public abstract void Import(T obj);

		public void Add(IDiffObject obj)
		{
			if (obj != null)
			{
				//clone the object so we don't modify objects that are being themselves diffed
				IDiffObject clone = obj.Clone();

				int n = Items.Count;
				if (!_allowMerges || n == 0 || !Items[n - 1].Merge(clone))
				{
					//if the collection is empty or if we couldn't merge the last element to the new object
					Items.Add(clone);
				}
			}
		}

		public void Push(IDiffObject obj)
		{
			if (obj != null)
			{
				//clone the object so we don't modify objects that are being themselves diffed
				IDiffObject clone = obj.Clone();

				if (_allowMerges && Items.Count > 0 && clone.Merge(Items[0]))
				{
					Items[0] = clone;
				}
				else
				{
					//there are no elements in the collection or we couldn't merge with the first
					Items.Insert(0, clone);
				}
			}

		}

		public void SortByValues(bool ignoreCase)
		{
			Items.Sort(new DiffObjectValuesComparer(ignoreCase));
		}

		public void AddRange(IDiffObjectsCollection collection)
		{
			if (collection != null)
			{
				Items.AddRange(collection.Items);
			}
		}

		public void PushRange(IDiffObjectsCollection collection)
		{
			if (collection != null)
			{
				Items.InsertRange(0, collection.Items);
			}
		}

		/// <summary>
		/// Sorts all the element by position
		/// </summary>
		public void SortByPosition()
		{
			Items.Sort(new DiffObjectPositionComparer());
		}

		public void MergeAll()
		{
			this.SortByPosition();
			int i = Items.Count - 1;

			while (i > 0)
			{
				if (Items[i - 1].Merge(Items[i]))
				{
					Items.RemoveAt(i);
				}
				i--;
			}

		}

		public abstract void InitDiffs();

		public abstract void InitCommonElements();

		#endregion
	}
}
