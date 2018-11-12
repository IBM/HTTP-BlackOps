using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Encapsulates the main search algorithm
	/// </summary>
	public abstract class BaseSearcher : ISearcher
	{

		private bool _stopSearch = false;


		/// <summary>
		/// Stops the search
		/// </summary>
		public void StopSearch()
		{
			_stopSearch = true;
		}


		/// <summary>
		/// Verifies if a request matches the search criteria
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="header"></param>
		/// <param name="criteriaSet"></param>
		/// <param name="result"></param>
		protected abstract void RequestMatches(ITrafficDataAccessor dataSource,
												TVRequestInfo header,
												SearchCriteriaSet criteriaSet,
												ISearchResult result);

		/// <summary>
		/// Calculates a hash to uniquely identify a search
		/// </summary>
		protected virtual int GetSearchHash(ITrafficDataAccessor dataSource, SearchCriteriaSet criteriaSet)
		{
			return dataSource.GetHashCode()^criteriaSet.GetHashCode();
		}

		/// <summary>
		/// Gets a  memory stream to search in for the specified context
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="header"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected MemoryStream GetMemoryStream(ITrafficDataAccessor dataSource, TVRequestInfo header, SearchContext context)
		{
			MemoryStream toBeSearched;
			if (context == SearchContext.Request || context == SearchContext.RequestBody)
			{
				toBeSearched = new MemoryStream(dataSource.LoadRequestData(header.Id));
			}
			else if (context == SearchContext.Response)
			{
				toBeSearched = new MemoryStream(dataSource.LoadResponseData(header.Id));
			}
			else
			{
				toBeSearched = new MemoryStream();
				byte[] data = dataSource.LoadRequestData(header.Id);
				toBeSearched.Write(data, 0, data.Length);
				data = Constants.DefaultEncoding.GetBytes(Environment.NewLine);
				toBeSearched.Write(data, 0, data.Length);
				data = dataSource.LoadResponseData(header.Id);
				toBeSearched.Write(data, 0, data.Length);
				//reset the prosition so readline can start from the beggining
				toBeSearched.Position = 0;
			}
			return toBeSearched;
		}


		/// <summary>
		///  Gets a subset of search results to search in based on the current criteria
		/// </summary>
		/// <param name="criteriaSet"></param>
		/// <returns>Search subset</returns>
		protected virtual SearchSubset GetSearchSubset(SearchCriteriaSet criteriaSet)
		{
			ICacheable finalEntry = null;
			int hashSum = criteriaSet.DescriptionFilter.GetHashCode();
			int i, n = criteriaSet.Count;
			for (i = 0; i < n; i++)
			{
				ISearchCriteria criteria = criteriaSet[i];
				//combine the current hash with the pervious hash
				hashSum = hashSum ^ criteria.GetHashCode();
				//check if a subset is cached for this hash
				ICacheable entry = SearchSubsetsCache.Instance.GetEntry(hashSum);
				if (entry != null)
				{
					criteriaSet.StartCriteriaIndex = i;
					finalEntry = entry;
				}
				else
				{
					break;
				}
			}

			SearchSubset subset = null;
			if (finalEntry != null)
			{
				subset = finalEntry.GetClone() as SearchSubset;
			}

			return subset;
		}

		/// <summary>
		/// Searches only in the specified subset
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="criteriaSet"></param>
		/// <param name="result"></param>
		/// <param name="subset"></param>
		private void SubsetSearch(ITrafficDataAccessor dataSource, SearchCriteriaSet criteriaSet, ISearchResult result, SearchSubset subset)
		{
			foreach (int reqId in subset)
			{
				TVRequestInfo header = dataSource.GetRequestInfo(reqId);
				if (_stopSearch) break;
				if (header != null)
				{
					RequestMatches(dataSource, header, criteriaSet, result);
				}
			}
		}

		/// <summary>
		/// Searches the entire data accessor for the specified criteria
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="criteriaSet"></param>
		/// <param name="result"></param>
		private void FullSearch(ITrafficDataAccessor dataSource, SearchCriteriaSet criteriaSet, ISearchResult result)
		{
			TVRequestInfo header;
			int currReqId = -1;
			while ((header = dataSource.GetNext(ref currReqId)) != null && !_stopSearch)
			{
				RequestMatches(dataSource, header, criteriaSet, result);
			}
		}

		#region ISearcher Members

		/// <summary>
		/// Performs a search
		/// </summary>
		/// <param name="dataSource">Traffic data accesssor</param>
		/// <param name="criteriaSet">Set of criteria for the search</param>
		/// <param name="result">The search result</param>
		public void Search(ITrafficDataAccessor dataSource, SearchCriteriaSet criteriaSet,  ISearchResult result)
		{
			_stopSearch = false;
			//check the search results cache
			int hash = GetSearchHash(dataSource, criteriaSet);
			ICacheable entry = SearchResultCache.Instance.GetEntry(hash);

			ISearchResult cachedResult = null;
			if (entry != null)
			{
				cachedResult = entry.Reserve() as ISearchResult;
				entry.Release();
			}

			if (cachedResult == null)
			{
				//check for a subset
				SearchSubset subset = GetSearchSubset(criteriaSet);
				if (subset == null)
				{
					FullSearch(dataSource, criteriaSet, result);
				}
				else
				{
					SubsetSearch(dataSource, criteriaSet, result, subset);
				}
				//cache the search
				SearchResultCache.Instance.Add(hash, new CacheEntry(result));
			}
			else
			{
				result.AddRange(cachedResult);
			}
		}

		#endregion
	}
}
