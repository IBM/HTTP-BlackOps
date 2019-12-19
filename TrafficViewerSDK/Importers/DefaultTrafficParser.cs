using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Properties;
using System.Diagnostics;
using TrafficViewerSDK.Http;


namespace TrafficViewerSDK.Importers
{
	/// <summary>
	/// Parses a traffic log into traffic file format
	/// </summary>
	public class DefaultTrafficParser : ITrafficParser
	{

		#region Options

		private string _proxyConnectionToSiteRegex;
		private string _threadIdRegex;
		private string _descriptionRegex;
		private string _timeFormat;
		private string _timeRegex;

		private int ESTIMATED_LINE_SIZE = 1024;
		private IEnumerable<string> _exclusions;
		private Dictionary<string, string> _customFieldDefinitions;
		private string _responseStatusRegex;
		private Regex _sendingRequestRegex;
		private Regex _receivingResponseRegex;


		#endregion

		#region Fields

		private const string HOST_HEADER = "Host:";

		private Dictionary<string, ThreadInfo> _threads = new Dictionary<string, ThreadInfo>();

		private Dictionary<string, string> _importTypes = new Dictionary<string, string>();

		/// <summary>
		/// File to be parsed
		/// </summary>
		private FileStream _rawLog;

		/// <summary>
		/// The target Traffic Viewer File
		/// </summary>
		private ITrafficDataAccessor _trafficViewerFile;

		/// <summary>
		/// used to detect if the user requested a stop
		/// </summary>
		private bool _stopRequested = false;

		/// <summary>
		/// stores the current time, set by the last line of thread information
		/// </summary>
		private string _currentTime;

		/// <summary>
		/// the index of the current requestHeader
		/// </summary>
		private int _currentIndex = -1;

		/// <summary>
		/// the current request header
		/// </summary>
		private TVRequestInfo _currentHeader = null;

		/// <summary>
		/// will store raw Http Traffic
		/// </summary>
		private RequestResponseBytes _currentRequestData = new RequestResponseBytes();

		/// <summary>
		/// Will store the current thread information
		/// </summary>
		ThreadInfo _currentThreadInfo;

		/// <summary>
		/// byte array used at concatenation
		/// </summary>
		private byte[] _newLineBytes = Constants.DefaultEncoding.GetBytes(Environment.NewLine);

		/// <summary>
		/// Specifies if a new line should be written, new thread lines introduce artificial line breaks
		/// </summary>
		private bool _isNewThreadChunk = true;

		/// <summary>
		/// Contains culture information for time conversion
		/// </summary>
		private System.Globalization.DateTimeFormatInfo _dateTimeFormatInfo =
				new System.Globalization.DateTimeFormatInfo();

		/// <summary>
		/// Is used in deciding the types of lines in the Traffic Log
		/// </summary>
		private LineTypeSelector _lineTypeSelector;

		/// <summary>
		/// Controls how many requests should be loaded at a time
		/// </summary>
		private int _tailChunk = -1;

		/// <summary>
		/// How many requests including their responses were found during this loading session
		/// </summary>
		private int _thisSessionRequestCount = 0;

		#endregion

		#region Properties

		private TrafficParserStatus _parserStatus = TrafficParserStatus.Stopped;
		/// <summary>
		/// The current status of the parser
		/// </summary>
		public TrafficParserStatus ParserStatus
		{
			get
			{
				return _parserStatus;
			}
		}

		private ParsingOptions _parsingOptions;
		/// <summary>
		/// Gets the options used by this parser
		/// </summary>
		public ParsingOptions Options
		{
			get
			{
				return _parsingOptions;
			}
		}

		#endregion

		#region Private Methods

		private bool IsExcluded(string requestLine)
		{
			foreach (string e in _exclusions)
			{
				if (Utils.IsMatch(requestLine, e))
				{
					return true;
				}
			}
			return false;
		}

