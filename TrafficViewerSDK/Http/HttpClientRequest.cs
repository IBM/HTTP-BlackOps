using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Threading;
using TrafficViewerSDK.Properties;
using TrafficViewerSDK.Options;
using System.Diagnostics;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Can be used to send an HTTP(s) request
	/// </summary>
	public class HttpClientRequest
	{
		private const int MAX_BUFFER_SIZE = 10240;

		private object _lock = new object();

		private ManualResetEvent _requestCompleteEvent = new ManualResetEvent(false);
		/// <summary>
		/// Used by the caller to wait for the response to come back
		/// </summary>
		public ManualResetEvent RequestCompleteEvent
		{
			get { return _requestCompleteEvent; }
		}

		/// <summary>
		/// Current read  buffer
		/// </summary>
		private byte[] _buffer;
		/// <summary>
		/// Used to build the response
		/// </summary>
		private ByteArrayBuilder _dataBuilder;

		/// <summary>
		/// The connection to the server
		/// </summary>
		private HttpClientConnection _connection;

		private byte[] _response;
		/// <summary>
		/// Gets the current response
		/// </summary>
		public byte[] Response
		{
			get { return _response; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public HttpClientRequest()
		{

		}

		/// <summary>
		/// Creates a request with a predefined connection
		/// </summary>
		/// <param name="connection"></param>
		public HttpClientRequest(HttpClientConnection connection)
		{
			_connection = connection;
		}

		/// <summary>
		/// Called to read the response asyncroneusly
		/// </summary>
		/// <param name="ar"></param>
		private void ReadResponse(IAsyncResult ar)
		{
			Stream stream = null;
			int bytesRead = 0;
			HttpClientResult result = HttpClientResult.Error;
			try
			{
				lock (_lock)
				{
					stream = (Stream)ar.AsyncState;
					bytesRead = stream.EndRead(ar);

					if (bytesRead > 0)
					{
						//add to the exiting data
						_dataBuilder.AddChunkReference(_buffer, bytesRead);
						//make a new chunk
						_buffer = new byte[MAX_BUFFER_SIZE];
						//continue reading
						stream.BeginRead(_buffer, 0, MAX_BUFFER_SIZE, new AsyncCallback(ReadResponse), stream);
					}
					else
					{
						//construct the full response
						_response = _dataBuilder.ToArray();
						result = HttpClientResult.Success;
					}
				}
			}
			catch
			{
				//we don't care that much for the errors that occur here
			}
			finally
			{
				if (bytesRead == 0)
				{
					//if the caller was waiting on the request complete event allow continuation
					_requestCompleteEvent.Set();

					//we're done reading close the connection and trigger the event with the collected response
					//the result will be success
					if (RequestComplete != null)
					{
						RequestComplete.Invoke
								(new HttpClientRequestCompleteEventArgs(new HttpResponseInfo(_response)));
					}
					_connection.Close();
				}
			}
		}


		/// <summary>
		/// Triggered when a request was complete
		/// </summary>
		public event HttpClientRequestCompleteEvent RequestComplete;


		/// <summary>
		/// Sends the specified request to the server
		/// (Proxy not supported)
		/// </summary>
		/// <param name="rawRequest"></param>
		public void SendRequest(string rawRequest)
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(rawRequest);
			this.SendRequest(reqInfo, reqInfo.IsSecure);
		}

		/// <summary>
		/// Sends the specified request to the server
		/// </summary>
		/// <param name="rawRequest"></param>
		/// <param name="https"></param>
		public void SendRequest(string rawRequest, bool https)
		{
			HttpRequestInfo reqInfo = new HttpRequestInfo(rawRequest);
			this.SendRequest(reqInfo, reqInfo.Host, reqInfo.Port, https);
		}

		/// <summary>
		/// Sends the specified request to the server 
		/// </summary>
		/// <param name="parsedRequest"></param>
		/// <param name="https"></param>
		public void SendRequest(HttpRequestInfo parsedRequest, bool https)
		{
			SendRequest(parsedRequest, parsedRequest.Host, parsedRequest.Port, https);
		}


		/// <summary>
		/// Sends the specified request to the server
		/// (Proxy not supported)
		/// </summary>
		/// <param name="parsedRequest"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="https"></param>
		public void SendRequest(HttpRequestInfo parsedRequest, string host, int port, bool https)
		{
            _requestCompleteEvent.Reset();

			_dataBuilder = new ByteArrayBuilder();

			if (_connection == null)
			{
				_connection = new HttpClientConnection(host, port, https);
			}

			try
			{
				//connect
				if (_connection.Connect())
				{
					bool isProxy = _connection.Host != host;

					//add connection closed header only this is supported at the moment
					parsedRequest.Headers["Connection"] = "close";

					if (isProxy)
					{
						parsedRequest.Headers["Proxy-Connection"] = "close";
					}

					// Turn off accepting of gzip/deflate						
					//parsedRequest.Headers.Remove("Accept-Encoding");

					//calculate the content length
					if (parsedRequest.ContentData != null)
					{
						parsedRequest.Headers["Content-Length"] = parsedRequest.ContentData.Length.ToString();
					}

					parsedRequest.Host = host;
					parsedRequest.Port = port;
					parsedRequest.IsSecure = https;

					if (isProxy && https)
					{
						//send a connect message to the proxy
						SendConnect(host, port);

					}

					byte[] reqBytes = Constants.DefaultEncoding.GetBytes(parsedRequest.ToString(isProxy && !https));

					//write to the stream
					_connection.Stream.Write(reqBytes, 0, reqBytes.Length);

					//start reading
					_buffer = new byte[MAX_BUFFER_SIZE];
					_connection.Stream.BeginRead(_buffer, 0, MAX_BUFFER_SIZE, new AsyncCallback(ReadResponse), _connection.Stream);
				}
				else
				{
					throw new Exception("Cannot connect to server");
				}
			}
			catch (Exception ex)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "HttpClient error sending request {0}", ex.Message);
				//notify the caller that the request was completed with an error
				if (RequestComplete != null)
				{
					RequestComplete.Invoke(
						new HttpClientRequestCompleteEventArgs());
					RequestCompleteEvent.Set();
				}

				_connection.Close();
			}
		}

		private void SendConnect(string host, int port)
		{
			string connectRequest = String.Format(Resources.ConnectRequest, host, port);
			byte[] connectBytes = Constants.DefaultEncoding.GetBytes(connectRequest);
			_connection.Stream.Write(connectBytes, 0, connectBytes.Length);
			//read the response

			ByteArrayBuilder builder = new ByteArrayBuilder();
			byte[] buffer = new byte[MAX_BUFFER_SIZE];
			int bytesRead = _connection.Stream.Read(buffer, 0, MAX_BUFFER_SIZE);
			if (bytesRead > 0)
			{
				builder.AddChunkReference(buffer, bytesRead);
			}

			if (builder.Length == 0)
			{
				throw new Exception("No response to connect");
			}
			else
			{
				HttpResponseInfo respInfo = new HttpResponseInfo();
				respInfo.ProcessResponse(builder.ToArray());
				if (respInfo.Status != 200)
				{
					throw new Exception("Connect response didn't get 200 status");
				}
				else
				{
					//secure the connection
					_connection.SecureStream(host);
				}
			}
		}


	}


	/// <summary>
	/// Used to signal that a response was received or that an error occured
	/// </summary>
	/// <param name="e"></param>
	public delegate void HttpClientRequestCompleteEvent(HttpClientRequestCompleteEventArgs e);
	/// <summary>
	/// Information regarding result of a request
	/// </summary>
	public class HttpClientRequestCompleteEventArgs : EventArgs
	{
		private HttpClientResult _result = HttpClientResult.Error;
		/// <summary>
		/// The result of an HttpClient send operation
		/// </summary>
		public HttpClientResult Result
		{
			get { return _result; }
			set { _result = value; }
		}

		private HttpResponseInfo _httpResponse;
		
		/// <summary>
		/// The Http response, null if an error occurred
		/// </summary>
		public HttpResponseInfo HttpResponse
		{
			get { return _httpResponse; }
			set { _httpResponse = value; }
		}


		private byte[] _byteResponse;
		/// <summary>
		/// The response from a binary operation
		/// </summary>
		public byte[] ByteResponse
		{
			get { return _byteResponse; }
			set { _byteResponse = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public HttpClientRequestCompleteEventArgs()
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="response">The result of an HttpClient send operation</param>
		/// <param name="result">The Http response, null if an error occurred</param>
		public HttpClientRequestCompleteEventArgs(HttpResponseInfo response)
		{
			_result = HttpClientResult.Success;
			_httpResponse = response;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="response">The result of an HttpClient send operation</param>
		/// <param name="result">The Http response, null if an error occurred</param>
		public HttpClientRequestCompleteEventArgs(byte [] response)
		{
			_result = HttpClientResult.Success;
			_byteResponse = response;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="response">The result of an HttpClient send operation</param>
		/// <param name="result">The Http response, null if an error occurred</param>
		public HttpClientRequestCompleteEventArgs(string response):this(Constants.DefaultEncoding.GetBytes(response))
		{
		
		}
	}

}
