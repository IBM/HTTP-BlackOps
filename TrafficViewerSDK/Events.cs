using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Search;
using System.Threading;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Used to signal that the user requested a replace operation
	/// </summary>
	/// <param name="e"></param>
	public delegate void ReplaceEvent(ReplaceEventArgs e);
	/// <summary>
	/// Information regarding the replace
	/// </summary>
	public class ReplaceEventArgs : EventArgs
	{
		private IList<LineMatch> _matches;
		/// <summary>
		/// Gets a list of one or more matches to be replaced
		/// </summary>
		public IList<LineMatch> Matches
		{
			get { return _matches; }
		}


		private string _replacement;
		/// <summary>
		/// The replacement string
		/// </summary>
		public string Replacement
		{
			get { return _replacement; }
		}

		/// <summary>
		/// Information about what needs to be replaced
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="replacement"></param>
		public ReplaceEventArgs(IList<LineMatch> matches, string replacement)
		{
			_matches = matches;
			_replacement = replacement;
		}
	}


	/// <summary>
	/// Used with changes in the request information
	/// </summary>
	/// <param name="e"></param>
	public delegate void TVDataAccessorDataBatchEvent(TVDataAccessorBatchEventArgs e);
	/// <summary>
	/// Information regarding the requests being modified
	/// </summary>
	public class TVDataAccessorBatchEventArgs : EventArgs
	{
		private IEnumerable<int> _requestList;
		/// <summary>
		/// The list of requests
		/// </summary>
		public IEnumerable<int> RequestList
		{
			get { return _requestList; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requestList"></param>
		public TVDataAccessorBatchEventArgs(IEnumerable<int> requestList)
		{
			_requestList = requestList;
		}
	}


    /// <summary>
    /// Used to signal that a trap event occured
    /// </summary>
    /// <param name="e"></param>
    public delegate void RequestTrapEvent(RequestTrapEventEventArgs e);
    /// <summary>
    /// Information regarding the request that was trapped
    /// </summary>
    public class RequestTrapEventEventArgs : EventArgs
    {
        private TVRequestInfo _tvReqInfo;
		/// <summary>
		/// Traffic viewer information about request
		/// </summary>
        public TVRequestInfo TvReqInfo
        {
            get { return _tvReqInfo; }
        }

		private HttpRequestInfo _httpReqInfo;
		/// <summary>
		/// HTTP information about request
		/// </summary>
		public HttpRequestInfo HttpReqInfo
		{
			get { return _httpReqInfo; }
		}

		private HttpResponseInfo _httpRespInfo;
		/// <summary>
		/// Response info
		/// </summary>
		public HttpResponseInfo HttpRespInfo
		{
			get { return _httpRespInfo; }
		}

        private ManualResetEvent _reqLock;
		/// <summary>
		/// Lock to ensure thread access control
		/// </summary>
        public ManualResetEvent ReqLock
        {
            get { return _reqLock; }
        }

		/// <summary>
		/// Request trap event arguments
		/// </summary>
		/// <param name="tvReqInfo"></param>
		/// <param name="httpReqInfo"></param>
		/// <param name="httpRespInfo"></param>
		/// <param name="reqLock"></param>
        public RequestTrapEventEventArgs(TVRequestInfo tvReqInfo, HttpRequestInfo httpReqInfo, HttpResponseInfo httpRespInfo, ManualResetEvent reqLock)
        {
            _tvReqInfo = tvReqInfo;
            _reqLock = reqLock;
			_httpReqInfo = httpReqInfo;
			_httpRespInfo = httpRespInfo;
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tvReqInfo"></param>
		/// <param name="httpReqInfo"></param>
		/// <param name="reqLock"></param>
		public RequestTrapEventEventArgs(TVRequestInfo tvReqInfo, HttpRequestInfo httpReqInfo, ManualResetEvent reqLock) : this(tvReqInfo, httpReqInfo, null, reqLock) { }

		/// <summary>
		/// COnstructor
		/// </summary>
		/// <param name="tvReqInfo"></param>
		/// <param name="httpRespInfo"></param>
		/// <param name="reqLock"></param>
		public RequestTrapEventEventArgs(TVRequestInfo tvReqInfo, HttpResponseInfo httpRespInfo, ManualResetEvent reqLock) : this(tvReqInfo, null, httpRespInfo, reqLock) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tvReqInfo"></param>
		/// <param name="reqLock"></param>
		public RequestTrapEventEventArgs(TVRequestInfo tvReqInfo, ManualResetEvent reqLock) : this(tvReqInfo, null, null, reqLock) { }
    }


	/// <summary>
	/// Used with changes in the request information
	/// </summary>
	/// <param name="e"></param>
	public delegate void TVDataAccessorDataEvent(TVDataAccessorDataArgs e);
	/// <summary>
	/// Information regarding the requests being modified
	/// </summary>
	public class TVDataAccessorDataArgs : EventArgs
	{
		private int _requestId;
		/// <summary>
		/// Request id
		/// </summary>
		public int RequestId
		{
			get { return _requestId; }
		}

		private TVRequestInfo _header;
		/// <summary>
		/// Traffic viewer request info
		/// </summary>
		public TVRequestInfo Header
		{
			get { return _header; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="header"></param>
		public TVDataAccessorDataArgs(int requestId, TVRequestInfo header)
		{
			_requestId = requestId;
			_header = header;
		}
	}

	/// <summary>
	/// Used with changes in the state of a data accessor
	/// </summary>
	/// <param name="e"></param>
	public delegate void TVDataAccessorStateHandler(TVDataAccessorStateArgs e);
	/// <summary>
	/// Information regarding the change in state
	/// </summary>
	public class TVDataAccessorStateArgs : EventArgs
	{
		AccessorState _state;

		/// <summary>
		/// State of the data accessor
		/// </summary>
		public AccessorState State
		{
			get { return _state; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="state"></param>
		public TVDataAccessorStateArgs(AccessorState state)
		{
			_state = state;
		}
	}

	/// <summary>
	/// Used to signal that the user executed a search
	/// </summary>
	/// <param name="e"></param>
	public delegate void SearchExecutedEvent(SearchExecutedEventArgs e);
	/// <summary>
	/// Information regarding the search
	/// </summary>
	public class SearchExecutedEventArgs : EventArgs
	{
		private string _searchText;
		/// <summary>
		/// Is it a regex?
		/// </summary>
		public string SearchText
		{
			get { return _searchText; }
		}

		private bool _isRegex;
		/// <summary>
		/// Is it a regex?
		/// </summary>
		public bool IsRegex
		{
			get { return _isRegex; }
			set { _isRegex = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="searchText"></param>
		/// <param name="isRegex"></param>
		public SearchExecutedEventArgs(string searchText, bool isRegex)
		{
			_searchText = searchText;
			_isRegex = isRegex;
		}
	}

	/// <summary>
	/// Used to signal that the user executed a search
	/// </summary>
	/// <param name="e"></param>
	public delegate void SearchIndexChangedEvent(SearchIndexChangedEventArgs e);
	/// <summary>
	/// Information regarding the search
	/// </summary>
	public class SearchIndexChangedEventArgs : EventArgs
	{
		private string _searchText;
		/// <summary>
		/// The search criteria
		/// </summary>
		public string SearchText
		{
			get { return _searchText; }
		}

		private int _requestId;
		/// <summary>
		/// The id of the request that was 
		/// </summary>
		public int RequestId
		{
			get { return _requestId; }
		}

		private string _lineMatch;
		/// <summary>
		/// The line that was matched
		/// </summary>
		public string LineMatch
		{
			get { return _lineMatch; }
		}

		private bool _isRegex;
		/// <summary>
		/// Is it a regex?
		/// </summary>
		public bool IsRegex
		{
			get { return _isRegex; }
			set { _isRegex = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="searchText"></param>
		/// <param name="requestId"></param>
		/// <param name="lineMatch"></param>
		/// <param name="isRegex"></param>
		public SearchIndexChangedEventArgs(string searchText,int requestId, string lineMatch, bool isRegex)
		{
			_searchText = searchText;
			_requestId = requestId;
			_lineMatch = lineMatch;
			_isRegex = isRegex;
		}
	}




}
