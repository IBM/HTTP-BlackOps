using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Handles the logic for the Command Proxy REST service
	/// </summary>
	public class CommandProxyHttpClient : IHttpClient
	{
		private ILogWriter _logWriter;

		private const string START_PAGE = "/start";
		private const string STOP_PAGE = "/stop";
		private const string PORT_PARAM = "port";
		private const string FILE_PARAM = "fileName";
		private CommandProxy _commandProxy;

		/// <summary>
		/// Handles the logic for the Command Proxy REST service
		/// </summary>
		/// <param name="commandProxy"></param>
		public CommandProxyHttpClient(CommandProxy commandProxy)
		{
			_commandProxy = commandProxy;
			_logWriter = TrafficViewerSDK.SdkSettings.Instance.Logger;
		}

		/// <summary>
		/// Called when a request comes into the proxy server
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		public HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
		{
			int port;
			HttpResponseInfo respInfo = null;
			requestInfo.ParseVariables();

			if (String.Compare(START_PAGE, requestInfo.Path, true) == 0)
			{
				port = ExtractPort(requestInfo);
				_commandProxy.StartManualExploreProxy(port);
				respInfo = GetSuccessResponse(Resources.RecordingStarted);
				
			}
			else if (String.Compare(STOP_PAGE, requestInfo.Path, true) == 0)
			{
				port = ExtractPort(requestInfo);
				string fileName = null;
				if (requestInfo.QueryVariables.ContainsKey(FILE_PARAM))
				{
					fileName = Utils.UrlDecode(requestInfo.QueryVariables[FILE_PARAM]);
				}
				_commandProxy.StopManualExploreProxy(port, fileName);
				respInfo = GetSuccessResponse(Resources.RecordingStopped);
			}
			else
			{
				_logWriter.Log(TraceLevel.Error, "An invalid url was specified: {0}", requestInfo.FullUrl);
				throw new HttpProxyException(HttpStatusCode.NotImplemented, "Not implemented", ServiceCode.CommandProxyNotImplemented);
			}

			return respInfo;
		}

		/// <summary>
		/// Gets a success response to the commands
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private HttpResponseInfo GetSuccessResponse(string message)
		{
			string response = String.Format("HTTP/1.1 200 OK\r\nContent-Type: text/plain;charset=UTF-8\r\n\r\n{0}", message);
			HttpResponseInfo respInfo = new HttpResponseInfo(response);
			return respInfo;
		}

		/// <summary>
		/// Extracts the port value from the request
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		private int ExtractPort(HttpRequestInfo requestInfo)
		{
			int port = -1;

			string portString = null;
			if(requestInfo.QueryVariables.ContainsKey(PORT_PARAM))
			{
				portString = requestInfo.QueryVariables[PORT_PARAM];
			}

			if (!Utils.ParsePort(portString, out port))
			{
				_logWriter.Log(TraceLevel.Error, "An invalid port was specified: {0}", port);
				throw new HttpProxyException(HttpStatusCode.BadRequest, "Invalid port value", ServiceCode.CommandProxyStartInvalidPort);
			}

			return port;
		}

		/// <summary>
		/// Does nothing for this client
		/// </summary>
		/// <param name="networkSettings"></param>
		public void SetNetworkSettings(INetworkSettings networkSettings)
		{
			; //do nothing
		}
	}
}
