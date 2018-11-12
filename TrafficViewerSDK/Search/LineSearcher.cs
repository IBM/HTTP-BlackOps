using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.Search
{
	/// <summary>
	/// Line matches collection
	/// </summary>
	public class LineMatches : List<LineMatch>, ISearchResult
	{

		#region ISearchResult Members

		/// <summary>
		/// Adds a range of results
		/// </summary>
		/// <param name="collection"></param>
		public void AddRange(System.Collections.IEnumerable collection)
		{
			base.AddRange((IEnumerable <LineMatch>) collection);
		}

		#endregion

		#region ICloneable Members
		/// <summary>
		/// Clones the current object
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			LineMatches clone = new LineMatches();
			clone.AddRange(this);
			return clone;
		}

		#endregion
	}
	
	/// <summary>
	/// Contains information about a line match
	/// </summary>
	public class LineMatch
	{
		private int _requestId;
		/// <summary>
		/// The request where the match was found
		/// </summary>
		public int RequestId
		{
			get { return _requestId; }
			set { _requestId = value; }
		}


		private SearchContext _context;
		/// <summary>
		/// The context where the match was found
		/// </summary>
		public SearchContext Context
		{
			get { return _context; }
			set { _context = value; }
		}

		private string _line;
		/// <summary>
		/// The value of the line where the match was found
		/// </summary>
		public string Line
		{
			get { return _line; }
			set { _line = value; }
		}

		List<MatchCoordinates> _matchCoordinates;
		/// <summary>
		/// Gets/sets the location(s) of the match
		/// </summary>
		public List<MatchCoordinates> MatchCoordinatesList
		{
			get { return _matchCoordinates; }
			set { _matchCoordinates = value; }
		}


		/// <summary>
		/// Contains information about a line match
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="line"></param>
		/// <param name="matchCoordinates">The locations and lengths of matches</param>
		/// <param name="context">The location of the match</param>
		public LineMatch(int requestId, string line, List<MatchCoordinates> matchCoordinates, SearchContext context)
		{
			_requestId = requestId;
			_line = line;
			_matchCoordinates = matchCoordinates;
			_context = context;
		}
	}


	/// <summary>
	/// Searches for matches in every line
	/// </summary>
	public class LineSearcher : BaseSearcher
	{
		private const int ESTIMATED_LINE_LEN = 1024;


		/// <summary>
		/// Verifies a request
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="header"></param>
		/// <param name="criteriaSet"></param>
		/// <param name="result"></param>
		protected override void RequestMatches(ITrafficDataAccessor dataSource, TVRequestInfo header, SearchCriteriaSet criteriaSet, ISearchResult result)
		{
			bool found = false;
			int hashSum = criteriaSet.DescriptionFilter.GetHashCode();
			
			List<MatchCoordinates> matchCoordinates; //variable used to obtain match coordinates

			//create a temp list to store all the matches
			LineMatches tempMatches = new LineMatches();

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
					found = false;
					//compose the text to search
					if (criteria.Context == SearchContext.RequestLine)
					{
						if (criteria.Matches(header.RequestLine, 0, out matchCoordinates))
						{
							//if we got here it means that the request line matches
							//using the request search context since we really care only for request or response
                            found = true;
							tempMatches.Add(new LineMatch(header.Id, header.RequestLine, matchCoordinates, SearchContext.Request));
						}
						else
						{
							found = false;
							break; //one of the criteria did not match so the end result is false
						}
					}
					else
					{
					
						//search in request first
						if(criteria.Context == SearchContext.Full || criteria.Context == SearchContext.Request || criteria.Context == SearchContext.RequestBody)
						{
							found = SearchInContext(dataSource, header, criteria, SearchContext.Request, ref tempMatches);
						}
						if (criteria.Context == SearchContext.Full || criteria.Context == SearchContext.Response)
						{
							found |= SearchInContext(dataSource, header, criteria, SearchContext.Response, ref tempMatches);
						}

						if (!found)
						{
							break; //no match was found in the entire stream
						}
					}

					//if at the end of criteria iteration found is still true cache the hashSum of this criteria
					//so we can get the sublist of request ids in future searches
					if (found)
					{
						hashSum = hashSum ^ criteria.GetHashCode();

						SearchSubset subset = null;

						ICacheable entry = SearchSubsetsCache.Instance.GetEntry(hashSum);

						if (entry == null)
						{
							subset = new SearchSubset();
							subset.Add(header.Id);
							SearchSubsetsCache.Instance.Add(hashSum, new CacheEntry(subset));
						}
						else
						{
							subset = entry.Reserve() as SearchSubset;
							if (!subset.Contains(header.Id))
							{
								subset.Add(header.Id);
							}
							entry.Release();
						}

					}

				} //end of for, all the criterias were matched or one criteria did not match

				if (found)
				{ 
					//add all the temp matches to the results
					result.AddRange(tempMatches);
				}
			}
		}

		/// <summary>
		/// Searches in the specified context
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="header"></param>
		/// <param name="criteria"></param>
		/// <param name="context"></param>
		/// <param name="matches"></param>
		/// <returns></returns>
		private bool SearchInContext(
			ITrafficDataAccessor dataSource, 
			TVRequestInfo header, 
			ISearchCriteria criteria, 
			SearchContext context, 
			ref LineMatches matches)
		{
			bool found;
			MemoryStream toBeSearched;
			List<MatchCoordinates> matchCoordinates;

			toBeSearched = GetMemoryStream(dataSource, header, context);

			byte[] lineBytes;
			found = false;
			bool searchLine = criteria.Context != SearchContext.RequestBody;
			int relativePosition = 0;
			while ((lineBytes = Utils.ReadLine(toBeSearched, ESTIMATED_LINE_LEN, LineEnding.Any)) != null)
			{
				string line = Constants.DefaultEncoding.GetString(lineBytes);

				if (searchLine)
				{
					if (criteria.Matches(line, relativePosition, out matchCoordinates))
					{
						found = true;

						matches.Add(new LineMatch(header.Id, line, matchCoordinates, context));

					}
				}
				else
				{
					if (line == String.Empty) //if a \r\n was hit in the request we're in the req body
					{
						searchLine = true;
					}
				}

				//update the relative position with the current stream position, before reading a new line
				relativePosition = (int)toBeSearched.Position;
			}
			return found;
		}
	}
}
