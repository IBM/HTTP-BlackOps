using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Options;

namespace TrafficServer
{
	/// <summary>
	/// Allows trapping the request and response before sending it back to the user
	/// </summary>
	public class AdvancedExploreProxyConnection : ManualExploreProxyConnection
	{

		private Dictionary<string, string> _requestReplacements;
        private Dictionary<string, string> _responseReplacements;

		private bool _trackRequestContext = false;
		private const string REQ_CONTEXT_ID = "R_CI";
		private const string RX_GROUP = "(.+)";
		private const string HEX_VAL = "HEX_VAL";
		private const string HEX_REGEX = "[a-fA-F0-9]{16,64}";
		private const int MAX_REQUEST_CONTEXT_SIZE = 100;

        private Random _rng = new Random();
        private IDictionary<string,Func<string>> _dynamicReplacements = new Dictionary<string,Func<String>>();
		/// <summary>
		/// Constant used to identify alias urls
		/// </summary>
		public readonly static string REQ_ID_STRING = "reqId";

		private Dictionary<string, TrackingPattern> _autoTrackingPatternList;
		/// <summary>
		/// The list of autotracking parameters
		/// </summary>
		protected Dictionary<string, TrackingPattern> AutoTrackingPatternList
		{
			get { return _autoTrackingPatternList; }
		}

		/// <summary>
		/// Constructor for an advanced proxy connection
		/// </summary>
		/// <param name="tcpClient"></param>
		/// <param name="isSecure"></param>
		/// <param name="dataStore"></param>
		/// <param name="description"></param>
		/// <param name="networkSettings">Settings used for the connection, proxy etc.</param>
		/// <param name="replacements">Strings to be replaced in the request</param>
		public AdvancedExploreProxyConnection(TcpClient tcpClient, bool isSecure, ITrafficDataAccessor dataStore, string description, INetworkSettings networkSettings, bool trackRequestContext)
			: base(tcpClient, isSecure, dataStore, description, networkSettings)
		{
			_requestReplacements = dataStore.Profile.GetRequestReplacements();
            _responseReplacements = dataStore.Profile.GetResponseReplacements();
			_trackRequestContext = trackRequestContext;

			_autoTrackingPatternList = new Dictionary<string, TrackingPattern>();

            _dynamicReplacements.Add(Constants.SEQUENCE_VAR_PATTERN, () => { return DateTime.Now.Ticks.ToString(); });
            _dynamicReplacements.Add(Constants.GUID_VAR_PATTERN, () => { return Guid.NewGuid().ToString(); });
            _dynamicReplacements.Add("__RSHORT__", () => { return this._rng.Next(0, short.MaxValue).ToString(); });
            
			var trackingPatterns = dataStore.Profile.GetTrackingPatterns();
			foreach (var key in trackingPatterns.Keys)
			{
				TrackingPattern item = trackingPatterns[key];
				if (item.TrackingType == TrackingType.AutoDetect)
				{
					_autoTrackingPatternList.Add(item.Name, item);

				}

			}

		}


		/// <summary>
		/// Updates dynamic values in the request
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		protected string UpdateDynamicPatterns(string rawRequest)
		{
            foreach (KeyValuePair<String,Func<String>> replace in _dynamicReplacements)
            {
                if (Utils.IsMatch(rawRequest, replace.Key))
                {
                    rawRequest = Regex.Replace(rawRequest, replace.Key, replace.Value());
                }
            }
            
			return rawRequest;
		}


        /// <summary>
        /// Remove request headers that can cause issues with the recording
        /// </summary>
        /// <param name="reqInfo">The request info to be processed</param>
        /// <param name="isNonEssential">Whether this has deemed to be non-essential</param>
        /// <returns></returns>
        protected override HttpRequestInfo ProcessHeaders(HttpRequestInfo reqInfo, bool isNonEssential)
        {
           //in advanced explore proxy headers are removed through request replacements
            
            return reqInfo;
        }

