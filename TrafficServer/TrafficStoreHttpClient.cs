using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;
using TrafficServer.Properties;
using System.Net;

namespace TrafficServer
{
	public class TrafficStoreHttpClient : IHttpClient
	{
		private const int MATCHES_LIMIT = 100;
		private const string ALERT_MATCH = @"alert(?:\(|%28)(\d+)";
		/// <summary>
		/// Returned by the GetNext method of an empty response set
		/// </summary>
		private const int NULL_INDEX = -1;
		
        
		bool _ignoreAuth = false;
		TrafficServerMode _matchMode = TrafficServerMode.BrowserFriendly;
		ITrafficDataAccessor _sourceStore = null;
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sourceStore">The data accessort containing the mock data</param>
		/// <param name="matchMode"></param>
		public TrafficStoreHttpClient(ITrafficDataAccessor sourceStore, TrafficServerMode matchMode, bool ignoreAuth)
		{
			_sourceStore = sourceStore;
			_matchMode = matchMode;
			_ignoreAuth = ignoreAuth;
		}

		/// <summary>
		/// Actually gets a matching response from the mock data
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		public HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
		{
			HttpResponseInfo responseInfo = null;
			string currentRequestString = requestInfo.ToString();
			string currentAlertId = Utils.RegexFirstGroupValue(currentRequestString, ALERT_MATCH);
			TrafficServerMode currentMatchMode = _matchMode;

			if (!String.IsNullOrEmpty(currentAlertId)) //override the redundancy tuning if we are trying to match a alert
			{
				currentMatchMode = TrafficServerMode.BrowserFriendly;	
			}

			//parse the request variables because we will need them to construct the hash
			requestInfo.ParseVariables();


			TrafficServerResponseSet responseSet = null;

			//look in the server cache for the request
			ICacheable entry =
				TrafficServerCache.Instance.GetEntry(requestInfo.GetHashCode(currentMatchMode));


			if (entry != null)
			{
				responseSet = entry.Reserve() as TrafficServerResponseSet;
				entry.Release();
			}

			TrafficServerResponseSet similarRequests = new TrafficServerResponseSet();

			if (responseSet == null)
			{
				//create a new empty response set
				responseSet = new TrafficServerResponseSet();

				RequestSearcher searcher = new RequestSearcher();

				SearchCriteriaSet criteriaSet;

				criteriaSet = GetCriteriaSet(requestInfo, currentMatchMode);

				RequestMatches matches = new RequestMatches();

				//do the search!
				searcher.Search(_sourceStore, criteriaSet, matches);

				//normalize the matches and keep only the ones that have the same variables and values
				if (matches.Count > 0)
				{
					HttpRequestInfo original;
					HttpRequestInfo found;
					byte[] requestBytes;
					int i, n = matches.Count;
					for (i = 0; i < n & i < MATCHES_LIMIT; i++)
					{
						int match = matches[i];
						TVRequestInfo header = _sourceStore.GetRequestInfo(match);
						if (_ignoreAuth)
						{
							if (
								String.Compare(header.ResponseStatus,"401",true) == 0 ||
								String.Compare(header.ResponseStatus,"407",true) == 0)
							{
								HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "Skipping authentication challenge");
								//simply skip 401 matches
								continue;
							}
						}

						if (String.Compare(header.Description, Resources.TrafficLogProxyDescription, true) == 0)
						{ 
							//is likely that the source store is also the save store and this may be
							//the current request being saved
							HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "Skipping request to traffic store");
							continue;
						}


						requestBytes = _sourceStore.LoadRequestData(match);
						string requestString = Constants.DefaultEncoding.GetString(requestBytes);

						if (String.IsNullOrEmpty(requestString))
						{
							continue; //skip the current match is incorrect
						}
						original = new HttpRequestInfo(DynamicElementsRemover.Remove(currentRequestString));
						found = new HttpRequestInfo(DynamicElementsRemover.Remove(requestString));

						if (RequestMatcher.IsMatch(original, found, TrafficServerMode.Strict))
						{
							responseSet.Add(match);
						}
						else if (currentMatchMode != TrafficServerMode.Strict
							&& RequestMatcher.IsMatch(original, found, currentMatchMode))
						{
							similarRequests.Add(match);
						}

					}
					//if no exact requests were found
					if (responseSet.Count == 0 && similarRequests.Count > 0)
					{
						HttpServerConsole.Instance.WriteLine
							(LogMessageType.Warning, "Warning, exact match was not found for {0} returning a similar request.",
							requestInfo.RequestLine);
					}
					responseSet.AddRange(similarRequests.Matches);
				}

				//add this response set to the cache
				TrafficServerCache.Instance.Add(requestInfo.GetHashCode(currentMatchMode), new CacheEntry(responseSet));
			}

			//get the next response id from the response set
			int requestId = responseSet.GetNext();

			if (requestId == NULL_INDEX)
			{

				HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "(404) Request not found: {0}"
					, requestInfo.RequestLine);
				//the request was not found at all, return a 404
				return new HttpResponseInfo(HttpErrorResponse.GenerateHttpErrorResponse(HttpStatusCode.NotFound, "Request Not Found", 
					"<html><head><title>Error code: 404</title><body><h1>Request was not found or variables didn't match.</h1></body></html>"));

			}

			if (requestId != NULL_INDEX)
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Returning response from request id: {0}", requestId);
				byte[] responseBytes = _sourceStore.LoadResponseData(requestId);

				if (responseBytes != null)
				{
					responseInfo = new HttpResponseInfo(responseBytes);

					if (!String.IsNullOrEmpty(currentAlertId))
					{
						Encoding encoding = HttpUtil.GetEncoding(responseInfo.Headers["Content-Type"]);
						string responseString = encoding.GetString(responseBytes);
						responseString = Utils.ReplaceGroups(responseString, ALERT_MATCH, currentAlertId);
						responseInfo = new HttpResponseInfo(encoding.GetBytes(responseString));
					}
					
				}

				//add the request id header
				responseInfo.Headers.Add("Traffic-Store-Req-Id", requestId.ToString());
			}

			return responseInfo;
		}

		/// <summary>
		/// Gets a search criteria for the request info
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <param name="currentMatchMode"></param>
		/// <returns></returns>
		private SearchCriteriaSet GetCriteriaSet(HttpRequestInfo requestInfo, TrafficServerMode currentMatchMode)
		{
			SearchCriteriaSet criteriaSet;
			criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, true, requestInfo.SearchRegex));

			//reduce the search area by adding the variable names and values as seach matches

			bool incVal = currentMatchMode != TrafficServerMode.BrowserFriendly;
			
			if (requestInfo.QueryVariables.Count > 0)
			{
				criteriaSet.Add(
					requestInfo.QueryVariables.GetSearchCriteria(incVal));
			}

			//for POST requests add the body variables as criteria
			if (requestInfo.ContentLength > 0)
			{
				if (requestInfo.BodyVariables.Count > 0)
				{
					criteriaSet.Add(
						requestInfo.BodyVariables.GetSearchCriteria(incVal));
				}
				else
				{
					//we are dealing with custom parameters so just add the full post data as
					//a criteria
					criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, false, requestInfo.ContentDataString));
				}
			}
			return criteriaSet;
		}

		public void SetNetworkSettings(INetworkSettings networkSettings)
		{
			; //doesn't do anything
		}
	}
}
