using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Processes commands
	/// /start/proxyPort
	/// /stop/proxyPort/fileName
	/// </summary>
	public class CommandProxyConnection : BaseProxyConnection
	{
		private CommandProxy _commandProxy;
		private CommandProxyHttpClient _commandProxyClient;
		/// <summary>
		///  Processes commands sent to the command proxy
		/// /start/proxyPort
		/// /stop/proxyPort>/fileName
		/// </summary>
		/// <param name="commandProxy"></param>
		/// <param name="client"></param>
		public CommandProxyConnection(CommandProxy commandProxy, TcpClient client) : base(client, false)
		{
			_commandProxy = commandProxy;
			_commandProxyClient = new CommandProxyHttpClient(commandProxy);
		}


		/// <summary>
		/// Gets the http client for the connection
		/// </summary>
		protected override IHttpClient HttpClient
		{
			get { return _commandProxyClient; }
		}
	}
}