		/// <summary>
		/// Triggered before the request is sent to the site
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		protected override HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
		{
            bool wasModified = false;
			if (_requestReplacements != null)
			{
				string request = requestInfo.ToString();
                
                

				foreach (string key in _requestReplacements.Keys)
                {
                    if (Utils.IsMatch(request, key))
                    {
                        request = Utils.ReplaceGroups(request, key, _requestReplacements[key]);
                        wasModified = true;
                        HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Request replacement applied for pattern: '{0}'", key);
                    }
				}

                if (wasModified)
                {
                    requestInfo = new HttpRequestInfo(request, false);
                    //update the content length
                    if (requestInfo.ContentData != null)
                    {
                        requestInfo.Headers["Content-Length"] = requestInfo.ContentData.Length.ToString();
                    }
                    requestInfo.IsSecure = ClientStreamWrapper.IsSecure;
                }
			}

			
			if (!_isNonEssential)
			{
				var originalRawRequest = requestInfo.ToString();
				//update tracked values
				string updatedRequest = PatternTracker.Instance.UpdateRequest(originalRawRequest);
				updatedRequest = UpdateDynamicPatterns(updatedRequest);
				if (!originalRawRequest.Equals(updatedRequest))
				{
					bool isSecure = requestInfo.IsSecure;
					requestInfo = new HttpRequestInfo(updatedRequest, false);
					requestInfo.IsSecure = isSecure;
                    wasModified = true;	
				}

                if (wasModified)
                {
                    //update the request in the file
                    TrafficDataStore.SaveRequest(CurrDataStoreRequestInfo.Id, Constants.DefaultEncoding.GetBytes(updatedRequest));
                }
			}
			

			if (CurrDataStoreRequestInfo != null)
			{
				//see if the request was trapped
				if (HttpTrap.Instance.TrapRequests &&
					HttpTrap.Instance.TrapRequest(CurrDataStoreRequestInfo, requestInfo))
				{
					//then we need to update the request info from the data source
					requestInfo = new HttpRequestInfo(TrafficDataStore.LoadRequestData(CurrDataStoreRequestInfo.Id), false);
					//update the content length
					if (requestInfo.ContentData != null)
					{
						requestInfo.Headers["Content-Length"] = requestInfo.ContentData.Length.ToString();
					}
					requestInfo.IsSecure = ClientStreamWrapper.IsSecure;
				}
			}

			


			return requestInfo;
		}



		private void TrackRequestContext(HttpRequestInfo requestInfo)
		{


			foreach (TrackingPattern pattern in _autoTrackingPatternList.Values)
			{
				string rawRequest = requestInfo.ToString();

				string needle = Utils.RegexFirstGroupValue(rawRequest, pattern.RequestPattern);

				if (String.IsNullOrWhiteSpace(needle)) continue;


				//first search for the path of the current request in responses
				LineMatches results = SearchParameterValue(needle);

				if (results.Count == 0)
				{
					needle = Utils.UrlDecode(needle);
					results = SearchParameterValue(needle);
				}

				//if any of the two searches returned results
				if (results.Count != 0)
				{
					//get the last match to extract the request context
					var match = results[results.Count - 1];
					CurrDataStoreRequestInfo.RefererId = match.RequestId;
					//replace the path in the match
					string requestContext = match.Line.Replace(needle, REQ_CONTEXT_ID);

					if (requestContext.Length > MAX_REQUEST_CONTEXT_SIZE)
					{
						requestContext = TrimRequestContext(requestContext);
					}

					//also replace hexadecimal values
					requestContext = Regex.Replace(requestContext, HEX_REGEX, HEX_VAL);

					//escape the line
					requestContext = Regex.Escape(requestContext);
					//insert the group
					requestContext = requestContext.Replace(REQ_CONTEXT_ID, RX_GROUP);
					//insert the HEX regex
					requestContext = requestContext.Replace(HEX_VAL, HEX_REGEX);

					CurrDataStoreRequestInfo.RequestContext = requestContext;
					CurrDataStoreRequestInfo.TrackingPattern = pattern.Name;

					TrafficDataStore.UpdateRequestInfo(CurrDataStoreRequestInfo);

					string originalPath = requestInfo.Path;
					CurrDataStoreRequestInfo.UpdatedPath = originalPath;

					//change the path of the request
					HttpRequestInfo newReq = new HttpRequestInfo(requestInfo.ToArray(false), false);

					//we are only replacing the last portion of the path and the query string to prevent relative path issues and also cookie path issues
					int lastIndexOfSlash = originalPath.LastIndexOf('/');
					if (lastIndexOfSlash >= 0)
					{
						originalPath = originalPath.Substring(0, lastIndexOfSlash + 1);
					}


					newReq.Path = String.Format("{0}{1}{2}", originalPath, REQ_ID_STRING, CurrDataStoreRequestInfo.Id);

					TrafficDataStore.SaveRequest(CurrDataStoreRequestInfo.Id, newReq.ToArray(false));

					HttpServerConsole.Instance.WriteLine
					("Found request context for request '{0}' id: {1}, referer id:{2}",
					requestInfo.Path, CurrDataStoreRequestInfo.Id, CurrDataStoreRequestInfo.RefererId);
					HttpServerConsole.Instance.WriteLine
					(requestContext);

					return; //we can only have one tracking pattern per request

				}
			}
		}