		private byte[] ConcatBytes(byte[] first, byte[] second, bool addNewLine)
		{
			int length = first.Length + second.Length;
			int index = 0;
			if (addNewLine)
			{
				length += _newLineBytes.Length;
			}
			byte[] result = new byte[length];
			first.CopyTo(result, index);
			index = first.Length;
			if (addNewLine)
			{
				_newLineBytes.CopyTo(result, index);
				index += _newLineBytes.Length;
			}
			second.CopyTo(result, index);
			return result;
		}

		#region LINE TYPE HANDLERS CODE

		private void HandleHalt()
		{
			_parserStatus = TrafficParserStatus.Stopped;
			if (_threads.Count > 0)
			{
				//save all requests that are in process without removing them from their stack, so they can be resumed later
				foreach (ThreadInfo threadInfo in _threads.Values)
				{
					SaveThreadRequests(threadInfo);
				}
			}
			else if (_currentThreadInfo != null)
			{
				SaveThreadRequests(_currentThreadInfo);
			}
		}

		private void SaveThreadRequests(ThreadInfo threadInfo)
		{
			foreach (KeyValuePair<int, RequestResponseBytes> request in threadInfo.CurrentRequests)
			{
				TVRequestInfo header = _trafficViewerFile.GetRequestInfo(request.Key);
				if (header != null)
				{
					if (request.Value.RequestSize > header.RequestLength)
					{
						_trafficViewerFile.SaveRequest(request.Key, request.Value);
					}

					if (request.Value.ResponseSize > header.ResponseLength)
					{
						_trafficViewerFile.SaveResponse(request.Key, request.Value);
					}

					_thisSessionRequestCount++;
				}
			}
		}

		private void HandleBeginThread(string line)
		{
			//save current request
			if (_currentRequestData.RawRequest != null)
			{
				if (_currentHeader != null && _currentHeader.RequestLength < _currentRequestData.RequestSize)
				{
					_trafficViewerFile.SaveRequest(_currentIndex, _currentRequestData);
				}
			}

			_isNewThreadChunk = true; //the traffic might have been broken by the new line

			//extract a new thread id
			string currentThreadId = Utils.RegexFirstGroupValue(line, _threadIdRegex);
			string temp = Utils.RegexFirstGroupValue(line, _timeRegex);
            if (temp != String.Empty)
            {
                _currentTime = temp;
            }
			if (currentThreadId != String.Empty && currentThreadId != _currentThreadInfo.ThreadId)
			{

				//check if the thread exists if not create a new one
				if (_threads.ContainsKey(currentThreadId))
				{
					_currentThreadInfo = _threads[currentThreadId];
					if (_currentThreadInfo.CurrentRequests.Count > 0)
					{
						KeyValuePair<int, RequestResponseBytes> top = _currentThreadInfo.CurrentRequests.Peek();
						_currentIndex = top.Key;
						_currentHeader = _trafficViewerFile.GetRequestInfo(_currentIndex);
						_currentRequestData = top.Value;
					}
					else
					{
						_currentIndex = -1;
						_currentHeader = null;
						_currentRequestData = new RequestResponseBytes();
					}
				}
				else
				{
					ThreadInfo newInfo = new ThreadInfo();
					newInfo.ThreadId = currentThreadId;
					_threads.Add(currentThreadId, newInfo);
					_currentThreadInfo = newInfo;
					_currentRequestData = new RequestResponseBytes();
				}

			}

			//if a begin line occured resume the _current thread
			_currentThreadInfo.Suspended = false;

			string description = Utils.RegexFirstGroupValue(line, _descriptionRegex);
			if (description != String.Empty)
			{
				_currentThreadInfo.Description = description;
			}
		}

