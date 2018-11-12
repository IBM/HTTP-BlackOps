using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficServer.Properties;
using TrafficViewerSDK;

namespace TrafficServer
{
	internal static class FireBugLite
	{
		private static DateTime _lastRequested;
		private const long MINIMUM_REQUEST_INTERVAL = 1 * 1000 * 10000; //1 second

		/// <summary>
		/// String to add to the page html
		/// </summary>
		private const string FIREBUG_LITE_STRING = "<script type=\"text/javascript\" src=\"https://getfirebug.com/firebug-lite.js\"></script>";

		/// <summary>
		/// Adds the firebug string to the response
		/// </summary>
		/// <param name="responseInfo">Source response info</param>
		/// <returns>Modified response infor</returns>
		public static HttpResponseInfo AddToResponse(HttpResponseInfo responseInfo)
		{
			//construct a new response

			//first insure that this is not part of a scan or a series of inline requests by checking the time interval
			//betweeb the last transformation and the curren
			TimeSpan ts = DateTime.Now.Subtract(_lastRequested);

			if (ts.Ticks > MINIMUM_REQUEST_INTERVAL)
			{
				string body = responseInfo.ResponseBody.ToString();

				bool replaced = true;
				int indexOfHead = body.IndexOf("<head", StringComparison.OrdinalIgnoreCase);

				if (indexOfHead != -1)
				{
					int endTagIndex = body.IndexOf(">", indexOfHead);

					body = body.Insert(endTagIndex + 1, FIREBUG_LITE_STRING);
				}
				else
				{
					int indexOfBody = body.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
					if (indexOfBody != -1)
					{
						body = body.Insert(indexOfBody, "<head>" + FIREBUG_LITE_STRING + "</head>");
					}
					else
					{
						replaced = false;
					}

				}

				if (replaced)
				{
					StringBuilder newResponse = new StringBuilder();
					newResponse.AppendLine(responseInfo.StatusLine);
					responseInfo.Headers.Remove("Transfer-Encoding");
					newResponse.AppendLine(responseInfo.Headers.ToString());
					newResponse.AppendLine();
					newResponse.Append(body);

					HttpResponseInfo newResponseInfo = new HttpResponseInfo();
					newResponseInfo.ProcessResponse(newResponse.ToString());

					responseInfo = newResponseInfo;

					//save the time of the last transformation
					_lastRequested = DateTime.Now;
				}
			}
			return responseInfo;
		}

	}
}
