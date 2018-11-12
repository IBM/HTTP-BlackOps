using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Security;
using System.Security.Authentication;
using System.Net.Sockets;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Encapsulates methods to manage the I/O operations with the proxy client including logging in the traffic log for
	/// client connection. 
	/// </summary>
	/// <remarks>It would be ideal for the client to log the traffic however we don't always have control over the client</remarks>
	public class HttpProxyClientStreamWrapper
	{
		/// <summary>
		/// The inner stream
		/// </summary>
		Stream _stream;
		/// <summary>
		/// The tcp client associated with the stream
		/// </summary>
		TcpClient _client;

		/// <summary>
		/// The stream of the wrapper
		/// </summary>
		public Stream Stream
		{
			get { return _stream; }
			set { _stream = value; }
		}

		/// <summary>
		/// Reference to the buffer that is passed by the I/O methods of the wrapper
		/// </summary>
		private byte[] _buffer;

		/// <summary>
		/// Instantiates a client stream using the specified arguments
		/// </summary>
		/// <param name="client">Client obtained for the connection</param>
		public HttpProxyClientStreamWrapper(TcpClient client)
		{
			_stream = client.GetStream();
			_client = client;
		}

		/// <summary>
		/// Whether the wrapper can read from or write to the stream
		/// </summary>
		public bool Closed
		{
			get
			{ 
				bool result = true;
				if (_stream != null)
				{
					result = !(_stream.CanRead || _stream.CanWrite);
				}
				return result;
			}
		}


		/// <summary>
		/// Wether the stream is secure
		/// </summary>
		public bool IsSecure
		{
			get
			{
				return _stream is SslStream;
			}
		}

		/// <summary>
		/// Converts the current tcp stream into a SSL stream
		/// </summary>
		/// <returns>False if we failed to secure the stream</returns>
		public bool SecureStream(string host)
		{
			bool success;
			// Secure the connection for requests contained in the traffic log for others just pass it on to the server
			SslStream secureStream = new SslStream(_stream);
			_stream = secureStream;
			try
			{

                var cert = AppScanProxyCert.Cert;

                if (host != null)
                {
                    try
                    {
                        cert = CertificateAuthority.GetBlackOpsCert(host);
                    }
                    catch (Exception ex)
                    {
                        HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "Could not dynamically generate certificate: {0}", ex.Message);
                        cert = AppScanProxyCert.Cert;//default to the appscan proxy cert
                    }
                }
                secureStream.AuthenticateAsServer(cert, false, SslProtocols.Tls | SslProtocols.Tls11| SslProtocols.Tls12, false);
				
				success = true;
			}
			catch(Exception ex)
			{			
				success = false;
				if (ex is ObjectDisposedException || ex is AuthenticationException || ex is IOException)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "Could not secure the stream with the client: {0}",ex.Message);
					Close();
				}
				else
				{
					throw;
				}
			}

			return success;
		}


		/// <summary>
		/// Calls the write method of the underlying stream
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from the buffer to the stream</param>
		/// <param name="offset">The offset at which to begin copying to the stream</param>
		/// <param name="count">The number of bytes to be written to the current stream</param>
		/// <returns>True on success</returns>
		public bool Write(byte[] buffer, int offset, int count)
		{
			if (_stream == null || !_stream.CanWrite)
			{
				return false;
			}

			bool success = true;

			try
			{
				_buffer = buffer; //save the reference to the buffer
				_stream.Write(buffer, offset, count);
			}
			catch (Exception ex)
			{
				if(ex is IOException || ex is ObjectDisposedException)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "The transport stream was broken while sending a response");
					success = false;
					Close();
				}
				else
				{
					throw; //some other exception occured, probably related to the args
				}

			}
			return success;
		}

		/// <summary>
		/// Calls the read method of the underlying stream
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from the stream into the buffer</param>
		/// <param name="offset">The offset at which to begin copying to the buffer</param>
		/// <param name="count">The number of bytes to be read from the current stream</param>
		/// <returns>Bytes read</returns>
		public int Read(byte[] buffer, int offset, int count)
		{
			if (_stream == null || !_stream.CanRead)
			{
				return 0;
			}
			
			int bytesRead = 0;
			try
			{
				_buffer = buffer;
				bytesRead = _stream.Read(buffer, offset, count);
			}
			catch (Exception ex)
			{
				if (ex is IOException || ex is ObjectDisposedException)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "The transport stream was broken while reading a request");
					Close();
				}
				else
				{
					throw;
				}

			}

			return bytesRead;
		}

		/// <summary>
		/// Starts an asyncroneus read operation for the underlying stream
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from the stream into the buffer</param>
		/// <param name="offset">The offset at which to begin copying to the buffer</param>
		/// <param name="count">The number of bytes to be read from the current stream</param>
		/// <param name="callback">Callback method to be executed when bytes were read from the stream</param>
		/// <param name="state">Data to be passed to the callback method</param>
		/// <returns>Null if an exception happened</returns>
		public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (_stream == null || !_stream.CanRead)
			{
				return null;
			}
		
			IAsyncResult result = null;
			try
			{
				_buffer = buffer; //save the reference to the buffer so we can reuse it later in EndRead
				result = _stream.BeginRead(buffer, offset, count, callback, state);
			}
			catch (Exception ex)
			{
				if (ex is IOException || ex is ObjectDisposedException)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "The transport stream was broken while starting to read a request");
					Close();
				}
				else
				{
					throw;
				}
			}
			return result;
		}

		/// <summary>
		/// Waits for the pending asyncroneus read to complete
		/// </summary>
		/// <param name="asyncResult">The reference to the pending asyncroneus request</param>
		/// <returns>Bytes read</returns>
		public int EndRead(IAsyncResult asyncResult)
		{
			if (_stream == null || !_stream.CanRead)
			{
				return 0;
			}
			
			int bytesRead = 0;
			try
			{
				bytesRead = _stream.EndRead(asyncResult);
			}
			catch (Exception ex)
			{
				if (ex is IOException || ex is ObjectDisposedException)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Warning, "The transport stream was broken while finishing to read a request");
				}
				else
				{
					throw;
				}
			}
			return bytesRead;
		}

		/// <summary>
		/// Closes the underlying stream
		/// </summary>
		public void Close()
		{
			if (_stream != null)
			{
				try
				{
					_stream.Close();
                    _client.Close();
				}
				catch { }
			}
		}


	}
}
