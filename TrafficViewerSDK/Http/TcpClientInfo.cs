using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Stores information about the TCP client
	/// </summary>
	public class TcpClientInfo
	{
		private TcpClient _client;
		/// <summary>
		/// The TCP client object
		/// </summary>
		public TcpClient Client
		{
			get { return _client; }
			set { _client = value; }
		}

		private bool _isSecure = false;
		/// <summary>
		/// Whether the connection came on a secure port or not
		/// </summary>
		public bool IsSecure
		{
			get { return _isSecure; }
			set { _isSecure = value; }
		}

		/// <summary>
		/// Constructor for TCPClientInfo
		/// </summary>
		/// <param name="client"></param>
		/// <param name="isSecure"></param>
		public TcpClientInfo(TcpClient client, bool isSecure)
		{
			_client = client;
			_isSecure = isSecure;
		}
	}
}