		private void HandleFirstRequestLine(string line, byte[] lineBytes)
		{
			_currentThreadInfo.Location = LocationInThread.InsideRequest;
			//if a current request already exists in the thread this is end of that request
			//except in the case of AppScan standard where manual explore requests are doubled
			if (!Utils.IsMatch(_currentThreadInfo.Description, _proxyConnectionToSiteRegex)
				&& _currentThreadInfo.CurrentRequests.Count > 0)
			{
				KeyValuePair<int, RequestResponseBytes> top = _currentThreadInfo.CurrentRequests.Pop();
				TVRequestInfo header = _trafficViewerFile.GetRequestInfo(top.Key);
				if (header != null)
				{
					if (top.Value.RawResponse == null)//there is no response, time out
					{
						if (top.Value.RequestSize > header.RequestLength)
						{
							_trafficViewerFile.SaveRequest(top.Key, top.Value);
						}
					}
					else
					{
						//the request was already saved save the response if necessary
						if (top.Value.ResponseSize > header.ResponseLength)
						{
							_trafficViewerFile.SaveResponse(top.Key, top.Value);
						}
					}
					//this concludes the current request update the requests counter
					_thisSessionRequestCount++;
				}
			}
			//discard exclusions
			if (IsExcluded(line))
			{
				_currentThreadInfo.Location = LocationInThread.Exclusion;
				return;
			}
			//create new request header
			_currentHeader = new TVRequestInfo();
			_currentHeader.Description = _currentThreadInfo.Description;
			_currentHeader.ThreadId = _currentThreadInfo.ThreadId;
			_currentHeader.RequestLine = line;
            if (_currentTime != null)
            {
                try
                {
                    _currentHeader.RequestTime = DateTime.ParseExact(_currentTime, _timeFormat, _dateTimeFormatInfo);
                }
                catch { }
            }
            else
            {
                _currentHeader.RequestTime = DateTime.Now;
            }
			//save newly created request header to the list of headers
			_currentIndex = _trafficViewerFile.AddRequestInfo(_currentHeader);
			if (lineBytes != null) //if lineBytes is null we are using the _currentRequestData
			{
				//create new requestdata
				_currentRequestData = new RequestResponseBytes();
				_currentRequestData.AddToRequest(lineBytes);
			}
			_isNewThreadChunk = false;
			//push request header and request data to the stack of active requests for the current thread
			_currentThreadInfo.CurrentRequests.Push(new KeyValuePair<int, RequestResponseBytes>(_currentIndex, _currentRequestData));
			//since a request line was encountered we are inside a request from now on
		}

		private void HandleFirstResponseLine(string line, byte[] lineBytes)
		{
			//if you are inside an exclusion discard
			if (_currentThreadInfo.Location == LocationInThread.Exclusion)
			{
				return;
			}
			//if you never had a request before discard
			if (_currentIndex < 0)
			{
				return;
			}

			//save current request
			if (_currentRequestData.RawRequest != null)
			{
				if (_currentHeader != null && _currentHeader.RequestLength < _currentRequestData.RequestSize)
				{
					_trafficViewerFile.SaveRequest(_currentIndex, _currentRequestData);
				}
			}

			//handle the case when there are two or more subsequent responses on the same thread(AppScan Manual Explore)
			//the fact that we still have more than one requests in this threads stack means we
			//didn't finish to process the first
			while (_currentRequestData.RawResponse != null && _currentThreadInfo.CurrentRequests.Count > 1)
			{
				//the fact that we are now receiving the second response means that the first request is complete
				//extract the first request from the stack of current requests of the thread, save it and drop it
				KeyValuePair<int, RequestResponseBytes> top = _currentThreadInfo.CurrentRequests.Pop();
				TVRequestInfo header = _trafficViewerFile.GetRequestInfo(top.Key);
				//save the response only if it has changed since last
				if (top.Value.ResponseSize > header.ResponseLength)
				{
					_trafficViewerFile.SaveResponse(top.Key, top.Value);
				}
				//update the request counter
				_thisSessionRequestCount++;
				//set the _currentRequestData to the next request data in the stack
				_currentRequestData = _currentThreadInfo.CurrentRequests.Peek().Value;
				_currentIndex = _currentThreadInfo.CurrentRequests.Peek().Key;
				_currentHeader = _trafficViewerFile.GetRequestInfo(_currentIndex);
			}

			if (lineBytes != null)
			{
				_currentRequestData.AddToResponse(lineBytes);
			}
			_isNewThreadChunk = false;
			_currentHeader.ResponseStatus =
				Utils.RegexFirstGroupValue(line, _responseStatusRegex);
            if (_currentTime != null)
            {
                try
                {
                    _currentHeader.ResponseTime =
                        DateTime.ParseExact(_currentTime, _timeFormat, _dateTimeFormatInfo);
                }
                catch { }
            }
            else
            {
                _currentHeader.ResponseTime = DateTime.Now;
            }
			_currentThreadInfo.Location = LocationInThread.InsideResponse;
		}