		/// <summary>
		/// Trims around the request context id so only MAX_REQUEST_CONTEXT chars are left
		/// </summary>
		/// <param name="requestContext"></param>
		/// <returns></returns>
		private static string TrimRequestContext(string requestContext)
		{
			//the request context is too big and might be harder to match, trim the ends
			int charsRemaining = MAX_REQUEST_CONTEXT_SIZE - REQ_CONTEXT_ID.Length;
			int leftPos = requestContext.IndexOf(REQ_CONTEXT_ID);
			if (leftPos < 0) return String.Empty;
			int rightPos = leftPos + REQ_CONTEXT_ID.Length;
			bool remainingRight = true;
			bool remainingLeft = true;
			while (charsRemaining > 0 && (remainingLeft || remainingRight))
			{
				if (rightPos < requestContext.Length)
				{
					rightPos++;
					charsRemaining--;
				}
				else
				{
					remainingRight = false;
				}

				if (leftPos > 0)
				{
					leftPos--;
					charsRemaining--;
				}
				else
				{
					remainingLeft = false;
				}

			}

			requestContext = requestContext.Substring(leftPos, rightPos - leftPos);
			return requestContext;
		}

		private LineMatches SearchParameterValue(string paramValue)
		{
			SearchContext context = SearchContext.Response;

			var searcher = new LineSearcher();
			//clear the search caches
			SearchResultCache.Instance.Clear();
			SearchSubsetsCache.Instance.Clear();

			var criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(context,
	 true, "Content-Type: (text|application)"));

			criteriaSet.Add(new SearchCriteria(context,
				 false, paramValue));


			LineMatches results = new LineMatches();
			searcher.Search(TrafficDataStore, criteriaSet, results);
			return results;
		}

		/// <summary>
		/// Triggered before the response is sent back to the client
		/// </summary>
		/// <param name="responseInfo"></param>
		/// <returns></returns>
		protected override HttpResponseInfo OnBeforeResponseToClient(HttpResponseInfo responseInfo)
		{
            if (_responseReplacements != null && _responseReplacements.Count > 0)
            {
                string response = responseInfo.ToString();
                bool wasReplaced = false;

                foreach (string key in _responseReplacements.Keys)
                {
                    if (Utils.IsMatch(response, key))
                    {
                        response = Utils.ReplaceGroups(response, key, _responseReplacements[key]);
                        HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Response replacement applied for pattern: '{0}'", key);
                        wasReplaced = true;
                    }
                }

                if (wasReplaced)
                {
                    //remove chunked encoding
                    response = Regex.Replace(response,"Transfer-Encoding:\\s?chunked\\r\\n","", RegexOptions.IgnoreCase);
                    responseInfo = new HttpResponseInfo(response);
                }
            }

			if (!_isNonEssential)
			{
				//update tracked paterns from the response
				PatternTracker.Instance.UpdatePatternValues(responseInfo);
			}


			if (CurrDataStoreRequestInfo != null)
			{
				//trap the response (this will only occur if the trap is enabled)
				if (HttpTrap.Instance.TrapResponses)
				{
					TrafficDataStore.SaveResponse(CurrDataStoreRequestInfo.Id, responseInfo.ToArray());

					if (HttpTrap.Instance.TrapResponse(CurrDataStoreRequestInfo, responseInfo))
					{
						responseInfo = new HttpResponseInfo(TrafficDataStore.LoadResponseData(CurrDataStoreRequestInfo.Id));

						if (responseInfo.ResponseBody != null && responseInfo.ResponseBody.IsChunked == false)
						{
							responseInfo.Headers["Content-Length"] = responseInfo.ResponseBody.Length.ToString();
						}
					}
				}

				if (_trackRequestContext)
				{
					//replace the request with its request id
					//store information about where the request was found in previous traffic responses stored in the
					//current traffic file
					TrackRequestContext(_requestInfo);
				}

			}
			return responseInfo;
		}
	}
}
