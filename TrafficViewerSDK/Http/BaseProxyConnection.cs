using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Net.Security;
using System.Security.Authentication;
using TrafficViewerSDK.Properties;
using System.Net;
using System.Text.RegularExpressions;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Class containing common code used by all proxy connections
	/// </summary>
	public abstract class BaseProxyConnection : IProxyConnection
	{
		/// <summary>
		/// The name of the connection header
		/// </summary>
		protected const string CONNECTION_HEADER = "Connection";
		/// <summary>
		/// The name of the proxy-connection header
		/// </summary>
		protected const string PROXY_CONNECTION_HEADER = "Proxy-Connection";

		private TcpClient _client;
		private HttpProxyClientStreamWrapper _clientStreamWrapper;
		private DateTime _currentRequestTime;
		private bool _isSecure = false;
		
		/// <summary>
		/// Stores information about the request received from the client
		/// </summary>
		protected HttpRequestInfo _requestInfo;
		/// <summary>
		/// Used to build the request as it comes from the client stream
		/// </summary>
		protected ByteArrayBuilder _requestBuilder;
		/// <summary>
		/// Used to build the data to be saved to the disk
		/// </summary>
		private RequestResponseBytes _currentRequestResponseBytes = null;
		/// <summary>
		/// Whether the connection should be closed
		/// </summary>
		protected bool _isClose = false;
		/// <summary>
		/// Lock used when sending the response back to the client
		/// </summary>
		protected Object _proxyLock = new Object();
		/// <summary>
		/// Size of the buffer used to send/receive http trafic
		/// </summary>
		private const int BUFFER_SIZE = 4096;
		/// <summary>
		/// The buffer used to send/receive http trafic
		/// </summary>
		private byte[] _buffer = new byte[BUFFER_SIZE];
		/// <summary>
		/// Temporary buffer for reading/writing
		/// </summary>
		protected byte[] Buffer
		{
			get
			{
				return _buffer;
			}
		}
		/// <summary>
		/// Wraps a stream object associated with client connection
		/// </summary>
		protected HttpProxyClientStreamWrapper ClientStreamWrapper
		{
			get
			{
				return _clientStreamWrapper;
			}
		}
		private string _requestDescription;
		private TVRequestInfo _currDataStoreRequestInfo = null;
		/// <summary>
		/// Gets the current data store request info
		/// </summary>
		protected TVRequestInfo CurrDataStoreRequestInfo
		{
			get { return _currDataStoreRequestInfo; }
		}
		
		/// <summary>
		/// Where to save the request data
		/// </summary>
		private ITrafficDataAccessor _trafficDataStore = null;
		/// <summary>
		/// Where to read/save request data
		/// </summary>
		public ITrafficDataAccessor TrafficDataStore
		{
			get { return _trafficDataStore; }
		}
		/// <summary>
		/// Whether the connection is closed
		/// </summary>
		public bool Closed
		{
			get
			{
				return !_client.Connected || _clientStreamWrapper.Closed;
			}
		}

		private bool _isBusy = false;
		/// <summary>
		/// True if the connection is processing a request, false if the connection is closed or just waiting for a new request to come in
		/// </summary>
		public bool IsBusy
		{
			get 
			{
				return !Closed && _isBusy;
			}
			set 
			{
				_isBusy = value;
			}
		}
		
		/// <summary>
		/// The HTTP Factory to be used by this connection
		/// </summary>
		protected abstract IHttpClient HttpClient
		{
			get;
		}

		private IEnumerable<string> _exclusions = null;
        /// <summary>
        /// whether this is an essential request
        /// </summary>
        protected bool _isNonEssential;
        /// <summary>
        /// Flag indicating a connection stop request
        /// </summary>
        private bool _stop = true;
		
		/// <summary>
		/// Creates a connection
		/// </summary>
		/// <param name="client"></param>
		/// <param name="isSecure"></param>
		public BaseProxyConnection(TcpClient client, bool isSecure)
			: this(client, isSecure, null, null)
		{
		
		}

		/// <summary>
		/// Creates a connection
		/// </summary>
		/// <param name="client">TCP client to use</param>
		/// <param name="isSecure">Whether to use SSL or not</param>
		/// <param name="dataStore">Data store to save requests/responses to or to read from</param>
		/// <param name="requestDescription">Description added to the data store</param>
		public BaseProxyConnection(TcpClient client, bool isSecure, ITrafficDataAccessor dataStore, string requestDescription)
		{
			_client = client;
			_clientStreamWrapper = new HttpProxyClientStreamWrapper(_client);
			_isSecure = isSecure;
			_trafficDataStore = dataStore;
			_requestDescription = requestDescription;
			_requestBuilder = new ByteArrayBuilder();
			if (dataStore != null)
			{
				_exclusions = dataStore.Profile.GetExclusions();
			}
			HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
							"Inbound connection from {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address);
			
		}

		/// <summary>
		/// Destroy the proxy
		/// </summary>
		~BaseProxyConnection()
		{
			Stop();
		}


		/// <summary>
		/// Adds the request to the proxy data store
		/// </summary>
		protected void AddRequestToDataStore()
		{
			TVRequestInfo currDataStoreRequestInfo = new TVRequestInfo();
			currDataStoreRequestInfo.RequestLine = _requestInfo.RequestLine;

			currDataStoreRequestInfo.Description = _requestDescription;
			currDataStoreRequestInfo.RequestTime = _currentRequestTime;
			currDataStoreRequestInfo.ThreadId = Utils.GetCurrentWin32ThreadId().ToString();
			currDataStoreRequestInfo.IsHttps = _requestInfo.IsSecure;
            currDataStoreRequestInfo.Host = _requestInfo.Host;

			int reqId = TrafficDataStore.AddRequestInfo(currDataStoreRequestInfo);
			if (reqId != -1)
			{
				_currentRequestResponseBytes = new RequestResponseBytes();
				_currentRequestResponseBytes.AddToRequest(_requestInfo.ToArray(false));
				//saving the request in a direct to site format, since it will come
				//in proxy format GET http://site.com/ rather than  GET /, 
				//if we need a server to connect to the target the client will handle that 
				TrafficDataStore.SaveRequest(reqId,_currentRequestResponseBytes);
				_currDataStoreRequestInfo = TrafficDataStore.GetRequestInfo(reqId);
			}
		}

		/// <summary>
		/// Checks if the request is excluded
		/// </summary>
		/// <param name="requestHead">Request line + Request Headers</param>
		/// <returns></returns>
		protected virtual bool IsExcludedRequest(string requestHead)
		{
			bool result = false;
			
			if(_exclusions != null)
			{
				result = Utils.IsMatchInList(requestHead, _exclusions);
			}

			return result;
		}


		/// <summary>
		/// Optional method that allows the child class to modify the chunk before being sent to the client
		/// </summary>
		/// <param name="chunk"></param>
		/// <returns></returns>
		protected virtual byte[] HandleResponseByteChunk(byte[] chunk) 
		{
			if (_currDataStoreRequestInfo != null)
			{
				_currentRequestResponseBytes.AddToResponse(chunk);
			}
			return chunk;
		}

		/// <summary>
		/// Writes a response to the client stream
		/// </summary>
		/// <param name="responseInfo"></param>
		protected virtual void ReturnResponse(HttpResponseInfo responseInfo)
		{
			HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
							"Sending response: {0} for request: {1}", responseInfo.StatusLine, _requestInfo.RequestLine);


			if (_currDataStoreRequestInfo != null)
			{
				_currDataStoreRequestInfo.ResponseStatus = responseInfo.Status.ToString();
				_currDataStoreRequestInfo.ResponseTime = DateTime.Now;
			}

			//process response headers
			if (responseInfo.Status == 401)
			{ 
				//we need to send the proxy support header/ or overwrite the existing proxy support header
				responseInfo.Headers["Proxy-Support"] = "Session-Based-Authentication";
			}

			//if connection is close disconnect, or if the client sent us a connection close message
			if (String.Compare(responseInfo.Headers[CONNECTION_HEADER], "close", true) == 0 || 
				String.Compare(_requestInfo.Headers[CONNECTION_HEADER], "close", true) == 0)
			{
				_isClose = true;
			}
			
			byte[] responseHead = HandleResponseByteChunk(responseInfo.ResponseHead);

			if (!TryWriteToStream(responseHead))
			{
				return;
			};

			// Return the substitute response body
			if (responseInfo.ResponseBody.IsChunked)
			{
				byte[] chunk;
				byte[] chunkBuf;
				while ((chunk = responseInfo.ResponseBody.ReadChunk()) != null)
				{
					//write the chunk size line
					chunkBuf = Constants.DefaultEncoding.GetBytes(String.Format("{0:x}\r\n", chunk.Length));
					chunkBuf = HandleResponseByteChunk(chunkBuf);
					if (!TryWriteToStream(chunkBuf))
					{
						return;
					}
					//write the chunk
					chunk = HandleResponseByteChunk(chunk);
					if (!TryWriteToStream(chunk))
					{
						return;
					}
					chunkBuf = Constants.DefaultEncoding.GetBytes("\r\n");
					chunkBuf = HandleResponseByteChunk(chunkBuf);
					if (!TryWriteToStream(chunkBuf))
					{
						return;
					}

				}
				//write a last chunk with the value 0
				// write the last chunk size
				chunkBuf = Constants.DefaultEncoding.GetBytes("0\r\n\r\n");
				chunkBuf = HandleResponseByteChunk(chunkBuf);
				if (!TryWriteToStream(chunkBuf))
				{
					return;
				}
			}
			else
			{
				byte[] buffer = responseInfo.ResponseBody.ToArray();
				if (buffer != null)
				{
					buffer = HandleResponseByteChunk(buffer);
					if (!TryWriteToStream(buffer))
					{
						return;
					}
				}
			}


			//cleanup the request info
			_requestBuilder = new ByteArrayBuilder();

			if (_currDataStoreRequestInfo != null)
			{
				TrafficDataStore.SaveResponse(_currDataStoreRequestInfo.Id, _currentRequestResponseBytes);
			}

			_currDataStoreRequestInfo = null;
			_currentRequestResponseBytes = null;

			//close the connection if the request had a connection close header
			if (_isClose)
			{
				ClientStreamWrapper.Close();
			}
			else
			{
				_isBusy = false;
				ClientStreamWrapper.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(OnRead), ClientStreamWrapper);
			}

		}

		/// <summary>
		/// Attempts to write to the network stream
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns>true on success</returns>
		private bool TryWriteToStream(byte[] bytes)
		{
			bool success = ClientStreamWrapper.Write(bytes, 0, bytes.Length);
			if (!success)
			{
				ClientStreamWrapper.Close();
			}
			return success;
		}

		/// <summary>
		/// Override to change the OnRead behavior
		/// </summary>
		/// <param name="ar"></param>
		protected virtual void OnRead(IAsyncResult ar)
		{
			try
			{
				if (_stop || Closed)
				{
					return;
				}

				_isBusy = true;
				HttpProxyClientStreamWrapper wrapper = (HttpProxyClientStreamWrapper)ar.AsyncState;
				int bytesRead = 0;

				bytesRead = wrapper.EndRead(ar);

				//we are still connected and we read more bytes

				if (bytesRead > 0)
				{
					// Append data to the request
					_requestBuilder.AddChunkReference(Buffer, bytesRead);

					_requestInfo = new HttpRequestInfo(_requestBuilder.ToArray(), false);
					
					if (!_requestInfo.IsFullRequest)
					{
						// not finished keep on reading!
						wrapper.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(OnRead), wrapper);
					}
					else
					{
						lock (_proxyLock)
						{
							// Done reading, process the request
							ProcessRequest();
						}
					}
				}
				else
				{
					//we read 0 bytes
					_isBusy = _requestBuilder.Length > 0;
				}
			}
			catch (Exception ex)
			{
				ClientStreamWrapper.Close();
				HttpServerConsole.Instance.WriteLine(ex);
			}

		}

		

		/// <summary>
		/// Process a CONNECT request
		/// </summary>
		protected void ProcessConnect()
		{
			// Reset request
			_requestBuilder = new ByteArrayBuilder();

			try
			{
				// got a connect
				string response = _requestInfo.HttpVersion + " 200 Connection established\r\nContent-length: 0\r\nProxy-Agent: Traffic Viewer Proxy\r\n\r\n";
				byte[] line = Constants.DefaultEncoding.GetBytes(response);

				HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
							"Sending response: 200 Connection established for request: {0}", _requestInfo.RequestLine);

				// Give the OK
				if (!TryWriteToStream(line))
				{
					return;
				}

				if (!TrySecureStream())
				{
					return;
				}

				// Keep reading on this now encrypted stream
				ClientStreamWrapper.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(OnRead), ClientStreamWrapper);

			}
			catch (Exception ex)
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
							"SSL Connect failed: {0}", ex.Message);
				ClientStreamWrapper.Close();
			}
		}

		/// <summary>
		/// Override to do something with the request before sending it to the site
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		protected virtual HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
		{
			return requestInfo;
		}

		/// <summary>
		/// Override to do something with the response before returning it to the client
		/// </summary>
		/// <param name="responseInfo"></param>
		/// <returns></returns>
		protected virtual HttpResponseInfo OnBeforeResponseToClient(HttpResponseInfo responseInfo)
		{
			return responseInfo;
		}

		
		/// <summary>
		/// Handles the request
		/// </summary>
		protected virtual void ProcessRequest()
		{
			try
			{
				_requestInfo.IsSecure = ClientStreamWrapper.IsSecure;
				_currentRequestTime = DateTime.Now;

				HttpServerConsole.Instance.WriteLine("Got request: {0}", _requestInfo.RequestLine);

				if (_requestInfo.IsConnect)
				{
					// Connect requests are handled differently.
					// Process it and return.
					ProcessConnect();
					return;
				}
				
				_isNonEssential = IsExcludedRequest(_requestInfo.RequestLine + Environment.NewLine + _requestInfo.Headers.ToString());	//test both request line (for file extensions) and headers (for domains) 

				_requestInfo = ProcessHeaders(_requestInfo, _isNonEssential);

                if (!_isNonEssential)
                {
                   
                    if (_trafficDataStore != null)
                    {
                        //save request to the traffic data store before sending it
                        AddRequestToDataStore();
                    }
                }
				
				_requestInfo = OnBeforeRequestToSite(_requestInfo);
				
				HttpResponseInfo responseInfo = HttpClient.SendRequest(_requestInfo);

				if (responseInfo == null) //timeout or connection closed
				{
					throw new WebException("Send error", WebExceptionStatus.SendFailure);
				}

				responseInfo = OnBeforeResponseToClient(responseInfo);

				ReturnResponse(responseInfo);
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
		}


        

		/// <summary>
		/// Handles any exception that occurs for the current request
		/// </summary>
		/// <param name="ex"></param>
		private void HandleException(Exception ex)
		{
			try
			{
				
				HttpProxyException proxyException = ex as HttpProxyException;
				if (proxyException == null)
				{
					WebException webException = ex as WebException;

					//check if we got a web exception
					if (webException != null)
					{
						//handle the exception as web exception
						if (webException.Status == WebExceptionStatus.TrustFailure || webException.Status == WebExceptionStatus.SecureChannelFailure)
						{
							proxyException = new HttpProxyException(HttpStatusCode.BadGateway, "Bad SSL Connection", ServiceCode.OnSecureInvalidCertificate);
						}
						else if (webException.Status == WebExceptionStatus.ServerProtocolViolation)
						{
							proxyException = new HttpProxyException(HttpStatusCode.BadGateway, "Server Protocol Violation", ServiceCode.ServerProtocolViolation);
						}
						else
						{
							//the web exception is coming from the client
							proxyException = new HttpProxyException(HttpStatusCode.GatewayTimeout, "Proxy Error Sending Request to the Site", ServiceCode.ProxyErrorSendingRequestToTheSite);
						}
					}
					else
					{
						proxyException = new HttpProxyException(HttpStatusCode.InternalServerError, "AppScan Proxy Internal Error", ServiceCode.ProxyInternalError);
					}
				}
				string message = String.Format(Resources.ProxyInternalError, _requestInfo.RequestLine);
				HttpServerConsole.Instance.WriteLine(LogMessageType.Error, message);
				
				ReturnHttpErrorResponse(proxyException);
				
			}
			catch
			{
				// do nothing
			}
		}

		/// <summary>
		/// Process Request headers before sending
		/// Add keep alive for requests that contain authorization
		/// </summary>
		/// <param name="reqInfo">The request info to be processed</param>
		/// <param name="isNonEssential">Whether the request is deemed non essential</param>
		protected virtual HttpRequestInfo ProcessHeaders(HttpRequestInfo reqInfo, bool isNonEssential)
		{
			//the proxy connection header was intended to us, don't send it to the site
			//instead pass the information via the connection header, if we want the connection be kept alive
			if (reqInfo.Headers[PROXY_CONNECTION_HEADER] != null)
			{
				reqInfo.Headers[CONNECTION_HEADER] = reqInfo.Headers[PROXY_CONNECTION_HEADER];
				reqInfo.Headers.Remove(PROXY_CONNECTION_HEADER);
			}
			//ensure the client keeps the connection open for handshake authorization requests
			//the connection header may be missing
			
			if (reqInfo.Headers["Authorization"] != null &&
			reqInfo.Headers["Authorization"] != "Basic")
			{
				reqInfo.Headers[CONNECTION_HEADER] = "keep-alive";
			}

			return reqInfo;
		}

		/// <summary>
		/// Writes a CRWAE message to the clien
		/// </summary>
		/// <param name="proxyException"></param>
		protected void ReturnHttpErrorResponse(HttpProxyException proxyException)
		{
			ReturnHttpErrorResponse(proxyException.StatusCode, proxyException.StatusLine, proxyException.ServiceCode, proxyException.MessageArgs);
		}

		/// <summary>
		/// Writes a CRWAE message to the client when response is not available from server
		/// </summary>
		/// <param name="statusCode">HTTP status code for the response</param>
		/// <param name="reason">Brief explanation of error</param>
		/// <param name="serviceCode">ID of the CRAWE message</param>
		/// <param name="args">Message args</param>
		protected virtual void ReturnHttpErrorResponse(HttpStatusCode statusCode, string reason, ServiceCode serviceCode, params object [] args)
		{
			
			byte[] messageBytes = HttpErrorResponse.GenerateHttpErrorResponse(statusCode, reason, serviceCode, args);
			if (!TryWriteToStream(messageBytes))
			{
				return;
			}

			//add the response if we are adding requests to the data source
			if (_currDataStoreRequestInfo != null)
			{
				_currDataStoreRequestInfo.ResponseStatus = statusCode.ToString();
				_currDataStoreRequestInfo.ResponseTime = DateTime.Now;
				TrafficDataStore.SaveResponse(_currDataStoreRequestInfo.Id, messageBytes);
				_currDataStoreRequestInfo = null;
			}
			ClientStreamWrapper.Close();
		}


		/// <summary>
		/// Secures the current connection stream
		/// </summary>
		/// <returns>True if the operation is successful</returns>
		protected virtual bool TrySecureStream()
		{
            string host = null;
            if (_requestInfo != null)
            {
                host = _requestInfo.Host;
            }
			bool success = ClientStreamWrapper.SecureStream(host);
			if (!success)
			{
				ClientStreamWrapper.Close();
			}
			return success;
		}


		/// <summary>
		/// Starts the connection
		/// </summary>
		public virtual void Start()
		{
            _stop = false;
			try
			{
				if (_isSecure)
				{
					if (!TrySecureStream())
					{
						return;	
					}
				}

				// Keep on reading
				IAsyncResult result = ClientStreamWrapper.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(OnRead), ClientStreamWrapper);
				if (result == null)
				{
					ClientStreamWrapper.Close();
				}
			}
			catch (Exception ex)
			{
				HttpServerConsole.Instance.WriteLine("Error starting proxy connection: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Stops the proxy
		/// </summary>
		public void Stop()
		{
            _stop = true;
			if (ClientStreamWrapper != null)
			{
				ClientStreamWrapper.Close();
			}
           
		}


	
	}
}