		private void HandleSendingRequestLine(string line) 
		{ 
			byte[] bytes;
			
			bytes = ReadNextBytes(line, _sendingRequestRegex);
			if (bytes != null)
			{
				if (_currentRequestData == null || _currentRequestData.RawResponse != null)
				{
					_currentRequestData = new RequestResponseBytes();
					_currentHeader = null;
				}

				_currentRequestData.AddToRequest(bytes);
				//check if this the start of a new request
				if (_currentHeader == null || String.IsNullOrEmpty(_currentHeader.RequestLine))
				{
					string reqLine = HttpRequestInfo.GetRequestLine(_currentRequestData.RawRequest);
					//check if it's recognized as a valid request line
					if (_lineTypeSelector.GetLineType(reqLine) == LineType.FirstRequestLine)
					{
						HandleFirstRequestLine(reqLine, null);
					}
				}

			}
		}


		private void HandleReceivingResponseLine(string line)
		{
			byte[] bytes;

			bytes = ReadNextBytes(line, _receivingResponseRegex);
			if (bytes != null)
			{
				if (_currentRequestData == null || _currentHeader == null)
				{
					//this is a situation where we have a response without a request
					return;
				}

				_currentRequestData.AddToResponse(bytes);
				
				if (String.IsNullOrEmpty(_currentHeader.ResponseStatus))
				{
					//check if we have the full response line
					MemoryStream ms = new MemoryStream(_currentRequestData.RawResponse);
					byte[] respLineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE);
					string respLine = Utils.ByteToString(respLineBytes);
					if (_lineTypeSelector.GetLineType(respLine) == LineType.FirstResponseLine)
					{
						HandleFirstResponseLine(respLine, null);
					}
				}

			}
		}


		private byte[] ReadNextBytes(string line, Regex regex)
		{
			byte[] bytes = null;
			Match m = regex.Match(line);
			if (m.Groups.Count > 1)
			{
				int length;
				if (int.TryParse(m.Groups[1].Value, out length))
				{
					//read the next bytes
					bytes = new byte[length];
					_rawLog.Read(bytes, 0, length);
				}
			}
			return bytes;
		}

		
		/// <summary>
		/// Signal that the current thread is interrupted
		/// </summary>
		private void HandleEndThread()
		{
			_currentThreadInfo.Suspended = true;
		}

		private void HandleHttpTraffic(string line, byte[] lineBytes)
		{
			if (_currentThreadInfo.Suspended)
			{
				return; //we are outside of the current thread
			}

			bool addNewLine = true;
			if (_isNewThreadChunk)
			{
				_isNewThreadChunk = false;
				addNewLine = false;
			}
			//add current line to the corresponding request component
			switch (_currentThreadInfo.Location)
			{
				case LocationInThread.InsideRequest:
					if (addNewLine)
					{
						_currentRequestData.AddToRequest(_newLineBytes);

					}
					_currentRequestData.AddToRequest(lineBytes);
					//extract host information if available
					break;
				case LocationInThread.InsideResponse:
					if (addNewLine)
					{
						_currentRequestData.AddToResponse(_newLineBytes);
					}
					_currentRequestData.AddToResponse(lineBytes);
					break;
			}
		}

