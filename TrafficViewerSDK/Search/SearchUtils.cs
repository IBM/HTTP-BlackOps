using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Collection of search results
	/// </summary>
	public interface ISearchResult : IList,ICloneable
	{
		/// <summary>
		/// Adds a list of elements to the current list
		/// </summary>
		/// <param name="collection"></param>
		void AddRange(IEnumerable collection);
	}

	/// <summary>
	/// Used to specify the area of the search
	/// </summary>
	public enum SearchContext
	{
		/// <summary>
		/// RequestLine
		/// </summary>
		RequestLine,
		/// <summary>
		/// RequestBody
		/// </summary>
		RequestBody,
		/// <summary>
		/// Request
		/// </summary>
		Request,
		/// <summary>
		/// Response
		/// </summary>
		Response,
		/// <summary>
		/// Full
		/// </summary>
		Full,
		/// <summary>
		/// None
		/// </summary>
		None
	}

	/// <summary>
	/// Cache for the search results
	/// </summary>
    public class SearchResultCache : CacheManager<ICacheable> 
    { 
    
        private static object _lock = new object();

        private static SearchResultCache _instance;

        private SearchResultCache()
        {
        }

        /// <summary>
        /// Returns the singleton
        /// </summary>
        public static SearchResultCache Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SearchResultCache();
                    }
                }
                return _instance;
            }
        }
    }

}
