using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	///  Generates an HTTP response for an error in a proxy connection.
	/// </summary>
	public class HttpErrorResponse
	{
		/// <summary>
		/// Generates an HTTP response for an error in a proxy connection. The response headers
		/// should indicate to the browser that the connection will be closed.
		/// </summary>
		/// <param name="statusCode">An HTTP status code for the error (e.g., 4xx, 5xx, etc.)</param>
		/// <param name="reason">A reason phrase for the HTTP status line</param>
		/// <param name="serviceCode">CRAWE Service Code</param>
		/// <param name="args">Optional arguments for the error message</param>
		/// <returns>An HTTP response containing the error message encoded using UTF-8</returns>
		public static byte[] GenerateHttpErrorResponse(HttpStatusCode statusCode, string reason, ServiceCode serviceCode, params object[] args)
		{
			string message = String.Format("<html>" +
												"<head>" +
													"<title>{0}: {1}</title>" +
												"</head>" +
												"<body>" +
													"<h1>{2}<br/></h1>" +
												"</body>" +
											"</html>",
											TrafficViewerSDK.Properties.Resources.ErrorCodeTitle, statusCode, 
											Utils.HtmlEncode(TrafficViewerSDK.Properties.Resources.HttpBlackOpsErrorMessage));

			return GenerateHttpErrorResponse(statusCode, reason, message);
		}


		/// <summary>
		/// Generates an HTTP response for an error in a proxy connection. The response headers
		/// should indicate to the browser that the connection will be closed.
		/// </summary>
		/// <param name="statusCode">An HTTP status code for the error (e.g., 4xx, 5xx, etc.)</param>
		/// <param name="reason">A reason phrase for the HTTP status line</param>
		/// <param name="message">String value to be displayed</param>
		/// <returns>An HTTP response containing the error message encoded using UTF-8</returns>
		public static byte[] GenerateHttpErrorResponse(HttpStatusCode statusCode, string reason, string message)
		{

			string httpResponse = "HTTP/1.1 " + (int)statusCode + " " + reason + "\r\n" +
								  "Connection: close\r\n" +
								  "Proxy-Connection: close\r\n" +
								  "Pragma: no-cache\r\n" +
								  "Cache-Control: no-cache\r\n" +
								  "Content-Type: text/html; charset=utf-8\r\n" +
                                  "Content-Length: " + Constants.DefaultEncoding.GetByteCount(message) + "\r\n" +
								  "\r\n" +
								  message;
			// UTF-8 is safe for the status line and headers since it is backward-compatible with US-ASCII
            return Constants.DefaultEncoding.GetBytes(httpResponse);
		}
	}
}
