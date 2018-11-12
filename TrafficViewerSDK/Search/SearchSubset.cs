using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Collection of search subsets
	/// </summary>
	public class SearchSubset : List<int>,ICloneable 
	{
		#region ICloneable Members
		/// <summary>
		/// Makes a clone
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			SearchSubset clone = new SearchSubset();
			clone.AddRange(this);
			return clone;
		}

		#endregion
	}


	/// <summary>
	/// Caches lists of request ids that are subsets for search criterias
	/// </summary>
    public class SearchSubsetsCache : CacheManager<ICacheable>
	{

        private static object _lock = new object();

        private static SearchSubsetsCache _instance;

        private SearchSubsetsCache()
        {
        }

        /// <summary>
        /// Returns the singleton
        /// </summary>
        public static SearchSubsetsCache Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SearchSubsetsCache();
                    }
                }
                return _instance;
            }
        }

		/// <summary>
		/// Calculates the weight of a cache element
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected override int CalculateWeight(int key)
		{
			try
			{
				SearchSubset set = _entries[key].Reserve() as SearchSubset;
				int result = _weigthsIndex[key] + set.Count + 1;
				_entries[key].Release();
				return result;
			}
			catch
			{
				return Int32.MaxValue;
			}
		}
	}
}
