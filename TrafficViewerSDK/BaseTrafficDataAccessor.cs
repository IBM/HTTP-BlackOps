using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Search;
using System.IO;
using System.Diagnostics;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Saves the most frequently requested HTTP traffic data into the memory
	/// </summary>
    public class RequestDataCache : CacheManager<ICacheable>
    {
        private static object _lock = new object();
        private int _objectRegisterCounter = 100000;

        public int GetRegisteredObjectId()
        {
            _objectRegisterCounter += 100000;
            return _objectRegisterCounter;
        }

		/// <summary>
		/// 
		/// </summary>
        private static RequestDataCache _instance;


        private RequestDataCache()
        {
        }

        /// <summary>
        /// Returns the singleton
        /// </summary>
        public static RequestDataCache Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new RequestDataCache();
                    }
                }
                return _instance;
            }
        }
    
    }


	/// <summary>
	/// Encapsulates logic common to most traffic accessors
	/// </summary>
	public abstract class BaseTrafficDataAccessor : ITrafficDataAccessor
	{
        /// <summary>
        /// Unique id to hash cached requests
        /// </summary>
        private int _objectId = RequestDataCache.Instance.GetRegisteredObjectId();
       
		/// <summary>
		/// Index used for GetNext() GetPrevious()
		/// </summary>
		protected int _firstIndex = -1;
		/// <summary>
		/// Index used for GetNext() GetPrevious()
		/// </summary>
		protected int _lastIndex = -1;
		/// <summary>
		/// If this flag is set to true requests are saved to the memory buffer. During save for example is not necessary to
		/// cache requests. This is mostly required when the user views a request.
		/// </summary>
		protected bool _cacheEnabled = true;
		/// <summary>
		/// Flag that tells us if a tail is active
		/// </summary>
		protected bool _tailInProgress = false;
		/// <summary>
		/// Collection that stores the tv request info objects
		/// </summary>
		protected SortedDictionary<int, TVRequestInfo> _requestInfos;
		/// <summary>
		/// Lock used to protect against conflicts while accessing
		/// the data
		/// </summary>
		protected readonly object _lockData = new object();

		/// <summary>
		/// Commits a request to the memory cache
		/// </summary>
		/// <param name="reqHeaderId"></param>
		/// <param name="data"></param>
		private void BufferSaveRequest(int reqHeaderId, byte[] data)
		{
			if (!_cacheEnabled)
			{
				return;
			}

			ICacheable entry = RequestDataCache.Instance.GetEntry(_objectId ^ reqHeaderId);
			RequestResponseBytes reqData;
			if (entry == null)
			{
				reqData = new RequestResponseBytes();
				reqData.RawRequest = data;
                RequestDataCache.Instance.Add(_objectId ^ reqHeaderId, new CacheEntry(reqData));
			}
			else
			{
				reqData = entry.Reserve() as RequestResponseBytes;
				reqData.RawRequest = data;
				entry.Release();
			}

		}


        /// <summary>
        /// Changes the current state
        /// </summary>
        /// <param name="accessorState"></param>
        public void SetState(AccessorState accessorState)
        {
            _state = accessorState;
            //trigger event
            if (StateChanged != null)
            {
                StateChanged.Invoke(new TVDataAccessorStateArgs(accessorState));
            }

        }


        /// <summary>
        /// Clears request list
        /// </summary>
        public bool Clear()
        {
            List<int> reqIds = new List<int>();
            reqIds.AddRange(_requestInfos.Keys);
            return RemoveRequestBatch(reqIds);
        }

		/// <summary>
		/// Commits a response to the memory cache
		/// </summary>
		/// <param name="reqHeaderId"></param>
		/// <param name="data"></param>
		private void BufferSaveResponse(int reqHeaderId, byte[] data)
		{
            ICacheable entry = RequestDataCache.Instance.GetEntry(_objectId ^ reqHeaderId);

			if (entry != null)
			{
				RequestResponseBytes reqData = entry.Reserve() as RequestResponseBytes;
				reqData.RawResponse = data;
				entry.Release();
			}
		}

		/// <summary>
		/// Reads data from the data source
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		protected abstract byte[] DataRead(long startPosition, int length);

		/// <summary>
		/// Writes a chunk of data to the data source
		/// </summary>
		/// <param name="buffer">The bytes to write</param>
		/// <param name="offset">The offset in the buffer from which to start writing</param>
		/// <param name="length">How many bytes should be written</param>
		protected abstract void DataWrite(byte[] buffer, int offset, int length);

		/// <summary>
		/// Gets the current write position
		/// </summary>
		protected abstract long WritePosition
		{
			get;
		}

		/// <summary>
		/// Invokes the data cleared event
		/// </summary>
		protected void InvokeDataCleared()
		{
			if (DataCleared != null)
			{
				DataCleared.Invoke(null);
			}
		}

		
		#region Accessor Methods

		private ParsingOptions _profile = ParsingOptions.GetLegacyAppScanProfile();
		/// <summary>
		/// Stores information about the parsing profile that
		/// was used to import the current data
		/// </summary>
		public ParsingOptions Profile
		{
			get
			{
				return _profile;
			}
			set
			{
				_profile = value;
			}
		}

		private AccessorState _state;
		/// <summary>
		/// Gets the state of the file
		/// </summary>
		public AccessorState State
		{
			get
			{
				return _state;
			}
		}

		/// <summary>
		/// Retrieves the number of requests
		/// </summary>
		public int RequestCount
		{
			get
			{
				return _requestInfos.Count;
			}
		}
		
		/// <summary>
		/// Occurs when a new request entry was added to the list
		/// </summary>
		public event TVDataAccessorDataEvent RequestEntryAdded;

        /// <summary>
        /// Occurs when a request was updated
        /// </summary>
        public event TVDataAccessorDataEvent RequestEntryUpdated;

		/// <summary>
		/// Occurs when the request was fully added to the file. 
		/// Is not guaranteed that this event will be triggered in order.
		/// Use RequestEntryAdded to ensure requests are processed in the proper order
		/// </summary>
		public event TVDataAccessorDataEvent RequestChanged;

		/// <summary>
		/// Occurs when a response was completed
		/// </summary>
		public event TVDataAccessorDataEvent ResponseChanged;

		/// <summary>
		/// Occurs when a request is removed from the list
		/// </summary>
		public event TVDataAccessorDataEvent RequestEntryRemoved;

		/// <summary>
		/// Occurs when a batch of requests is removed from the list
		/// </summary>
		public event TVDataAccessorDataBatchEvent RequestBatchRemoved;

		/// <summary>
		/// Occurs when all the file data was cleared
		/// </summary>
		public event TVDataAccessorDataEvent DataCleared;

		/// <summary>
		/// Occurs when the TrafficViewer File changes its state
		/// </summary>
		public event TVDataAccessorStateHandler StateChanged;

		/// <summary>
		/// Occurs when the accessor executes a replace operation
		/// </summary>
		public event ReplaceEvent ReplaceEvent;

		/// <summary>
		/// Can be used to iterate through the request list
		/// </summary>
		/// <param name="currIndex">The current index in the list. If -1 the first element is returned</param>
		/// <returns>RequestHeader</returns>
		public TVRequestInfo GetNext(ref int currIndex)
		{
			if (_requestInfos == null)
			{
				return null;
			}

			do
			{
				currIndex++;
			}
			while (!_requestInfos.ContainsKey(currIndex)
				&& currIndex <= _lastIndex);

			if (currIndex > _lastIndex)
			{
				return null;
			}

			return _requestInfos[currIndex];
		}

		/// <summary>
		/// Can be used to iterate through the request list
		/// </summary>
		/// <param name="currIndex">The current index in the list. If -1 the last element is returned</param>
		/// <returns>RequestHeader</returns>
		public TVRequestInfo GetPrevious(ref int currIndex)
		{
			if (_requestInfos == null)
			{
				return null;
			}

			do
			{
				currIndex--;
			}
			while (!_requestInfos.ContainsKey(currIndex)
				&& currIndex >= _firstIndex && currIndex > -1);

			if (_firstIndex<=-1 || currIndex < _firstIndex)
			{
				return null;
			}

			return _requestInfos[currIndex];
		}

		/// <summary>
		/// Gets the request header object for the specified id
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		public TVRequestInfo GetRequestInfo(int requestHeaderId)
		{
            lock (_lockData)
            {
                if (_requestInfos.ContainsKey(requestHeaderId))
                {
                    return _requestInfos[requestHeaderId];
                }
            }
			return null;
		}

		/// <summary>
		/// Adds a new request info row to the list
		/// </summary>
		/// <param name="header"></param>
		/// <returns>The index of the new entry</returns>
		public int AddRequestInfo(TVRequestInfo header)
		{
			int result = -1;

			try
			{
				lock (_lockData) //critical section begins
				{
                    if (header.Id == -1)
                    {
                        if (_firstIndex == -1)
                        {
                            _firstIndex = 0;
                            _lastIndex = 0;
                        }
                        else
                        {
                            _lastIndex++;
                        }

                        //save the index in the request header
                        header.Id = _lastIndex;
                    }
                    else
                    {
						if (_firstIndex == -1)
						{
							_firstIndex = header.Id;
						}
                        _lastIndex = header.Id;
                    }
					_requestInfos.Add(_lastIndex, header);
					//new request header was added to the list
					//this means that we have at least the request line
					//call the RequestAdded event

					result = _lastIndex;
				} //end critical section

				//now that the data was added invoke the event
				if (RequestEntryAdded != null)
				{
					TVDataAccessorDataArgs e = new TVDataAccessorDataArgs(_lastIndex, header);
					RequestEntryAdded.Invoke(e);
				}

			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "An error occured while adding a request info: {0}", ex.ToString());
			}

			return result;
		}


        /// <summary>
		/// Replaces the index at header.id with the passed object and signals that the request info was updated to all listeners
		/// </summary>
		/// <param name="header"></param>
		/// <returns>The index of the new entry</returns>
        public void UpdateRequestInfo(TVRequestInfo header)
        {
            lock(_lockData)
            {
                _requestInfos[header.Id] = header;
            }
            
            if (RequestEntryUpdated != null)
            {
                TVDataAccessorDataArgs e = new TVDataAccessorDataArgs(header.Id, header);
                RequestEntryUpdated.Invoke(e);
            }
        }

		/// <summary>
		/// Removes the specified header from the list
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		public bool RemoveRequest(int requestHeaderId)
		{
			bool returnValue = false;

			SetState(AccessorState.RemovingEntries);
			try
			{
				lock (_lockData)//critical section begins
				{
					_requestInfos.Remove(requestHeaderId);
					if (_requestInfos.Count == 0)
					{
						_firstIndex = _lastIndex = -1;
					}
					else if (requestHeaderId == _firstIndex)
					{
						while (_firstIndex < _lastIndex && !_requestInfos.ContainsKey(_firstIndex))
						{
							_firstIndex++;
						}
					}
					else if (requestHeaderId == _lastIndex)
					{
						while (_lastIndex > _firstIndex && !_requestInfos.ContainsKey(_lastIndex))
						{
							_lastIndex--;
						}
					}
				} //end critical section
				
				//trigger the entry removed event
				if (RequestEntryRemoved != null)
				{
					RequestEntryRemoved.Invoke(new TVDataAccessorDataArgs(requestHeaderId, null));
				}
				returnValue = true;
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "An error occured while removing a request info: {0}", ex.ToString());
			}
			SetState(AccessorState.Idle);

			return returnValue;
		}

		/// <summary>
		/// Removes the specified batch of requests from the list
		/// </summary>
		/// <param name="requestList"></param>
		/// <returns></returns>
		public bool RemoveRequestBatch(IEnumerable<int> requestList)
		{
			bool returnValue = false;
			SetState(AccessorState.RemovingEntries);
			try
			{
				lock (_lockData) //critical section begins
				{
					foreach (int requestId in requestList)
					{
						_requestInfos.Remove(requestId);
						if (_requestInfos.Count == 0)
						{
							_firstIndex = _lastIndex = -1;
						}
						else if (requestId == _firstIndex)
						{
							while (_firstIndex < _lastIndex && !_requestInfos.ContainsKey(_firstIndex))
							{
								_firstIndex++;
							}
						}
						else if (requestId == _lastIndex)
						{
							while (_lastIndex > _firstIndex && !_requestInfos.ContainsKey(_lastIndex))
							{
								_lastIndex--;
							}
						}
					}
				}//end critical section

				//trigger the entry removed event
				if (RequestBatchRemoved != null)
				{
					RequestBatchRemoved.Invoke(new TVDataAccessorBatchEventArgs(requestList));
				}
				returnValue = true;
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "An error occured while removing a request info: {0}", ex.ToString());
			}

			SetState(AccessorState.Idle);

			return returnValue;
		}


		/// <summary>
		/// Retrieves the request bytes from disk or from the memory cache
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		public byte[] LoadRequestData(int requestHeaderId)
		{
			byte[] result = new byte[0];
			try
			{
				lock (_lockData) //critical section begins
				{
                    TVRequestInfo reqInfo;
                    if (_requestInfos.TryGetValue(requestHeaderId, out reqInfo) && reqInfo.RequestLength > 0)
                    {
                        //check if the request is already in the buffer
                        ICacheable entry = RequestDataCache.Instance.GetEntry(_objectId ^ requestHeaderId);
                        RequestResponseBytes reqData = null;
                        if (entry != null)
                        {
                            reqData = entry.Reserve() as RequestResponseBytes;
                            entry.Release();
                        }

                        if (reqData != null && reqData.RawRequest != null)
                        {
                            result = reqData.RawRequest;
                        }
                        else
                        {

                            //load request from disk
                            int length = reqInfo.RequestLength;
                            long startPosition = reqInfo.RequestStartPosition;
                            result = DataRead(startPosition, length);

                            //save request to buffer if is not null
                            if (result.Length != 0)
                            {
                                BufferSaveRequest(requestHeaderId, result);
                            }
                        }

                        if (reqInfo.IsEncrypted && result!=null && result.Length > 0)
                        { 
                            //decrypt the request
                            result = Encryptor.Decrypt(result);
                        }
                    }

				} //critical section ends
			}
			catch (Exception ex)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot load request data for request id: {0} . Stack trace: {1}", requestHeaderId, ex.ToString());
			}

			return result;

		}

		/// <summary>
		/// Retrieves the response bytes from disk or from the memory cache
		/// </summary>
		/// <param name="requestHeaderId"></param>
		/// <returns></returns>
		public byte[] LoadResponseData(int requestHeaderId)
		{
			byte[] result = new byte[0];
			try
			{
				lock (_lockData) //start critical section
				{
					//check if the request is already in the buffer
                    TVRequestInfo reqInfo;
                    if (_requestInfos.TryGetValue(requestHeaderId, out reqInfo) && reqInfo.ResponseLength > 0)
                    {

                        ICacheable entry = RequestDataCache.Instance.GetEntry(_objectId ^ requestHeaderId);
                        RequestResponseBytes reqData = null;

                        if (entry != null)
                        {
                            reqData = entry.Reserve() as RequestResponseBytes;
                            entry.Release();
                        }

                        if (reqData != null && reqData.RawResponse != null)
                        {
                            result = reqData.RawResponse;
                        }
                        else
                        {

                            //load response from disk
                            int length = reqInfo.ResponseLength;
                            long startPosition = reqInfo.ResponseStartPosition;
                            result = DataRead(startPosition, length);

                            BufferSaveResponse(requestHeaderId, result);

                        }

                        if (reqInfo.IsEncrypted && result != null && result.Length > 0)
                        {
                            //decrypt the request
                            result = Encryptor.Decrypt(result);
                        }
                    }

				} //critical sectrion ends
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot load response data for request id: {0} . Stack trace: {1}", requestHeaderId, ex.ToString());
			}
			return result;
		}

		/// <summary>
		/// Saves request bytes to disk and caches it if tail is on
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data">Request data bytes</param>
		public void SaveRequest(int requestId, RequestResponseBytes data)
		{
			if (data == null || data.RawRequest == null)
			{
				return;
			}

			bool isRequestChanged = false;

			try
			{

				lock (_lockData) //critical section begins
				{

					if (_requestInfos.ContainsKey(requestId))
					{

						//save to memory buffer only if tail is on
						//this is done to use minimum memory footprint since on 
						//normal load the user is viewing the requests
						//at the beginning of the file
						TVRequestInfo reqInfo = _requestInfos[requestId];
						//save to disk
						reqInfo.RequestStartPosition = WritePosition;
                        string reqLine = null;
                        if (_tailInProgress || RequestDataCache.Instance.GetEntry(_objectId ^ requestId) != null || reqInfo.IsEncrypted)
						{
                            byte[] rawRequest = data.RawRequest;
                            reqLine = HttpRequestInfo.GetRequestLine(rawRequest);
							//this takes memory but at the same time insures that the user can see the data fast during tail
                            if (reqInfo.IsEncrypted)
                            {
                                rawRequest = Encryptor.Encrypt(rawRequest);
                            }
                            reqInfo.RequestLength = rawRequest.Length;
                            BufferSaveRequest(requestId, rawRequest);
                            DataWrite(rawRequest, 0, rawRequest.Length);
						}
						else
						{
							byte[] chunk;
							//this saves memory and writes the chunks of data directly to disk
							data.ResetRequestChunkPosition();
							while ((chunk = data.ReadRequestChunk()) != null)
							{
                                if (reqLine == null)
                                {
                                    reqLine = HttpRequestInfo.GetRequestLine(chunk);
                                }
                                DataWrite(chunk, 0, chunk.Length);
							}
                            reqInfo.RequestLength = data.RequestSize;
						}
                        reqInfo.RequestLine = reqLine;
						isRequestChanged = true;
					}
				}//critical section ends

				//Invoke event
				if (isRequestChanged && RequestChanged != null)
				{
					RequestChanged.Invoke(
						new TVDataAccessorDataArgs(requestId, _requestInfos[requestId]));
				}
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error saving request data for request id: {0} . Stack trace: {1}", requestId, ex.ToString());
			}
		}

		/// <summary>
		/// Saves response bytes to disk and caches it if tail is on
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data">Request data object storing the response</param>
		public void SaveResponse(int requestId, RequestResponseBytes data)
		{
			if (data == null || data.RawResponse == null)
			{
				return;
			}

			bool isResponseChanged = false;
			TVRequestInfo reqInfo = null;

			try
			{
				lock (_lockData) //critical section begins
				{
					if (_requestInfos.ContainsKey(requestId))
					{

						//save to memory buffer only if tail is on
						//this is done to use minimum memory footprint since on 
						//normal load the user is viewing the requests
						//at the beginning of the file
						reqInfo = _requestInfos[requestId];
						//save to disk
						reqInfo.ResponseStartPosition = WritePosition;
						
                        string respStatus = null;

                        if (_tailInProgress || RequestDataCache.Instance.GetEntry(_objectId ^ requestId) != null || reqInfo.IsEncrypted)
						{
                            byte[] rawResponse = data.RawResponse;
                            respStatus = HttpResponseInfo.GetResponseStatus(rawResponse);
							//this takes memory but at the same time insures that the user can see the data fast during tail

                            if (reqInfo.IsEncrypted)
                            {
                                rawResponse = Encryptor.Encrypt(rawResponse);
                            }

							BufferSaveResponse(requestId, rawResponse);
                            DataWrite(rawResponse, 0, rawResponse.Length);
                            reqInfo.ResponseLength = rawResponse.Length;
						}
						else
						{
							byte[] chunk;
							//this saves memory and writes the chunks of data directly to disk
							data.ResetResponseChunkPosition();
							while ((chunk = data.ReadResponseChunk()) != null)
							{
                                if (respStatus == null)
								{
                                    respStatus = HttpResponseInfo.GetResponseStatus(chunk);
								}
								DataWrite(chunk, 0, chunk.Length);
							}
                            reqInfo.ResponseLength = data.ResponseSize;
						}

                        reqInfo.ResponseStatus = respStatus;
						isResponseChanged = true;
					}
				}//critical section ends
				
				if (isResponseChanged && ResponseChanged != null)
				{
					ResponseChanged.Invoke(
						new TVDataAccessorDataArgs(requestId, reqInfo));
				}
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error saving response data for request id: {0} . Stack trace: {1}", requestId, ex.ToString());
			}

		}


		/// <summary>
		/// Saves the raw request to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		public void SaveRequest(int requestId, byte[] data)
		{
			RequestResponseBytes reqData = new RequestResponseBytes();
			reqData.AddToRequest(data);
			SaveRequest(requestId, reqData);
		}


		/// <summary>
		/// Saves the raw response to disk
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="data"></param>
		public void SaveResponse(int requestId, byte[] data)
		{
			RequestResponseBytes reqData = new RequestResponseBytes();
			reqData.AddToResponse(data);
			SaveResponse(requestId, reqData);
		}



		/// <summary>
		/// Checks if the data accessor contains the id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsId(int id)
		{
			return _requestInfos.ContainsKey(id);
		}

		/// <summary>
		/// Save the raw request with the specified response
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="request"></param>
		/// <param name="response"></param>
		public void SaveRequestResponse(int requestId, byte[] request, byte[] response)
		{
			RequestResponseBytes data = new RequestResponseBytes();
			data.AddToRequest(request);
			data.AddToResponse(response);

			SaveRequest(requestId, data);
			SaveResponse(requestId, data);

		}

		/// <summary>
		/// Save the raw request with the specified response
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="request"></param>
		/// <param name="response"></param>
		public void SaveRequestResponse(int requestId, string request, string response)
		{
			byte[] requestBytes = Constants.DefaultEncoding.GetBytes(request);
			byte[] responseBytes = Constants.DefaultEncoding.GetBytes(response);
			SaveRequestResponse(requestId, requestBytes, responseBytes);

		}


		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns>The request id</returns>
		public int AddRequestResponse(string request, string response)
		{

			return AddRequestResponse(request, response, false);
		}

		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="isHttps"></param>
		/// <returns>The request id</returns>
		public int AddRequestResponse(string request, string response, bool isHttps)
		{
            byte[] reqBytes = Constants.DefaultEncoding.GetBytes(request);
            byte[] respBytes = Constants.DefaultEncoding.GetBytes(response);
			return AddRequestResponse(reqBytes, respBytes, isHttps);
		}

		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public int AddRequestResponse(byte[] request, byte[] response)
		{
			return AddRequestResponse(request,response,true);
		}

		/// <summary>
		/// Appends the specified request with the associated raw response to the current traffic file and returns the request id
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="isHttps"></param>
		/// <returns></returns>
		public int AddRequestResponse(byte[] request, byte[] response, bool isHttps)
		{
			TVRequestInfo tvReqInfo = new TVRequestInfo();

			tvReqInfo.RequestLine = HttpRequestInfo.GetRequestLine(request);
			tvReqInfo.Description = "N/A";
			tvReqInfo.RequestTime = DateTime.Now;
			tvReqInfo.ResponseTime = DateTime.Now;
			tvReqInfo.ThreadId = Utils.GetCurrentWin32ThreadId().ToString();
			tvReqInfo.IsHttps = isHttps;
			tvReqInfo.ResponseStatus = HttpResponseInfo.GetResponseStatus(response);

			AddRequestInfo(tvReqInfo);

			SaveRequestResponse(tvReqInfo.Id, request, response);

			return tvReqInfo.Id;

		}

		/// <summary>
		/// Replaces the specified set of matches with the replacement string
		/// </summary>
		/// <param name="matchList"></param>
		/// <param name="replacement"></param>
		public void Replace(IList<LineMatch> matchList, string replacement)
		{
			int count = matchList.Count;
			for (int i = count - 1; i > -1; i--)
			{
				LineMatch match = matchList[i];
				if (match.Context == SearchContext.Request)
				{
					byte[] reqBytes = LoadRequestData(match.RequestId);
					reqBytes = ReplaceInComponent(reqBytes, match, replacement);
					SaveRequest(match.RequestId, reqBytes);
					//update request line
					TVRequestInfo reqInfo = GetRequestInfo(match.RequestId);
					if(String.Compare(match.Line,reqInfo.RequestLine) == 0)
					{
						HttpRequestInfo httpReqInfo = new HttpRequestInfo(reqBytes);
						reqInfo.RequestLine = httpReqInfo.RequestLine;
					}
				}
				else if (match.Context == SearchContext.Response)
				{
					byte[] respBytes = LoadResponseData(match.RequestId);
					respBytes = ReplaceInComponent(respBytes, match, replacement);
					SaveResponse(match.RequestId, respBytes);
				}
			}

			if (ReplaceEvent != null)
			{
				ReplaceEvent.Invoke(new ReplaceEventArgs(matchList, replacement));
			}

		}

		/// <summary>
		/// Utility function containing the replace logic for both requests and responses
		/// </summary>
		/// <param name="originalValueBytes"></param>
		/// <param name="match"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		private byte[] ReplaceInComponent(byte[] originalValueBytes, LineMatch match, string replacement)
		{
			string val = Constants.DefaultEncoding.GetString(originalValueBytes);
			int count = match.MatchCoordinatesList.Count;
			for (int i = count - 1; i > -1; i--)
			{
				MatchCoordinates coords = new MatchCoordinates(-1, -1);
				try
				{
					 coords = match.MatchCoordinatesList[i];

					val = String.Format("{0}{1}{2}",
						val.Substring(0, coords.MatchPosition),
						replacement,
						val.Substring(coords.MatchPosition + coords.MatchLength));
				}
				catch (Exception ex)
				{
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Incorrect match coordinates for request id: '{0}', line match '{1}', context: '{2}', start: '{3}', length: '{4}', exception: '{5}'",
						LogMessageType.Error,
						match.RequestId,
						match.Line,
						match.Context,
						coords.MatchPosition,
						coords.MatchLength,
						ex.Message);
				}

			}

			return Constants.DefaultEncoding.GetBytes(val);
		}



		#endregion
	}
}
