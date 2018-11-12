using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Common interface for search objects
	/// </summary>
	public interface ISearcher
	{

		/// <summary>
		/// Performs a search on the specified data source, using the defined criteria and adding to the result
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="criteriaList"></param>
		/// <param name="result"></param>
		void Search(ITrafficDataAccessor dataSource, SearchCriteriaSet criteriaList, ISearchResult result);

	}
}