		private void HandleCustomFields(string line)
		{
			string s;
			Dictionary<string, string> customFields;
			foreach (KeyValuePair<string, string> fieldDefinition in _customFieldDefinitions)
			{
				if (_currentIndex == -1) return;
				s = Utils.RegexFirstGroupValue(line, fieldDefinition.Value);
				if (s != String.Empty)
				{
					customFields = _currentHeader.CustomFields;
					if (customFields == null)
					{
						customFields = new Dictionary<string, string>();
						_currentHeader.CustomFields = customFields;
					}
					if (customFields.ContainsKey(fieldDefinition.Key))
					{
						customFields[fieldDefinition.Key] = s;
					}
					else
					{
						customFields.Add(fieldDefinition.Key, s);
					}
				}
			}
		}

		private void HandleResponseReceivedMessage()
		{
			//save the response only if it has changed since last time
			if (_currentHeader != null && _currentHeader.ResponseLength > _currentRequestData.ResponseSize)
			{
				_trafficViewerFile.SaveResponse(_currentIndex, _currentRequestData);
				_thisSessionRequestCount++;
			}
		}

		#endregion

		/// <summary>
		/// This is the method resposible for parsing the raw traffic log
		/// </summary>
		private void ParseFunction()
		{
			_parserStatus = TrafficParserStatus.Running;
			byte[] bytes; //will store a line of bytes
			string line; //will store the converted bytes to string
			LineType lineType;

			_thisSessionRequestCount = 0;
            StreamReader sr = new StreamReader(_rawLog);


            do
			{
				try
				{
                    line = sr.ReadLine();
					lineType = _lineTypeSelector.GetLineType(line);

					//if the end of file was reached or the user stopped the parse
					if (lineType == LineType.EndOfFile || _stopRequested)
					{
						HandleHalt();
						return;
					}

                    bytes = Encoding.UTF8.GetBytes(line);

                    //according to the line type perform the coresponding action
                    switch (lineType)
					{
						case LineType.BeginThread:
							HandleBeginThread(line);
							break;
						case LineType.EndThread:
							HandleEndThread();
							break;
						case LineType.FirstRequestLine:
							HandleFirstRequestLine(line, bytes);
							break;
						case LineType.FirstResponseLine:
							HandleFirstResponseLine(line, bytes);
							break;
						case LineType.HttpTraffic:
							HandleHttpTraffic(line, bytes);
							break;
						case LineType.ResponseReceived:
							HandleResponseReceivedMessage();
							break;
						case LineType.SendingRequest:
							HandleSendingRequestLine(line);
							break;
						case LineType.ReceivingResponse:
							HandleReceivingResponseLine(line);
							break;
					}

					//handle tail (if is enabled _tailChunk > 0)
					if (_tailChunk > 0 && _thisSessionRequestCount >= _tailChunk)
					{
						_stopRequested = true;
					}

					//extract custom fields
					HandleCustomFields(line);
				}
				catch (OutOfMemoryException)
				{
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Out of memory exception occured during parsing {0}. Calling the garbage collector.", _rawLog.Name);
					GC.Collect();
				}
				catch (Exception ex)
				{
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error parsing {0}:{1}", _rawLog.Name, ex.Message);
				}
			}
			while (true);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Parses the specified raw log to a Traffic Viewer File format
		/// </summary>
		/// <param name="rawLogPath"></param>
		/// <param name="currentFile"></param>
		/// <param name="parsingOptions"></param>
		public void Parse(string rawLogPath, ITrafficDataAccessor currentFile, ParsingOptions parsingOptions)
		{
            //clears the data from the previous parsing operation
            _threads.Clear();
            _thisSessionRequestCount = 0;
            _currentHeader = null;
            _currentIndex = -1;

			_stopRequested = false;
			_proxyConnectionToSiteRegex = parsingOptions.ProxyConnectionToSiteRegex;
			_sendingRequestRegex = new Regex(parsingOptions.SendingRequestRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_receivingResponseRegex = new Regex(parsingOptions.ReceivingResponseRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_threadIdRegex = parsingOptions.ThreadIdRegex;
			_descriptionRegex = parsingOptions.DescriptionRegex;
			_timeFormat = parsingOptions.TimeFormat;
			
			if (parsingOptions.UseExclusions)
			{
				_exclusions = parsingOptions.GetExclusions();
			}
			else
			{
				_exclusions = new List<string>();
			}
			_customFieldDefinitions = parsingOptions.GetCustomFields();
			_responseStatusRegex = parsingOptions.ResponseStatusRegex;
			_lineTypeSelector = new LineTypeSelector(parsingOptions);
			_timeRegex = Utils.GetTimeRegex(_timeFormat);
			_parsingOptions = parsingOptions;
			_trafficViewerFile = currentFile;
			_trafficViewerFile.Profile = _parsingOptions;
			//open the raw log
			_rawLog = ParserUtils.OpenFile(rawLogPath);
			if (_rawLog == null)
			{
				throw new Exception("Cannot open raw traffic log in parsing thread");
			}
			_currentThreadInfo = new ThreadInfo();
			ParseFunction();
		}

		/// <summary>
		/// Temporarily stops the parse operation
		/// </summary>
		public void Stop()
		{
			_stopRequested = true;
		}

		/// <summary>
		/// Resumes the parse operation
		/// </summary>
		public void Resume()
		{
			if (_parserStatus == TrafficParserStatus.Stopped && _rawLog != null)
			{
				_stopRequested = false;
				ParseFunction();
			}
		}

		/// <summary>
		/// Same as resume but stops after tailChunk requests
		/// </summary>
		/// <param name="tailChunk">How many requests should be loaded at a time</param>
		public void Tail(int tailChunk)
		{
			_tailChunk = tailChunk;
			Resume();
			_tailChunk = -1;
		}

		/// <summary>
		/// Clears the source file
		/// </summary>
		public void ClearSource()
		{
			//open the raw log with different permissions
			if (_rawLog != null)
			{
				try
				{
					FileStream _clearHandle = File.Open(_rawLog.Name, FileMode.Open,
						FileAccess.ReadWrite, FileShare.ReadWrite);
					_clearHandle.SetLength(0);
					_clearHandle.Close();
					_rawLog.Position = 0;
				}
				catch
				{ 
					throw new Exception("Unable to clear source. File may be in use");
				}
			}
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultTrafficParser()
		{
			_importTypes.Add("Traffic Files", "*Traffic*");
			_importTypes.Add("All Files", "*.*");

		}

		/// <summary>
		/// Closes the target file if it wasn't closed
		/// </summary>
		~DefaultTrafficParser()
		{
			try
			{
				if (_rawLog != null)
				{
					_rawLog.Close();
				}
			}
			catch { }
		}
		
		/// <summary>
		/// Specifies the import extension filters accompanied by their description e.g {Traffic File,*Traffic*} or {Manual Explore File,*.exd}
		/// </summary>
		public Dictionary<string, string> ImportTypes
		{
			get { return _importTypes; }
		}

		/// <summary>
		/// Starts parsing operation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		public void Parse(object sender, ITrafficDataAccessor currentFile, ParsingOptions options)
		{
			throw new NotImplementedException("Not available for this import type");
		}

		/// <summary>
		/// The name of the importer
		/// </summary>
		public string Name
		{
			get { return Resources.TrafficLogParserName; }
		}


		/// <summary>
		/// Supports files and tails
		/// </summary>
		public ImportMode ImportSupport
		{
			get { return ImportMode.Files | ImportMode.Tail; }
		}


	}
}
