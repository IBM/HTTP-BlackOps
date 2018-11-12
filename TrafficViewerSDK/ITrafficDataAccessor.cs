using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Search;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Provides a common interface for traffic log data structures
	/// </summary>
	public interface ITrafficDataAccessor
	{

		#region Properties
		/// <summary>
		/// Contains parsing settings, highlighting options
		/// </summary>
		ParsingOptions Profile
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the state of the Accessor
		/// </summary>
		AccessorState State
		{
			get;
		}

		/// <summary>
		/// Returns the current number of requests
		/// </summary>
		int RequestCount
		{
			get;
		}

		#endregion

		#region Events

		/// <summary>
		/// Triggered when a new request header was added, passes RequestListEventArgs 
		/// </summary>
		event TVDataAccessorDataEvent RequestEntryAdded;

        /// <summary>
        /// Triggered when a request header was updated
        /// </summary>
        event TVDataAccessorDataEvent RequestEntryUpdated;

		/// <summary>
		/// Triggered when a new request was added, passes RequestListEventArgs 
		/// </summary>		
		event TVDataAccessorDataEvent RequestChanged;

		/// <summary>
		/// Triggered when a new response was added, passes RequestListEventArgs 
		/// </summary>		
		event TVDataAccessorDataEvent ResponseChanged;

		/// <summary>
		/// Occurs when all the requests in the data structure have been discarded
		/// </summary>
		event TVDataAccessorDataEvent DataCleared;

		/// <summary>
		///  Occurs when a request was removed,, passes RequestListEventArgs 
		/// </summary>
		event TVDataAccessorDataEvent RequestEntryRemoved;

		/// <summary>
		/// Occurs when a batch of requests is removed from the list
		/// </summary>
		event TVDataAccessorDataBatchEvent RequestBatchRemoved;

		/// <summary>
		///  Occurs when the accessor changes its state
		/// </summary>
		event TVDataAccessorStateHandler StateChanged;

		/// <summary>
		/// Occurs when the accessor executes a replace operation
		/// </summary>
		event ReplaceEvent ReplaceEvent;

		#endregion

		#region Methods

        /// <summary>
        /// Sets the state of the accessor
        /// </summary>
        /// <param name="accessorState"></param>
        void SetState(AccessorState accessorState);

		/// <summary>
		/// Enumerates through the collection of request headers
		/// </summary>
		/// <param name="currIndex">The index of the last returned header in the list. 
		/// Set index to -1 to get the first element</param>
		/// <returns>The current header or null</returns>
		TVRequestInfo GetNext(ref int currIndex);



		/// <summary>
		/// Enumerates through the collection of request headers
		/// </summary>
		/// <param name="currIndex">The index of the last returned header in the list. 
		/// Set index to -1 to get the last element</param>
		/// <returns>The current header or null</returns>
		TVRequestInfo GetPrevious(ref int currIndex);

		/// <summary>
		/// Returns a specific request header
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		TVRequestInfo GetRequestInfo(int requestHeaderId);

		/// <summary>
		/// Adds a request header
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		int AddRequestInfo(TVRequestInfo info);

        /// <summary>
        /// Updates a request header
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        void UpdateRequestInfo(TVRequestInfo info);

		/// <summary>
		/// Removes a request header
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		bool RemoveRequest(int requestHeaderId);

		/// <summary>
		/// Removes the specified batch of requests from the list
		/// </summary>
		/// <param name="requestList"></param>
		/// <returns></returns>
		bool RemoveRequestBatch(IEnumerable<int> requestList);

        /// <summary>
        /// Removes the specified batch of requests from the list
        /// </summary>
        /// <param name="requestList"></param>
        /// <returns></returns>
        bool Clear();

		/// <summary>
		/// Loads request data from the disk for the specified header id
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns>Request in byte[] format</returns>
		byte[] LoadRequestData(int requestHeaderId);

		/// <summary>
		/// Loads response data from the disk for the specified header id
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns>Response in byte[] format</returns>
		byte[] LoadResponseData(int requestHeaderId);

		/// <summary>
		/// Saves the raw request to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		void SaveRequest(int requestId, RequestResponseBytes data);

		/// <summary>
		/// Saves the raw response to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		void SaveResponse(int requestId, byte[] data);


		/// <summary>
		/// Saves the raw request to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		void SaveRequest(int requestId, byte[] data);

		/// <summary>
		/// Saves the raw response to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		void SaveResponse(int requestId, RequestResponseBytes data);

		/// <summary>
		/// Save the raw request with the specified response
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="request"></param>
		/// <param name="response"></param>
		void SaveRequestResponse(int requestId,string request, string response);


		/// <summary>
		/// Save the raw request with the specified response
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="request"></param>
		/// <param name="response"></param>
		void SaveRequestResponse(int requestId, byte[] request, byte[] response);

		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		int AddRequestResponse(string request, string response);

		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		int AddRequestResponse(byte [] request, byte [] response);


		/// <summary>
		/// Replaces the specified set of matches with the replacement string
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="replacement"></param>
		void Replace(IList<LineMatch> matches, string replacement);


		/// <summary>
		/// Checks if the specified id exists
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool ContainsId(int id);

		#endregion

	}
}
