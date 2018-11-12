using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Stores the positions found to this point
	/// </summary>
	public class TimeStampsPositionsCache : CacheManager<long>
	{
		private int CalculateKeyValue(DateTime time)
		{
			return 10000 * time.Hour + 100 * time.Minute + time.Second;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public TimeStampsPositionsCache()
		{
			_entries = new SortedDictionary<int, long>();
		}

		/// <summary>
		/// Adds a position to the cache
		/// </summary>
		/// <param name="key"></param>
		/// <param name="position"></param>
		public void Add(DateTime key, long position)
		{
			int intKey = CalculateKeyValue(key);
			Add(intKey, position);
		}

		/// <summary>
		/// Returns the closest entry to the specified time
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public long FindBestEntry(DateTime time)
		{
			long pos = 0;

			List<int> keys = new List<int>();
			keys.AddRange(_entries.Keys);

			if (keys.Count > 0)
			{
				int low = 0, high = keys.Count - 1, mid = 0;
				int intKey = CalculateKeyValue(time);

				while (low <= high)
				{
					mid = (low + high) / 2;
					if (keys[mid] == intKey)
					{
						return GetEntry(keys[mid]);
					}
					else
					{
						if (keys[mid] > intKey)
						{
							high = mid - 1;
						}
						else
						{
							low = mid + 1;
						}
					}
				}

				//if no exact match was found return the closest possible match
				//this will also return 0 if the cache is empty

				if (keys[mid] < intKey)
				{
					pos = GetEntry(keys[mid]);
				}
			}

			return pos;
		}
	}
}
