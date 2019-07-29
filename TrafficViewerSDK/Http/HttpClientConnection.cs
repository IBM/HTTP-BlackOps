using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Handles the logic to connect to the site
	/// </summary>
	public class HttpClientConnection
	{
		/// <summary>
		/// The socket wrapper
		/// </summary>
		TcpClient _tcpClient;
		/// <summary>
		/// The current stream
		/// </summary>
		Stream _stream;
		/// <summary>
		/// Gets the current connection stream
		/// </summary>
		public Stream Stream
		{
			get { return _stream; }
		}

		private int _connectionTimeout = 10 * 1000; //10 seconds
		/// <summary>
		/// How long we'll try to connect
		/// </summary>
		public int ConnectionTimeout
		{
			get { return _connectionTimeout; }
			set { _connectionTimeout = value; }
		}

		private const int _receiveTimeout = 60 * 1000;//1 minute
		/// <summary>
		/// How long we will wait for the response
		/// </summary>
		public int ReceiveTimeout
		{
			get { return _receiveTimeout; }
		} 

		/// <summary>
		/// Waits for the connection 
		/// </summary>
		private AutoResetEvent _connectionWait;

		private bool _isHttps;
		private string _host;
		/// <summary>
		/// The connection host
		/// </summary>
		public string Host
		{
			get { return _host; }
		}
		private int _port;
		/// <summary>
		/// The connection port
		/// </summary>
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}


		/// <summary>
		/// Handles the connection
		/// </summary>
		/// <param name="ar"></param>
		private void HandleConnected(IAsyncResult ar)
		{
			try
			{
				_stream = _tcpClient.GetStream();

				//handle https
				if (_isHttps)
				{
					//do a basic SSL handshake here, more to be added if required
					SecureStream(_host);
				}

			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, ex.Message);
				HttpServerConsole.Instance.WriteLine(LogMessageType.Error, ex.Message);
				Close();
				_stream = null;
			}
			finally
			{
				_connectionWait.Set();
			}

		}

		/// <summary>
		/// Secure the connection to the specified host using ssl
		/// </summary>
		/// <param name="host"></param>
		public void SecureStream(string host)
		{
			SslStream secureStream = new SslStream(_stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
			secureStream.AuthenticateAsClient(host, null, System.Security.Authentication.SslProtocols.Tls12, false);
			_stream = secureStream;
		}

		// The following method is invoked by the RemoteCertificateValidationDelegate.
		private static bool ValidateServerCertificate(
			  object sender,
			  X509Certificate certificate,
			  X509Chain chain,
			  SslPolicyErrors sslPolicyErrors)
		{
			//always ignore certificate errors
			return true;
		}

		/// <summary>
		/// Gets a connection to the specified server
		/// </summary>
		/// <returns>True on success</returns>
		public bool Connect()
		{
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                _connectionWait = new AutoResetEvent(false);
                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = _receiveTimeout;
                _tcpClient.BeginConnect(_host, _port, new AsyncCallback(HandleConnected), null);

                _connectionWait.WaitOne(_connectionTimeout);

                bool result = _stream != null;

                return result;
            }

            return true;
			
		}

		/// <summary>
		/// Closes the socket and the underlying stream
		/// </summary>
		public void Close()
		{
			if (_tcpClient != null && _stream!=null)
			{
				try
				{
					_stream.Close();
					_tcpClient.Close();
				}
				catch { }
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="isHttps"></param>
		public HttpClientConnection(string host, int port, bool isHttps)
		{
			_host = host;
			_port = port;

			//handle the situation where the port is not specified
			if (_port == 0)
			{
				if (isHttps)
				{
					_port = 443;
				}
				else
				{
					_port = 80;
				}
			}

			_isHttps = isHttps;
		}

	}
}
