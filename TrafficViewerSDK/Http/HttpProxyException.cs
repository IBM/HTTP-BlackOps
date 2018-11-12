using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Exception that can be caught and handled in an Http Response
	/// </summary>
	public class HttpProxyException : Exception
	{
		private HttpStatusCode _statusCode;
		/// <summary>
		/// Gets the HTTP status code for this exception
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get { return _statusCode; }
		}

		private string _statusLine;
		/// <summary>
		/// Gets the status line
		/// </summary>
		public string StatusLine
		{
			get { return _statusLine; }
		}
		private ServiceCode _serviceCode;
		/// <summary>
		/// Gets the service code for the message
		/// </summary>
		public ServiceCode ServiceCode
		{
			get { return _serviceCode; }
		}

		private object[] _messageArgs;
		/// <summary>
		/// Gets message args
		/// </summary>
		public object[] MessageArgs
		{
			get { return _messageArgs; }
		}

		/// <summary>
		/// Exception that can be caught and handled in an Http Response
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="statusLine"></param>
		/// <param name="serviceCode"></param>
		/// <param name="args"></param>
		public HttpProxyException(HttpStatusCode statusCode, string statusLine, ServiceCode serviceCode, params object [] args)
		{
			_statusCode = statusCode;
			_statusLine = statusLine;
			_serviceCode = serviceCode;
			_messageArgs = args;
		}
	}
}
