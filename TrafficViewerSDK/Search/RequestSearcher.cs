using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Matches returned by request searcher
	/// </summary>
	public class RequestMatches : List<int>, ISearchResult
	{

		#region ISearchResult Members
		/// <summary>
		/// Adds a range of results
		/// </summary>
		/// <param name="collection"></param>
		public void AddRange(System.Collections.IEnumerable collection)
		{
			base.AddRange((IEnumerable<int>)collection);
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Clones the current object
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			RequestMatches clone = new RequestMatches();
			clone.AddRange(this);
			return clone;
		}

		#endregion
	}


	/// <summary>
	/// Finds requests that match the specified criteria
	/// </summary>
	public class RequestSearcher : BaseSearcher
	{
		
		/// <summary>
		/// Retrive any cached subsets for the criterias of this search
		/// </summary>
		/// <param name="criteriaSet"></param>
		/// <returns></returns>
		protected override SearchSubset GetSearchSubset(SearchCriteriaSet criteriaSet)
		{
			SearchSubset subset = base.GetSearchSubset(criteriaSet);
			//increment the start criteria index past the criteria of the subset
			//explanation: for searches where we don't need to extract line information or the text of the match
			//we don't need to re-check the criteria
			if (subset != null)
			{
				criteriaSet.StartCriteriaIndex++;
			}
			return subset;
		}

		/// <summary>
		/// Verifies that a request matches
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="header"></param>
		/// <param name="criteriaSet"></param>
		/// <param name="result"></param>
		protected override void RequestMatches(ITrafficDataAccessor dataSource, TVRequestInfo header, SearchCriteriaSet criteriaSet, ISearchResult result)
		{
			bool found = true;
			int hashSum = criteriaSet.DescriptionFilter.GetHashCode();

			if (criteriaSet.DescriptionFilter == null ||
				criteriaSet.DescriptionFilter == String.Empty ||
				header.Description.IndexOf(criteriaSet.DescriptionFilter,
					StringComparison.CurrentCultureIgnoreCase) > -1)
			{
				//apply an AND operation with all the search criterias from the last search criteria cached

				int i, n = criteriaSet.Count, start = criteriaSet.StartCriteriaIndex;

				for (i = start; i < n; i++)
				{
					ISearchCriteria criteria = criteriaSet[i];
					//compose the text to search
					string toBeSearched;

					if (criteria.Context == SearchContext.RequestLine)
					{
						toBeSearched = header.RequestLine;
					}
					else if (criteria.Context == SearchContext.RequestBody || criteria.Context == SearchContext.Request)
					{
						toBeSearched = Constants.DefaultEncoding.GetString(dataSource.LoadRequestData(header.Id));
						if (criteria.Context == SearchContext.RequestBody)
						{
							HttpRequestInfo reqInfo = new HttpRequestInfo(toBeSearched);
							toBeSearched = reqInfo.ContentDataString;
						}
					}
					else if (criteria.Context == SearchContext.Response)
					{
						toBeSearched = Constants.DefaultEncoding.GetString(dataSource.LoadResponseData(header.Id));
					}
					else
					{
						string request = Constants.DefaultEncoding.GetString(dataSource.LoadRequestData(header.Id));
						string response = Constants.DefaultEncoding.GetString(dataSource.LoadResponseData(header.Id));
						StringBuilder sb = new StringBuilder(request.Length + response.Length);
						sb.Append(request);
						sb.Append(response);
						toBeSearched = sb.ToString();
					}

					found = criteria.Matches(toBeSearched);

					if (found) //if found is still true cache the request id as a match for the current set of criterias
					{
						hashSum = hashSum ^ criteria.GetHashCode();

						ICacheable entry = SearchSubsetsCache.Instance.GetEntry(hashSum);

						SearchSubset subset;

						if (entry == null)
						{
							subset = new SearchSubset();
							subset.Add(header.Id);
							SearchSubsetsCache.Instance.Add(hashSum, new CacheEntry(subset));
						}
						else
						{
							subset = entry.Reserve() as SearchSubset;
							//if (!subset.Contains(header.Id)) //not checking if the entry exists for performance reasons
							//{
								subset.Add(header.Id);
							//}
							entry.Release();
						}
					}
					else
					{
						break; //criteria didn't match, break the loop
					}
				} //end of for, all the criterias were matched or one criteria did not match
				
				if (found)
				{
					//add all the temp matches to the results
					result.Add(header.Id);
				}		
			}//end description check
		}

	}
}
