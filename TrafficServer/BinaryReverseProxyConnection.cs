using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficServer
{
	public class BinaryReverseProxyConnection : BaseProxyConnection
	{
		private string _forwardingHost;
		private int _forwardingPort;
		private System.Net.Sockets.TcpClient tcpClient;
		private bool _isSecure;
		private TrafficViewerSDK.ITrafficDataAccessor _dataStore;
		private TrafficViewerSDK.Http.INetworkSettings NetworkSettings;
		private Dictionary<string, string> Replacements; //not used for now
		private HttpClientConnection _upStreamConnection;

		private const int MAX_BUFFER_SIZE = 1024000;
		private byte[] _responseBuffer = new byte[MAX_BUFFER_SIZE];
		private long _clientId = DateTime.Now.Ticks;
		/// <summary>
		/// Used to build the response
		/// </summary>
		private ByteArrayBuilder _responseBuilder;
		private TVRequestInfo _messageReqInfo;
		private bool _isReading;

		public BinaryReverseProxyConnection(string _forwardingHost, int _forwardingPort, System.Net.Sockets.TcpClient tcpClient, bool isSecure, TrafficViewerSDK.ITrafficDataAccessor _dataStore, TrafficViewerSDK.Http.INetworkSettings NetworkSettings, Dictionary<string, string> Replacements)
			: base(tcpClient, isSecure)
		{
			// TODO: Complete member initialization
			this._forwardingHost = _forwardingHost;
			this._forwardingPort = _forwardingPort;
			this.tcpClient = tcpClient;
			this._isSecure = isSecure;
			this._dataStore = _dataStore;
			this.NetworkSettings = NetworkSettings;
			this.Replacements = Replacements;

			


		}

		public override void Start()
		{
			IsBusy = true;
			
			_upStreamConnection = new HttpClientConnection(_forwardingHost, _forwardingPort, _isSecure);
			if (_upStreamConnection.Connect())
			{
				base.Start();
			}
			else
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
					"Could not establish connection to {0}:{1}",
					_forwardingHost, _forwardingPort);
				Close();
				IsBusy = false;

			}

		}



		/// Override to change the OnRead behavior
		/// </summary>
		/// <param name="ar"></param>
		protected override void OnRead(IAsyncResult ar)
		{
			try
			{
				if (Closed)
				{
					return;
				}

				IsBusy = true;
				HttpProxyClientStreamWrapper wrapper = (HttpProxyClientStreamWrapper)ar.AsyncState;
				int bytesRead = 0;

				bytesRead = wrapper.EndRead(ar);

				//we are still connected and we read more bytes

				if (bytesRead > 0)
				{
					lock (_proxyLock)
					{
						_requestBuilder = new ByteArrayBuilder();
						// Append data to the request
						_requestBuilder.AddChunkReference(Buffer, bytesRead);
						// not finished keep on reading!
						ProcessMessage();
					}
				}
			}
			catch (Exception ex)
			{
				HttpServerConsole.Instance.WriteLine(ex);
				Close();

			}

		}

		private void ProcessMessage()
		{

			//add the request to data store
			_messageReqInfo = new TVRequestInfo();
			_messageReqInfo.IsHttps = _isSecure;
			_messageReqInfo.Description = "Binary Reverse Proxy";

			_messageReqInfo.RequestLine = String.Format("Binary Message - {0} Bytes", _requestBuilder.Length);
			_messageReqInfo.RequestTime = DateTime.Now;
			_dataStore.AddRequestInfo(_messageReqInfo);

			byte[] messageBytes = _requestBuilder.ToArray();

			_dataStore.SaveRequest(_messageReqInfo.Id, messageBytes);

			
			//write to the stream
			
			
			_responseBuilder = new ByteArrayBuilder();
			if (_upStreamConnection != null && _upStreamConnection.Stream != null && _upStreamConnection.Stream.CanWrite)
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
			"Sending received binary message upstream");
				_upStreamConnection.Stream.Write(messageBytes, 0, messageBytes.Length);

				if (ClientStreamWrapper.Closed)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
			"Downstream connection is broken");
					Close();
				}
				else
				{
					if (_upStreamConnection.Stream.CanRead && !_isReading)
					{

						_upStreamConnection.Stream.BeginRead(
					_responseBuffer, 0, MAX_BUFFER_SIZE, new AsyncCallback(ReadResponse), _upStreamConnection.Stream);
						ClientStreamWrapper.Stream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(OnRead), ClientStreamWrapper);
					}
				}
			}
			else
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
			"Upstream o connection is broken");
				Close();
			}

			/*_responseBuilder.AddChunkReference(_responseBuffer, bytesRead);
				

			HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
			"Sending upstream binary response downstream");

			byte []  response = _responseBuilder.ToArray();
				
			_dataStore.SaveResponse(_messageReqInfo.Id, response);
				
			ClientStreamWrapper.Write(response, 0, response.Length);
									

			//also start listening downstream*/
			
		}

		/// <summary>
		/// Called to read the response asyncroneusly
		/// </summary>
		/// <param name="ar"></param>
		private void ReadResponse(IAsyncResult ar)
		{
			lock (_proxyLock)
			{
				Stream stream = null;
				int bytesRead = 0;
				_isReading = true;

				try
				{
					stream = (Stream)ar.AsyncState;
					bytesRead = stream.EndRead(ar);
					_isReading = false;

					if (bytesRead > 0 && !ClientStreamWrapper.Closed)
					{
						//add to the exiting data
						_responseBuilder.AddChunkReference(_responseBuffer, bytesRead);
						byte[] chunk = new byte[bytesRead];
						Array.Copy(_responseBuffer, chunk, bytesRead);

						//save the response send the response down the pipe


						HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
					"Sending upstream binary response downstream");
						_dataStore.SaveResponse(_messageReqInfo.Id, _responseBuilder.ToArray());
						ClientStreamWrapper.Stream.Write(chunk, 0, chunk.Length);

						if (stream.CanRead && !_isReading)
						{
							_isReading = true;
							//continue reading
							stream.BeginRead(_responseBuffer, 0, MAX_BUFFER_SIZE, new AsyncCallback(ReadResponse), stream);
						}


					}
					else
					{
						Close();
					}

				}
				catch
				{
					Close();
				}
				finally
				{
				}
			}
		}

		public void Close()
		{
			HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
				"Closing upstream and downstream connections");
			ClientStreamWrapper.Close();
			_upStreamConnection.Close();
			_isReading = false;
			IsBusy = false;
		}

		/// <summary>
		/// This is really ignored because Process Request never gets called
		/// </summary>
		protected override IHttpClient HttpClient
		{
			get { return new WebRequestClient(); }
		}
	}
}
