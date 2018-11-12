using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Diagnostics;
using System.Security.Authentication;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Web;
using TrafficViewerSDK;
using System.Threading;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.Http
{

	/// <summary>
	/// Listens for incoming HTTP connections
	/// </summary>
	public abstract class BaseProxy : IHttpProxy
	{
		private int _connectionLimit = 1000;
		/// <summary>
		/// The maximum number of connections that will be started at a time
		/// </summary>
		public int ConnectionLimit
		{
			get { return _connectionLimit; }
			set { _connectionLimit = value; }
		}


		private TcpListener _listener;
		private TcpListener _secureListener;
		private Queue<IProxyConnection> _connections = new Queue<IProxyConnection>();
		private Queue<TcpClientInfo> _clientQueue = new Queue<TcpClientInfo>();
		private string _defaultLocalAddress;
		private IPAddress _localAddress;
		private string _host;
		private int _port;
		private int _securePort;
		private bool _forcePort = false;
		/// <summary>
		/// Whether the proxy uses a dynamic port
		/// </summary>
		protected bool _usesDynamicPort;
		/// <summary>
		/// Whether the proxy uses a dynamic secure port
		/// </summary>
		protected bool _usesDynamicPortSecure;


		private bool _listening = false;
		private readonly object _lockConnections = new object();

		private INetworkSettings _networkSettings = new DefaultNetworkSettings();
        private int MAX_PORT = 65535;
		/// <summary>
		/// Gets the network settings used by the proxy
		/// Can be used to configure the proxy to use a different proxy for the connection to site
		/// Can be used to configure invalid certificate callbacks
		/// Can be used to pass on credentials for a specific host (do not pass on credentials that are not tied to a host
		/// as it is a security concern)
		/// </summary>
		public INetworkSettings NetworkSettings
		{
			get { return _networkSettings; }
		}


        Dictionary<string, string> _extraOptions = new Dictionary<string,string>();
        /// <summary>
        /// Extra option for http client
        /// </summary>
        protected const string HTTP_CLIENT_PROXY_HOST = "HttpClientProxyHost";
        /// <summary>
        /// Extra option for http client
        /// </summary>
        protected const string HTTP_CLIENT_PROXY_PORT = "HttpClientProxyPort";
        /// <summary>
        /// Collection used to pass in additional options
        /// </summary>
        public Dictionary<string, string> ExtraOptions
        {
            get { return _extraOptions; }
        }

		#region Properties


		/// <summary>
		/// Sets the local IP address that this proxy listens on
		/// </summary>
		public string LocalAddress
		{
			set
			{
				if (String.IsNullOrEmpty(_defaultLocalAddress) ||
						!IPAddress.TryParse(_defaultLocalAddress, out _localAddress))
				{
					if (String.IsNullOrEmpty(value) ||
							!IPAddress.TryParse(value, out _localAddress))
					{
						_localAddress = Socket.OSSupportsIPv6 ? IPAddress.IPv6Any : IPAddress.Any;
					}
				}
			}
		}

		/// <summary>
		/// Port that this proxy is listening on
		/// </summary>
		/// <remarks>
		/// Will be 0 if this proxy uses a dynamic port and is not 
		/// currently listening on a port.
		/// </remarks>
		public int Port
		{
			get { return _port; }
            set { _port = value; }
		}

		/// <summary>
		/// Port that this proxy is listening on for secure connections
		/// </summary>
		public int SecurePort
		{
			get { return _securePort; }
            set { _securePort = value; }
		}


		/// <summary>
		/// Indicates whether the proxy dynamically finds an available port when 
		/// it starts listening
		/// </summary>
		public bool UsesDynamicPort
		{
			get
			{
				return _usesDynamicPort;
			}
		}

		/// <summary>
		/// Indicates whether the proxy dynamically finds an available secure port when 
		/// it starts listening
		/// </summary>
		public bool UsesDynamicPortSecure
		{
			get { return _usesDynamicPortSecure; }
		}

		/// <summary>
		/// Name of host this proxy resides on
		/// </summary>
		public string Host
		{
			get
			{
				if (IPAddress.Any.Equals(_localAddress) || IPAddress.IPv6Any.Equals(_localAddress))
				{
					if (String.IsNullOrEmpty(_host))
					{
						_host = Dns.GetHostName();
					}
					return _host;
				}
				return _localAddress.ToString();
			}
			set
			{
				_host = value;
                //try to resolve domain of the specified address
                
                IPAddress[] addresses = new IPAddress[0];
                try
                {
                    addresses = Dns.GetHostAddresses(_host);
                }
                catch (Exception ex)
                {
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, ex.ToString());
                }
                //start the proxy on the first host
                if (addresses.Length > 0)
                {
                    _defaultLocalAddress = addresses[0].ToString();
                    LocalAddress = _defaultLocalAddress;
                }
                else
                {
                    throw new HttpProxyException(HttpStatusCode.InternalServerError, "Invalid proxy host", ServiceCode.InvalidProxyHost);
                }
			}
		}

		/// <summary>
		/// Indicates whether this proxy is listening for connections
		/// </summary>
		public bool IsListening
		{
			get { return _listening; }
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="securePort"></param>
		/// <param name="forcePort">Do not automatically start on the next available port if the specified port is taken</param>
		public BaseProxy(string host, int port, int securePort, bool forcePort = false)
		{
            Host = host;
			_port = port;
			_securePort = securePort;
			_forcePort = forcePort;

            ExtraOptions.Add(HTTP_CLIENT_PROXY_HOST, String.Empty);
            ExtraOptions.Add(HTTP_CLIENT_PROXY_PORT, String.Empty);


			// If port is 0, then dynamically get an available port when told to start
			_usesDynamicPort = (_port == 0);
			_usesDynamicPortSecure = (_securePort == 0);

			


		}

		/// <summary>
		/// Stops the proxy when the object is disposed
		/// </summary>
		~BaseProxy()
		{
			this.Stop();
		}

		/// <summary>
		/// Gets the proxy connection to be used
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		protected abstract IProxyConnection GetConnection(TcpClientInfo clientInfo);

		private void OnAccept(IAsyncResult ar)
		{
            if (!_listening) return;//the proxy was stopped
			TcpListener listener = (TcpListener)ar.AsyncState;
			try
			{
				int port = ((IPEndPoint)listener.LocalEndpoint).Port;
				bool isSecure = port == SecurePort;

				TcpClient client = listener.EndAcceptTcpClient(ar);


				// Process this connection
				ProcessClient(new TcpClientInfo(client, isSecure));


				// Keep Listening
				listener.BeginAcceptTcpClient(new AsyncCallback(OnAccept), (object)listener);
			}
			catch (SocketException ex)
			{
				HttpServerConsole.Instance.WriteLine(
					String.Format("Error accepting connection in Traffic Server: {0}", ex.Message));
				// Keep Listening
				listener.BeginAcceptTcpClient(new AsyncCallback(OnAccept), (object)listener);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception ex)
			{
				HttpServerConsole.Instance.WriteLine(ex);
			}
		}

		/// <summary>
		/// Prunes the collections list and initializes new connections
		/// </summary>
		private void PruneConnections(object state)
		{
			try
			{
                if (!_listening) return;

				Queue<IProxyConnection> connectionsToStart = new Queue<IProxyConnection>();

				lock (_lockConnections)
				{
					IProxyConnection conn;
					int counter = 0;

					//first clear dead connections 
					while (counter < _connections.Count)
					{
						conn = _connections.Dequeue();
                        if (conn.Closed && _connections.Count >= _connectionLimit) 
                        { 
                            //call connection stop just in case we have a lingering thread
                            //this should set the _stop flag to true and end any async operations
                            conn.Stop();
                        }
                        else //if the connection is not closed put it back at the end of the queue
						{
							_connections.Enqueue(conn);
							counter++; //update the counter to show this connection was processed
						}
					}

					//if the number is still high clear connections that are not busy
					//find connections that idle and drop them starting with the oldest in the list
					counter = 0;
					while (_connections.Count >= _connectionLimit && counter < _connections.Count)
					{
						conn = _connections.Dequeue();
						if (!conn.IsBusy)
						{
							conn.Stop();
							HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
								"Max clients exceeded. Idle connection was dropped");
						}
						else
						{
							//put the connection back to the end of the queue
							_connections.Enqueue(conn);
							counter++;
						}
					}


					//lastly if the number is still high forcefully kill the oldest connections until we get below the limit
					//this should be a rare situation and something may be wrong with the connection, a very slow client
					while (_connections.Count >= _connectionLimit)
					{
						conn = _connections.Dequeue();
						conn.Stop();
						HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
							"Max clients exceeded. Older connections were dropped");
					}
					
					//now that we ensured there's room dequeue any pending connections and start them
					while (_connections.Count < _connectionLimit && _clientQueue.Count > 0)
					{
						TcpClientInfo clientInfo = _clientQueue.Dequeue();

						if (clientInfo.Client.Connected)
						{
							// Create the new connection
							conn = GetConnection(clientInfo);

							// Add the connection to list of connections
							_connections.Enqueue(conn);
							//adding the connection to a separate collection in order to 
							//start it outside the lock
							connectionsToStart.Enqueue(conn);
						}
					}
				}//end critical section

				//start connections
				while (connectionsToStart.Count > 0 && _listening)
				{
					IProxyConnection conn = connectionsToStart.Dequeue();
					conn.Start();
				}
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "BaseProxy.PruneConnections: Caught exception: {0}", ex.ToString());
			}
		}

		private void ProcessClient(TcpClientInfo clientInfo)
		{
            if (!_listening) return;//the proxy was stopped
			//queue the client
			_clientQueue.Enqueue(clientInfo);

			PruneConnections(null);
		}

		private void StartListener(ref TcpListener listener, ref bool usesDynamicPort)
		{
			try
			{
				listener.Start();
			}
			catch (SocketException)
			{
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
				if (_forcePort)
				{
					//we failed to start the proxy
					SdkSettings.Instance.Logger.Log(TraceLevel.Error, "The port {0} is in use.", port);
					throw new HttpProxyException(HttpStatusCode.ServiceUnavailable, "Port in use", ServiceCode.CommandProxyPortInUse, port);
				}
				else {
                    bool failed = false;
                    do
                    {
                        port++;
                        listener = new TcpListener(_localAddress, port);
                        usesDynamicPort = true;
                        try
                        {
                            listener.Start();
                        }
                        catch (SocketException ex)
                        {
                            failed = true;
                            //if we are still getting exceptions try starting the listener on localhost
                            HttpServerConsole.Instance.WriteLine(ex);
                            IPAddress.TryParse("127.0.0.1", out _localAddress);
                            listener = new TcpListener(_localAddress, 0);
                            listener.Start();
                        }
                    }
                    while (failed && port < MAX_PORT);
				}
				
			}
			catch (Exception ex)
			{
				//this is an unknown exception
				HttpServerConsole.Instance.WriteLine(ex);
				_listener = null;
				_secureListener = null;
			}


		}

		/// <summary>
		/// Start listening for connections
		/// </summary>
		/// <exception cref="InvalidOperationException">Proxy was already started</exception>
		virtual public void Start()
		{
			//set network options
            int clientProxyPort = 0;
            if(ExtraOptions.ContainsKey(HTTP_CLIENT_PROXY_HOST) &&
                ExtraOptions.ContainsKey(HTTP_CLIENT_PROXY_PORT) &&
                !String.IsNullOrWhiteSpace(ExtraOptions[HTTP_CLIENT_PROXY_HOST]) &&
                !String.IsNullOrWhiteSpace(ExtraOptions[HTTP_CLIENT_PROXY_PORT]) && 
                int.TryParse(ExtraOptions[HTTP_CLIENT_PROXY_PORT],out clientProxyPort))
            {
                //override defaults
                _networkSettings.WebProxy = new WebProxy(ExtraOptions[HTTP_CLIENT_PROXY_HOST], clientProxyPort);
            }

			if (_listening)
			{
				throw new InvalidOperationException("Attempted to start proxy when already started");
			}


			_listener = new TcpListener(_localAddress, Port);
			_secureListener = new TcpListener(_localAddress, SecurePort);

			//start the unsecure port
			StartListener(ref _listener, ref _usesDynamicPort);
			//start the secure port
			StartListener(ref _secureListener, ref _usesDynamicPortSecure);

			if (_listener != null && _secureListener != null)
			{
				// Expose which port we're using if it was dynamically selected
				if (_usesDynamicPort)
				{
					_port = ((IPEndPoint)_listener.LocalEndpoint).Port;
				}
				if (_usesDynamicPortSecure)
				{
					_securePort = ((IPEndPoint)_secureListener.LocalEndpoint).Port;
				}

				HttpServerConsole.Instance.WriteLine(String.Format("Waiting on Host: {0}, HTTP Port: {1}, Secure Port: {2}", _localAddress, _port, _securePort));

				_listener.BeginAcceptTcpClient(new AsyncCallback(OnAccept), _listener);
				_secureListener.BeginAcceptTcpClient(new AsyncCallback(OnAccept), _secureListener);
				_listening = true;

			}
		}

		/// <summary>
		/// Stop listening for connections
		/// </summary>
		virtual public void Stop()
		{
			if (!_listening)
			{
				return;
			}
            //indicate that the listening has stopped
            _listening = false;


            lock (_lockConnections)
            {
                _listener.Stop();
                _listener = null;

                _secureListener.Stop();
                _secureListener = null;
                
                foreach (IProxyConnection conn in _clientQueue)
                {
                    conn.Stop();
                }
                _clientQueue.Clear();
                ClearConnections();
                
            }

            

			if (_usesDynamicPort)
			{
				// Set port back to 0 so it will be chosen dynamically again if proxy is restarted
				_port = 0;
			}
			if (_usesDynamicPortSecure)
			{
				// Set port back to 0 so it will be chosen dynamically again if proxy is restarted
				_securePort = 0;
			}

			
			

            HttpServerConsole.Instance.WriteLine("Stopping Proxy");


			
			
		}

		private void ClearConnections()
		{
			foreach (IProxyConnection c in _connections)
			{
				c.Stop();//stop the connection
			}
			_connections.Clear();
		}


	}
}
