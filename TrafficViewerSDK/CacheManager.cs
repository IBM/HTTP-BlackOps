using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Objects that implement this interface can be used with cache managers
	/// </summary>
	public interface ICacheable
	{
		/// <summary>
		/// Returns the value stored by this object but at the same time locks other threads from getting the value
		/// </summary>
		/// <returns></returns>
		ICloneable Reserve();
		/// <summary>
		/// Releases the lock enabled by Reserve
		/// </summary>
		void Release();
		/// <summary>
		/// Returns a copy of the value stored by the object without locking other threads
		/// </summary>
		/// <returns></returns>
		ICloneable GetClone();
	
	}

	/// <summary>
	/// Implements ICacheable
	/// </summary>
	public class CacheEntry : ICacheable
	{
		/// <summary>
		/// The value of the cache entry
		/// </summary>
		protected ICloneable _value;
		/// <summary>
		/// Controls access to the cache entry
		/// </summary>
		protected Mutex _control = new Mutex();

		/// <summary>
		/// Constructor for a cache entry
		/// </summary>
		/// <param name="value"></param>
		public CacheEntry(ICloneable value)
		{
			_value = value;
		}

		#region ICacheable Members

		/// <summary>
		/// Use reserve to access a cache entry one at a time
		/// </summary>
		/// <returns></returns>
		public ICloneable Reserve()
		{
			_control.WaitOne();
			return _value;
		}

		/// <summary>
		/// Use release to allow the next thread access to the cache entry
		/// </summary>
		public void Release()
		{
			_control.ReleaseMutex();
		}

		/// <summary>
		/// Use get clone to make a clone of the cache entry
		/// </summary>
		/// <returns></returns>
		public ICloneable GetClone()
		{
			ICloneable result;
			_control.WaitOne();
			result = _value.Clone() as ICloneable;
			_control.ReleaseMutex();
			return result;
		}

		#endregion
	}

	/// <summary>
	/// Manages a cache of objects by keeping alive the objects with the biggest weight when the maximum
	/// size of the cache was reached
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CacheManager<T>
	{
		private int _maxSize = SdkSettings.Instance.MemoryBufferSize;
		private bool _enabled = true; //if the cache manager can store new objects

		/// <summary>
		/// Entries in the cache
		/// </summary>
		protected IDictionary<int, T> _entries;
		/// <summary>
		/// Index used to assign corresponding entries to a weight
		/// </summary>
		protected IDictionary<int, int> _weigthsIndex;
		/// <summary>
		/// Used to control multithreaded access
		/// </summary>
		protected static Mutex _mut = new Mutex();

		/// <summary>
		/// If the cache manager can store new objects
		/// </summary>
		public bool CacheEnabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
		
		/// <summary>
		/// Constructs a cache manager
		/// </summary>
		public CacheManager()
		{
			_entries = new Dictionary<int, T>();
			_weigthsIndex = new Dictionary<int, int>();
		}
		/// <summary>
		/// Gets/sets the max size of the cache
		/// </summary>
		public int MaxSize
		{
			get { return _maxSize; }
			set { _maxSize = value; }
		}

		/// <summary>
		/// Gets true if the buffer has reached it's maximum capacity
		/// </summary>
		public bool MaxSizeReached
		{
			get
			{
				_mut.WaitOne();

				bool result = _entries.Count >= _maxSize;

				_mut.ReleaseMutex();

				return result;
			}
		}

		/// <summary>
		/// Calculates an weight used to maintain the current entry in the cache
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected virtual int CalculateWeight(int key)
		{
			try
			{
				return _weigthsIndex[key] + 1; //by default the weight is the number of hits for the current entry
			}
			catch
			{
				return Int32.MaxValue;
			}
		}

		/// <summary>
		/// Removes an entry from the cache
		/// </summary>
		/// <param name="key"></param>
		protected virtual void RemoveEntry(int key)
		{
			_entries.Remove(key);
			_weigthsIndex.Remove(key);
		}

		/// <summary>
		/// Adds an entry to the cache
		/// </summary>
		/// <param name="key">The key of the entry in the cache. Usually the entry hash</param>
		/// <param name="entry"></param>
		public virtual void Add(int key, T entry)
		{
			if (!_enabled)
			{
				return; //don't do anything if the cache manager is not enables
			}
			_mut.WaitOne();

			//check that there's no other same key in the cache
			if (!_entries.ContainsKey(key))
			{
				//check that max size was not reached
				if (_entries.Count >= _maxSize)
				{
					int min = -1;
					int minKey = -1;
					//find the first key with the least amount of hits and remove it from the cache
					foreach (int weightKey in _weigthsIndex.Keys)
					{
						if (_weigthsIndex[weightKey] > min)
						{
							min = _weigthsIndex[weightKey];
							minKey = weightKey;
						}
					}
					RemoveEntry(minKey);
				}

				_entries.Add(key, entry);
				_weigthsIndex.Add(key, 0);
			}

			_mut.ReleaseMutex();
		}

		/// <summary>
		/// Replaces the entry at the key index with the value specified by the entry parameter
		/// </summary>
		/// <param name="key"></param>
		/// <param name="entry"></param>
		public void Replace(int key, T entry)
		{
			_mut.WaitOne();

			if (_entries.ContainsKey(key))
			{
				_entries[key] = entry;
			}

			_mut.ReleaseMutex();
		}

		/// <summary>
		/// Removes an entry from the cache
		/// </summary>
		/// <param name="key"></param>
		public void Remove(int key)
		{
			_mut.WaitOne();

			RemoveEntry(key);

			_mut.ReleaseMutex();
		}


		/// <summary>
		/// Clears the cache
		/// </summary>
		public void Clear()
		{
			_mut.WaitOne();
			_entries = new Dictionary<int, T>();
			_weigthsIndex = new Dictionary<int, int>();
			_mut.ReleaseMutex();
		}

		/// <summary>
		/// Gets an entry from the cache
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetEntry(int key)
		{
			//lock the current operation
			_mut.WaitOne();

			T result = default(T);
			if (_entries.ContainsKey(key))
			{
				result = _entries[key];
				_weigthsIndex[key] = CalculateWeight(key);
			}

			_mut.ReleaseMutex();

			return result;
		}

	}



}
